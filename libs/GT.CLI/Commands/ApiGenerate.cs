using GT.CLI.Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;

namespace GT.CLI.Commands
{
    /// <summary>
    /// 控制器代码生成
    /// </summary>
    public class ApiGenerate : GenerateBase
    {
        /// <summary>
        /// 实体文件路径
        /// </summary>
        public string EntityPath { get; }
        /// <summary>
        /// service项目路径
        /// </summary>
        public string ServicePath { get; }
        /// <summary>
        /// Web站点路径
        /// </summary>
        public string WebPath { get; }

        public ApiGenerate(string entityPath, string servicePath, string webPath)
        {
            EntityPath = entityPath;
            ServicePath = servicePath;
            WebPath = webPath;
        }
        /// <summary>
        /// 生成控制器
        /// </summary>
        public void GenerateController()
        {
            // 如果已经存在，则不创建
            if (File.Exists(Path.Combine(WebPath, "Controllers", "ApiController.cs")))
            {
                return;
            }
            // 在Web项目中创建基类
            var tplContent = GetTplContent("ApiController.tpl");
            // 替换数据库上下文
            var cpl = new CompilationHelper(ServicePath, new DirectoryInfo(ServicePath).Name);
            var classes = cpl.GetAllClasses();
            var contextClass = cpl.GetClassNameByBaseType(classes, "DbContext").FirstOrDefault();
            var contextName = contextClass?.Name ?? "ContextBase";
            tplContent = tplContent.Replace("{$ContextName}", contextName);
            SaveToFile(Path.Combine(WebPath, "Controllers"), "ApiController.cs", tplContent);
            Console.WriteLine("写入控制器基类完成");

            // 在Web项目中生成相应控制器
            tplContent = GetTplContent("MyController.tpl");
            // 查找模型名称和描述
            if (!File.Exists(EntityPath))
            {
                return;
            }
            // 解析类名
            var content = File.ReadAllText(EntityPath);
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = tree.GetCompilationUnitRoot();
            var classDeclarationSyntax = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            string className = classDeclarationSyntax.Identifier.ToString();
            // 替换模板
            tplContent = tplContent.Replace("{$EntityName}", className).Replace("{$Description}", className);
            SaveToFile(Path.Combine(WebPath, "Controllers"), className + "Controller.cs", tplContent);
            Console.WriteLine("写入Api控制器完成");
        }
        /// <summary>
        /// 生成仓储的注入服务
        /// </summary>
        public void GenerateRepositoryServicesDI()
        {
            // 获取services中所有继承Repository的仓储类名
            var cpl = new CompilationHelper(ServicePath, new DirectoryInfo(ServicePath).Name);
            var classes = cpl.GetAllClasses();
            var repositories = string.Join(string.Empty, classes.Where(c => c.BaseType.Name.Equals("Repository"))
                .Select(c => "            services.AddScoped(typeof(" + c.Name + "));\r\n").ToArray());
            repositories += "        services.AddHttpContextAccessor();\r\n";
            // 替换模板文件并写入
            var tplContent = GetTplContent("RepositoryServiceExtensions.tpl");
            string replaceSign = "// {$TobeAddRepository}";
            tplContent = tplContent.Replace(replaceSign, repositories + replaceSign);
            File.WriteAllText(Path.Combine(WebPath, "RepositoryServiceExtensions.cs"), tplContent);
            Console.WriteLine("写入仓储注册服务完成");
        }
    }
}
