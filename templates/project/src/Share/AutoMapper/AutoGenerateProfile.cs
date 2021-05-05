// 该文件由GT.CLI工具生成，请不要删除模板占位符。
// {AlreadyMapedEntity}
using Core.Entity;
using Share.Models;
using Share.Models.Common;
using AutoMapper;
using System;

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

            bool NotNull(object src)
            {
                return src switch
                {
                    null => false,
                    int @int when @int == 0 => false,
                    Guid guid when guid == Guid.Empty => false,
                    _ => true
                };
            }
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
