using System;

namespace GT.Agreement.Entity
{
    /// <summary>
    /// 最小实体
    /// </summary>
    internal interface IMiniEntity<T>
    {
        T Id { get; set; }
        DateTimeOffset CreatedTime { get; set; }
    }
}
