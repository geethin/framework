using Microsoft.AspNetCore.Mvc;

namespace App.Agreement
{
    /// <summary>
    /// api 服务基础类
    /// </summary>
    public class ApiServiceBase : ControllerBase
    {
        // TODO,处理管道,缓存 ,用户信息等
        public ApiServiceBase()
        {
        }

        public override ObjectResult Problem(string detail = null, string instance = null, int? statusCode = null, string title = null, string type = null)
        {
            return base.Problem(detail, instance, 500, title ?? "业务逻辑错误", "error");
        }
    }
}
