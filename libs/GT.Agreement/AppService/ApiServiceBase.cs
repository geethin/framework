using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GT.Agreement.AppService
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

    }
}
