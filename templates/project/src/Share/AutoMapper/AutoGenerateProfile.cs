// 该文件由GT.CLI工具生成，请不要删除模板占位符。
// {AlreadyMapedEntity}
using AutoMapper;
using Core.Entity;
using Share.Models;

namespace Share.AutoMapper
{
    /// <summary>
    /// GT.CLI 生成的AutoMapper配置
    /// </summary>
    public class GenerateProfile : Profile
    {
        public GenerateProfile()
        { 
            // {AppendMappers}
        }
    }

    /// <summary>
    /// 请使用该静态类，配置到自己的mapperProfile中,如:AutoGenerateProfile.Init();
    /// </summary>
    public static class AutoGenerateProfile
    {
        public static void Init()
        {
            new GenerateProfile();
        }
    }
}
