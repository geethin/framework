using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GT.CLI.Common
{
    /// <summary>
    /// 类型解析帮助类
    /// </summary>
    public class TypeHelper
    {
        public string Name { get; }
        public List<PropertyInfo> PropertyInfos { get; }
        public TypeHelper(string filePath)
        {
            if (File.Exists(filePath))
            {
                PropertyInfos = GetPropertyInfos(filePath);

                var content = File.ReadAllText(filePath);
                var tree = CSharpSyntaxTree.ParseText(content);
                var root = tree.GetCompilationUnitRoot();
                // 获取当前类名
                var classDeclarationSyntax = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
                Name = classDeclarationSyntax?.Identifier.ToString();
            }
        }

        /// <summary>
        /// 获取该类的所有属性
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<PropertyInfo> GetPropertyInfos(string filePath)
        {
            var properties = new List<PropertyInfo>();
            if (!File.Exists(filePath))
            {
                return properties;
            }

            var content = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = tree.GetCompilationUnitRoot();
            var compilation = CSharpCompilation.Create("tmp", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            var specialTypes = new[] { "DateTime", "DateTimeOffset", "Guid" };
            properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Select(prop =>
                {
                    var trivia = prop.GetLeadingTrivia();
                    var typeInfo = semanticModel.GetTypeInfo(prop.Type);
                    var metadataName = typeInfo.Type.MetadataName.ToString();

                    var type = prop.Type.ToString();
                    var name = prop.Identifier.ToString();

                    var propertyInfo = new PropertyInfo(type, name)
                    {
                        AttributeText = string.Join("\r\n", prop.AttributeLists.Select(a => a.ToString())),
                        Comments = trivia.ToString().TrimEnd(' '),
                    };
                    if (type.Equals("decimal"))
                    {
                        propertyInfo.IsDecimal = true;
                    }
                    if (prop.Type.IsNotNull)
                    {
                        propertyInfo.IsRequired = true;
                    }
                    var attributes = prop.DescendantNodes().OfType<AttributeSyntax>().ToList();
                    if (attributes != null && attributes.Count > 0)
                    {
                        var maxLength = attributes.Where(a => a.Name.ToString().Equals("MaxLength"))
                            .Select(a => a.ArgumentList.Arguments.FirstOrDefault())
                            .FirstOrDefault();
                        var minLength = attributes.Where(a => a.Name.ToString().Equals("MinLength"))
                            .Select(a => a.ArgumentList.Arguments.FirstOrDefault())
                            .FirstOrDefault();
                        if (maxLength != null)
                        {
                            propertyInfo.MaxLength = Convert.ToInt32(maxLength.ToString());
                        }
                        if (minLength != null)
                        {
                            propertyInfo.MinLength = Convert.ToInt32(minLength.ToString());
                        }
                    }
                    // TODO:此判断不准确
                    if (((INamedTypeSymbol)typeInfo.Type).IsGenericType)
                    {
                        propertyInfo.IsList = true;
                    }
                    else if (typeInfo.Type.OriginalDefinition.ToString() == metadataName && !specialTypes.Contains(metadataName))
                    {
                        propertyInfo.IsReference = true;
                    }
                    return propertyInfo;
                }).ToList();
            // 获取当前类名
            var classDeclarationSyntax = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();

            // 继承的类名
            BaseListSyntax baseList = classDeclarationSyntax.BaseList;
            if (baseList != null)
            {
                var baseType = baseList.DescendantNodes().OfType<SimpleBaseTypeSyntax>()
                    .FirstOrDefault(node => !node.ToFullString().StartsWith("I"))?.Type;
                var baseTypeInfo = semanticModel.GetTypeInfo(baseType);
                // 如果找到父类，则添加父类中的属性
                if (!string.IsNullOrEmpty(baseTypeInfo.Type.Name))
                {
                    var dir = new FileInfo(filePath).Directory.FullName;
                    var parentProperties = GetPropertyInfos(Path.Combine(dir, baseTypeInfo.Type.Name + ".cs"));
                    properties.AddRange(parentProperties);
                }
            }
            return properties.GroupBy(p => p.Name).Select(s => s.FirstOrDefault()).ToList();
        }

        public class PropertyInfo
        {
            public string Type { get; }
            public string Name { get; }
            public string DisplayName { get; set; }
            /// <summary>
            /// 是否是数组
            /// </summary>
            public bool IsList { get; set; } = false;
            /// <summary>
            /// 是否为引用类型
            /// </summary>
            public bool IsReference { get; set; } = false;
            public string AttributeText { get; set; }
            public string Comments { get; set; }
            public bool IsRequired { get; set; } = false;
            public int? MinLength { get; set; }
            public int? MaxLength { get; set; }
            public bool IsDecimal { get; set; } = false;
            public PropertyInfo(string type, string name)
            {
                Type = type;
                Name = name;
            }

            /// <summary>
            /// 转换成C#属性
            /// </summary>
            /// <returns></returns>
            public string ToCsharpLine()
            {
                if (!string.IsNullOrEmpty(AttributeText))
                {
                    AttributeText = AttributeText.Trim();
                    AttributeText = $@"        {AttributeText}
";
                }
                else
                {
                    AttributeText = string.Empty;
                }
                var content = @$"        public {Type} {Name} {{ get; set; }}";
                // 如果是引用对象
                if (IsReference)
                {
                    content = @$"        // public {Type}Dto {Name} {{ get; set; }}";
                }
                return $@"{Comments}{AttributeText}{content}
";
            }

            /// <summary>
            /// 转换成前端表单控件
            /// </summary>
            /// <returns></returns>
            public string ToNgInputControl()
            {
                var input = new NgInputBuilder(Type, Name, DisplayName)
                {
                    IsDecimal = IsDecimal,
                    IsRequired = IsRequired,
                    MaxLength = MaxLength,
                    MinLength = MinLength
                };
                return input.ToString();
            }
        }

    }
}
