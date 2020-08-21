using GT.CLI.Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static GT.CLI.Common.TypeHelper;

namespace GT.CLI.Commands
{
    /// <summary>
    /// 数据仓储生成
    /// </summary>
    public class RepositoryGenerate : GenerateBase
    {
        public string EntityPath { get; set; }
        public string ServicePath { get; set; }
        public RepositoryGenerate(string entityPath, string servicePath)
        {
            EntityPath = entityPath;
            ServicePath = servicePath;
        }

        /// <summary>
        /// 生成仓储
        /// </summary>
        public void GenerateReponsitory()
        {
            // 获取生成需要的实体名称
            if (!File.Exists(EntityPath))
            {
                return;
            }
            var content = File.ReadAllText(EntityPath);
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = tree.GetCompilationUnitRoot();
            var classDeclarationSyntax = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            string className = classDeclarationSyntax.Identifier.ToString();
            // 获取数据库上下文信息
            var cpl = new CompilationHelper(ServicePath, new DirectoryInfo(ServicePath).Name);
            var classes = cpl.GetAllClasses();
            var contextClass = cpl.GetClassNameByBaseType(classes, "DbContext").FirstOrDefault();
            var contextName = contextClass?.Name ?? "ContextBase";
            // 生成基础仓储实现类，替换模板变量并写入文件
            var tplContent = GetTplContent("Repository.tpl");
            tplContent = tplContent.Replace("{$ContextName}", contextName);
            SaveToFile(Path.Combine(ServicePath, "Repositories"), "Repository.cs", tplContent);
            // 生成当前实体的仓储服务
            tplContent = GetTplContent("MyRepository.tpl");
            tplContent = tplContent.Replace("{$EntityName}", className);
            tplContent = tplContent.Replace("{$ContextName}", contextName);
            SaveToFile(Path.Combine(ServicePath, "Repositories"), className + "Repository.cs", tplContent);
            Console.WriteLine("仓储生成完成");
        }

        /// <summary>
        /// 生成dtos
        /// </summary>
        public void GenerateDtos()
        {
            if (!File.Exists(EntityPath))
            {
                Console.WriteLine(EntityPath + " not found!");
                return;
            }
            var typeHelper = new TypeHelper(EntityPath);
            var properties = typeHelper.PropertyInfos;
            string className = typeHelper.Name;

            // 创建相关dto文件
            var referenceProps = properties.Where(p => p.IsReference)
                    .Select(s => new PropertyInfo("Guid?", s.Name + "Id"))
                    .ToList();
            var addDto = new DtoInfo
            {
                Name = className + "AddDto",
                Tag = className,
                Properties = properties.Where(p => p.Name != "Id" && p.Name != "CreatedTime" && !p.IsList && !p.IsReference).ToList()
            };
            addDto.Properties.AddRange(referenceProps);

            var updateDto = new DtoInfo
            {
                Name = className + "UpdateDto",
                Tag = className,
                Properties = properties.Where(p => p.Name != "Id" && p.Name != "CreatedTime" && !p.IsList && !p.IsReference).ToList()
            };
            // 列表项dto
            var ItemDto = new DtoInfo
            {
                Name = className + "Dto",
                Tag = className,
                Properties = properties.Where(p => !p.IsList).ToList()
            };
            var DetailDto = new DtoInfo
            {
                Name = className + "DetailDto",
                Tag = className,
                Properties = properties
            };
            var FilterDto = new DtoInfo
            {
                Name = className + "Filter",
                Tag = className,
                BaseType = "FilterBase",
                Properties = referenceProps
            };
            // TODO:可能存在自身到自身的转换
            addDto.Save(ServicePath);
            updateDto.Save(ServicePath);
            ItemDto.Save(ServicePath);
            DetailDto.Save(ServicePath);
            FilterDto.Save(ServicePath);
            Console.WriteLine("生成dto模型完成");

            // 添加autoMapper配置
            GenerateAutoMapperProfile(className);
        }

        /// <summary>
        /// 生成AutoMapperProfile
        /// </summary>
        /// <param name="entityName"></param>
        protected void GenerateAutoMapperProfile(string entityName)
        {
            string code =
@$"            CreateMap<{entityName}AddDto, {entityName}>();
            CreateMap<{entityName}UpdateDto, {entityName}>();
            CreateMap<{entityName}, {entityName}Dto>();
            CreateMap<{entityName}, {entityName}DetailDto>();        
";
            // 先判断是否存在配置文件
            var path = Path.Combine(ServicePath, "AutoMapper");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            const string AppendSign = "// {AppendMappers}";
            const string AlreadySign = "// {AlreadyMapedEntity}";
            var mapperFilePath = Path.Combine(path, "AutoGenerateProfile.cs");
            string content;
            if (File.Exists(mapperFilePath))
            {
                // 如果文件存在但当前entity没有生成mapper，则替换该文件
                content = File.ReadAllText(mapperFilePath);
                if (!content.Contains($"// {entityName}"))
                {
                    content = content.Replace(AlreadySign, $"// {entityName}\r\n" + AlreadySign);
                    content = content.Replace(AppendSign, code + AppendSign);
                }
            }
            else
            {
                // 读取模板文件
                content = GetTplContent("AutoMapper.tpl");
                content = content.Replace(AppendSign, code + AppendSign);
            }
            // 写入文件
            File.WriteAllText(mapperFilePath, content, Encoding.UTF8);
            Console.WriteLine("AutoMapper 配置完成");
        }

        public class DtoInfo
        {
            public string Name { get; set; }
            public string BaseType { get; set; }
            public List<PropertyInfo> Properties { get; set; }
            public string Tag { get; set; }

            public override string ToString()
            {
                var propStrings = string.Join(string.Empty, Properties.Select(p => p.ToCsharpLine()).ToArray());
                var baseType = string.IsNullOrEmpty(BaseType) ? "" : " : " + BaseType;
                var tpl = $@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Services.Models;
using GT.Agreement.Models;
using Core.Entities;

namespace Services.Models
{{
    public class {Name}{baseType}
    {{
{propStrings}    
    }}
}}";
                return tpl;
            }
            public void Save(string dir)
            {
                var path = Path.Combine(dir, "Models", Tag);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllText(Path.Combine(path, Name + ".cs"), ToString());
            }
        }
    }
}
