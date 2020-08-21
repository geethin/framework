﻿using GT.CLI.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Readers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GT.CLI
{
    public class RootCommands
    {
        public RootCommands()
        {
        }

        /// <summary>
        /// angular 代码生成
        /// </summary>
        /// <param name="url">swagger json地址</param>
        /// <param name="output">ng前端根目录</param>
        /// <returns></returns>
        public async Task GenerateNgAsync(string url = "", string output = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                url = "http://localhost:5000/swagger/v1/swagger.json";
            }
            try
            {
                using var web = new WebClient();
                var result = web.DownloadString(url);
                var openApiDoc = new OpenApiStringReader().Read(result, out var context);
                // 所有类型
                var schemas = openApiDoc.Components.Schemas;
                var tsGen = new TypescriptGenerate(schemas);
                await tsGen.BuildInterfaceAsync(output);

                // 请求服务构建
                var operations = openApiDoc.Paths.Values;
                var serviceGen = new NgServiceGenerate(openApiDoc.Paths);
                serviceGen.CopyBaseService(output);
                await serviceGen.BuildServiceAsync(openApiDoc.Tags, output);

                Console.WriteLine("ng请求服务生成完成");
            }
            catch (WebException webExp)
            {
                Console.WriteLine(webExp.Message);
                Console.WriteLine("请确定您的后台开启了swagger，并输入了正确的地址!");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.WriteLine(exp.StackTrace);
            }
        }

        /// <summary>
        /// api项目代码生成
        /// </summary>
        /// <param name="path">实体文件路径</param>
        /// <param name="servicePath">service目录</param>
        /// <param name="webPath">网站目录</param>
        public void GenerateApi(string path, string servicePath = "", string webPath = "")
        {
            var reposGen = new RepositoryGenerate(path, servicePath);
            reposGen.GenerateDtos();
            reposGen.GenerateReponsitory();

            if (!string.IsNullOrEmpty(webPath))
            {
                var apiGen = new ApiGenerate(path, servicePath, webPath);
                apiGen.GenerateRepositoryServicesDI();
                apiGen.GenerateController();
            }
        }

        /// <summary>
        /// 根据已生成的dto生成相应的前端表单页面
        /// </summary>
        /// <param name="servicePath">service根目录</param>
        /// <param name="name">实体类名称</param>
        /// <param name="output">前端根目录</param>
        public void GenerateNgPages(string name, string servicePath, string output = "")
        {
            var pageGen = new NgPageGenerate(name, servicePath, output);
            pageGen.Build();
            Console.WriteLine("前端页面生成完成");
        }

        public async Task GenerateAsync(string entityFile, string servicePath, string webPath, string output)
        {
            Console.WriteLine("生成后台Api代码");
            GenerateApi(entityFile, servicePath, webPath);

            Console.WriteLine("请输入swagger json在地址,按回车确认");
            var url = Console.ReadLine();
            Console.WriteLine("生成angular客户端请求服务");
            await GenerateNgAsync(url, output);
            Console.WriteLine("生成angular在基础表单页面");
            var fileName = Path.GetFileNameWithoutExtension(entityFile);
            GenerateNgPages(fileName, servicePath, output);

            Console.WriteLine("全部执行完成，请在web项目中注入仓储服务 services.AddRepositories();");
        }
    }

}


