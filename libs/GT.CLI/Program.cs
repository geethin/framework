using System;
using System.CommandLine;
using System.CommandLine.Invocation;
namespace GT.CLI
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            //var cmdTet = new RootCommands();
            //cmdTet.GenerateAsync(@"../../../../../src/Core/Entities/MemberCard.cs", @"../../../../../src/services", @"../../../../../src/Web","../../../../../clients/admin-front").Wait();
            //return 0;
            var gtCommand = new RootCommand("geethin commands")
            {
                Name = "gt"
            };
            // angular 生成命令
            var ngCommand = new Command("angular", "angular代码生成");
            ngCommand.AddAlias("ng");
            ngCommand.AddOption(new Option<string>(new[] { "--url", "-u" })
            {
                IsRequired = true,
                Description = "swagger url"
            });
            ngCommand.AddOption(new Option<string>(new[] { "--output", "-o" })
            {
                Description = "前端目录路径,默认:../angular"
            });

            // view生成命令
            var viewCommand = new Command("view", "view代码生成，只支持ng项目生成");
            viewCommand.AddAlias("view");
            viewCommand.AddOption(new Option<string>(new[] { "--name", "-n" })
            {
                IsRequired = true,
                Description = "实体类名称"
            });
            viewCommand.AddOption(new Option<string>(new[] { "--service", "-s" })
            {
                IsRequired = true,
                Description = "service项目路径"
            });
            viewCommand.AddOption(new Option<string>(new[] { "--output", "-o" })
            {
                IsRequired = true,
                Description = "前端目录路径,默认:../angular"
            });

            // api 生成命令
            var apiCommand = new Command("webapi", "aspnetcore webapi代码生成，模型，仓储");
            apiCommand.AddAlias("api");
            apiCommand.AddOption(new Option<string>(new[] { "--entity", "-e" })
            {
                IsRequired = true,
                Description = "实体模型文件路径"
            });
            apiCommand.AddOption(new Option<string>(new[] { "--service", "-s" })
            {
                Description = "数据仓储服务的项目路径，默认为./Services",
            });
            apiCommand.AddOption(new Option<string>(new[] { "--web", "-w" })
            {
                Description = "网站项目路径，默认为./Web"
            });
            // 集成命令
            var genCommand = new Command("generate", "代码生成");
            genCommand.AddAlias("g");
            genCommand.AddOption(new Option<string>(new[] { "--entity", "-e" })
            {
                IsRequired = true,
                Description = "实体模型文件路径"
            });
            genCommand.AddOption(new Option<string>(new[] { "--service", "-s" })
            {
                IsRequired = true,
                Description = "数据仓储服务的项目路径",
            });
            genCommand.AddOption(new Option<string>(new[] { "--web", "-w" })
            {
                IsRequired = true,
                Description = "网站项目路径"
            });
            genCommand.AddOption(new Option<string>(new[] { "--output", "-o" })
            {
                IsRequired = true,
                Description = "前端项目根目录"
            });

            // 执行方法
            var cmd = new RootCommands();
            ngCommand.Handler = CommandHandler.Create<string, string>(
                 async (url, output) =>
                {
                    System.Console.WriteLine(url + output);
                    await cmd.GenerateNgAsync(url, output);
                });

            apiCommand.Handler = CommandHandler.Create<string, string, string>(
                 (path, service, web) =>
                 {
                     //System.Console.WriteLine(path, service, web);
                     cmd.GenerateApi(path, service, web);
                 });
            viewCommand.Handler = CommandHandler.Create<string, string, string>(
                (name, servicePath, output) =>
                {
                    //System.Console.WriteLine(path, service, web);
                    cmd.GenerateNgPages(name, servicePath, output);
                });

            genCommand.Handler = CommandHandler.Create<string, string, string, string>(
              async (entity, service, web, output) =>
               {
                   await cmd.GenerateAsync(entity, service, web, output);
               });

            gtCommand.Add(ngCommand);
            gtCommand.Add(apiCommand);
            gtCommand.Add(viewCommand);
            gtCommand.Add(genCommand);

            return gtCommand.InvokeAsync(args).Result;
        }
    }
}
