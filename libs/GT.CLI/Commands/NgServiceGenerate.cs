﻿using GT.Agreement.Utils;
using GT.CLI.Common;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace GT.CLI.Commands
{
    /// <summary>
    /// angular request service
    /// </summary>
    public class NgServiceGenerate : GenerateBase
    {
        protected OpenApiPaths PathsPairs { get; }

        public NgServiceGenerate(OpenApiPaths paths)
        {
            PathsPairs = paths;
        }

        public void CopyBaseService(string ngRootPath = "")
        {
            string content = GetTplContent("base.service.tpl");
            if (string.IsNullOrEmpty(ngRootPath))
            {
                var parentPath = new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName;
                ngRootPath = Path.Combine(parentPath, "angluar");
            }
            var servicePath = Path.Combine(ngRootPath, "src", "app", "services");
            if (!Directory.Exists(servicePath))
            {
                Directory.CreateDirectory(servicePath);
            }
            var dstPath = Path.Combine(servicePath, "base.service.ts");
            File.WriteAllText(dstPath, content);

            Console.WriteLine("写入基础服务完成");
        }

        public async Task BuildServiceAsync(IList<OpenApiTag> tags, string ngRootPath = "")
        {
            if (string.IsNullOrEmpty(ngRootPath))
            {
                var parentPath = new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName;
                ngRootPath = Path.Combine(parentPath, "angluar");
            }
            // 生成文件的目录名称
            var servicesPath = Path.Combine(ngRootPath, "src", "app", "services");
            if (!Directory.Exists(servicesPath))
            {
                Directory.CreateDirectory(servicesPath);
            }

            var functions = new List<NgServiceFunction>();
            // 处理所有方法
            foreach (var path in PathsPairs)
            {
                foreach (var operation in path.Value.Operations)
                {
                    var function = new NgServiceFunction
                    {
                        Description = operation.Value.Summary,
                        Method = operation.Key.ToString(),
                        Name = operation.Value.OperationId,
                        Path = path.Key,
                        Tag = operation.Value.Tags.FirstOrDefault().Name,
                    };
                    (function.RequestType, function.RequestRefType) = GetParamType(operation.Value.RequestBody?.Content?.Values.FirstOrDefault()?.Schema);
                    (function.ResponseType, function.ResponseRefType) = GetParamType(operation.Value.Responses?.FirstOrDefault().Value
                        ?.Content.FirstOrDefault().Value
                        ?.Schema);
                    function.Params = operation.Value.Parameters?.Select(p =>
                        {
                            var location = p.In?.GetDisplayName();
                            var inpath = location?.ToLower()?.Equals("path");
                            var (type, _) = GetParamType(p.Schema);
                            return new FunctionParams
                            {
                                Description = p.Description,
                                Name = p.Name,
                                InPath = inpath ?? false,
                                IsRequired = p.Required,
                                Type = type
                            };
                        }).ToList();
                    functions.Add(function);
                }
            }
            // 生成文件
            var ngServices = new List<NgServiceFile>();
            // 先以tag分组
            var funcGroups = functions.GroupBy(f => f.Tag).ToList();
            foreach (var group in funcGroups)
            {
                // 查询该标签包含的所有方法
                var tagFunctions = group.ToList();
                var currentTag = tags.Where(t => t.Name == group.Key).FirstOrDefault();
                if (currentTag == null)
                {
                    currentTag = new OpenApiTag { Name = group.Key, Description = group.Key };
                }
                var ngServiceFile = new NgServiceFile
                {
                    Description = currentTag.Description,
                    Name = currentTag.Name,
                    Functions = tagFunctions
                };
                var content = ngServiceFile.ToString();
                var fileName = currentTag.Name.ToHyphen() + ".service.ts";
                await File.WriteAllTextAsync(Path.Combine(servicesPath, fileName), content);
                Console.WriteLine(fileName + "生成完成");
            }
        }
        private (string type, string refType) GetParamType(OpenApiSchema schema)
        {
            if (schema == null)
            {
                return (string.Empty, string.Empty);
            }

            string type = "any";
            string refType = string.Empty;
            if (schema.Reference != null)
            {
                return (schema.Reference.Id, schema.Reference.Id);
            }
            // 常规类型
            switch (schema.Type)
            {
                case "boolean":
                    type = "boolean";
                    break;
                case "integer":
                    // 看是否为enum
                    if (schema.Enum.Count > 0)
                    {
                        if (schema.Reference != null)
                        {
                            type = schema.Reference.Id;
                            refType = schema.Reference.Id;
                        }
                    }
                    else
                    {
                        type = "number";
                    }
                    break;
                case "file":
                    type = "FormData";
                    break;
                case "string":
                    switch (schema.Format)
                    {
                        case "binary":
                            type = "FormData";
                            break;
                        case "date-time":
                            type = "Date";
                            break;
                        default:
                            type = "string";
                            break;
                    }
                    break;

                case "array":

                    if (schema.Items.Reference != null)
                    {
                        type = schema.Items.Reference.Id + "[]";
                        refType = schema.Items.Reference.Id;
                    }
                    else
                    {
                        // 基础类型?
                        type = schema.Items.Type + "[]";
                    }
                    break;
                default:
                    break;
            }
            // 引用对象
            if (schema.OneOf.Count > 0)
            {
                // 获取引用对象名称
                type = schema.OneOf.First()?.Reference.Id;
                refType = schema.OneOf.First()?.Reference.Id;
            }
            return (type, refType);
        }
    }

    /// <summary>
    /// 服务文件
    /// </summary>
    public class NgServiceFile
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<NgServiceFunction> Functions { get; set; }

        public override string ToString()
        {
            var functions = string.Join("\n", Functions.Select(f => f.ToString()).ToArray());
            // import引用的models
            var importModels = "";
            var refTypes = new List<string>();
            var refs1 = Functions.Where(f => !string.IsNullOrEmpty(f.RequestRefType)).Select(f => f.RequestRefType).ToList();
            var refs2 = Functions.Where(f => !string.IsNullOrEmpty(f.ResponseRefType)).Select(f => f.ResponseRefType).ToList();
            if (refs1 != null)
            {
                refTypes.AddRange(refs1);
            }

            if (refs2 != null)
            {
                refTypes.AddRange(refs2);
            }

            refTypes = refTypes.GroupBy(t => t).Select(g => g.FirstOrDefault()).ToList();
            refTypes.ForEach(t =>
            {
                importModels += $"import {{ {t} }} from '../share/models/{t.ToHyphen()}.model';\n";
            });
            string result = $@"import {{ Injectable }} from '@angular/core';
import {{ BaseService }} from './base.service';
import {{ Observable }} from 'rxjs';
{importModels}
/**
 * {Description}
 */
@Injectable({{ providedIn: 'root' }})
export class {Name}Service extends BaseService {{

{functions}


}}";
            return result;
        }
    }

    /// <summary>
    /// 请求服务的函数
    /// </summary>
    public class NgServiceFunction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public string ResponseType { get; set; }
        /// <summary>
        /// 返回中的引用类型
        /// </summary>
        public string ResponseRefType { get; set; }
        public string RequestType { get; set; } = string.Empty;
        /// <summary>
        /// 请求中的引用类型
        /// </summary>
        public string RequestRefType { get; set; }
        /// <summary>
        /// 参数及类型
        /// </summary>
        public List<FunctionParams> Params { get; set; }
        /// <summary>
        /// 相对请求路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        public override string ToString()
        {
            // 函数名处理，去除tag前缀，然后格式化
            Name = Name.Replace(Tag + "_", "");
            Name = Name.ToCamelCase();
            // 处理参数
            var paramsString = "";
            var paramsComments = "";
            var dataString = "";
            if (Params?.Count > 0)
            {
                paramsString = string.Join(", ",
                    Params.Select(p => p.Name + (p.IsRequired ? ": " : "?: ") + p.Type).ToArray());
                Params.ForEach(p =>
                {
                    paramsComments += $"   * @param {p.Name} {p.Description ?? p.Type}\n";
                });
            }
            if (!string.IsNullOrEmpty(RequestType))
            {
                if (Params?.Count > 0)
                {
                    paramsString += ", ";
                }

                paramsString += $"data: {RequestType}";
                dataString = ", data";
                paramsComments += $"   * @param data {RequestType}\n";
            }
            // 注释生成
            var comments = $@"  /**
   * {Description ?? Name}
{paramsComments}   */";

            // 构造请求url
            var paths = Params.Where(p => p.InPath).Select(p => p.Name)?.ToList();
            paths.ForEach(p =>
            {
                var origin = $"{{{p}}}";
                Path = Path.Replace(origin, "$" + origin);
            });
            // 需要拼接的参数,特殊处理文件上传
            var reqParams = Params.Where(p => !p.InPath && p.Type != "FormData")
                .Select(p => p.Name)?.ToList();
            if (reqParams != null)
            {
                var queryParams = "";
                queryParams = string.Join("&", reqParams.Select(p => { return $"{p}=${{{p}}}"; }).ToArray());
                if (!string.IsNullOrEmpty(queryParams))
                {
                    Path += "?" + queryParams;
                }
            }
            var file = Params.Where(p => p.Type.Equals("FormData")).FirstOrDefault();
            if (file != null)
            {
                dataString = $", {file.Name}";
            }

            var function = @$"{comments}
  {Name}({paramsString}): Observable<{ResponseType}> {{
    const url = `{Path}`;
    return this.request<{ResponseType}>('{Method.ToLower()}', url{dataString});
  }}
";
            return function;
        }
    }

    /// <summary>
    /// 函数参数
    /// </summary>
    public class FunctionParams
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; } = true;
        /// <summary>
        /// 是否路由参数
        /// </summary>
        public bool InPath { get; set; } = false;
    }
}
