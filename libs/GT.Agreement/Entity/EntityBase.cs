using System;

namespace GT.Agreement.Entity
{
    public class EntityBase<T> : IEntity<T>
    {
        public virtual DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.Now;
        public virtual Status Status { get; set; } = Status.Default;
        public virtual T Id { get; set; }
        public virtual DateTimeOffset CreatedTime { get; set; }
    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 默认值 
        /// </summary>
        Default,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted,
        /// <summary>
        /// 无效
        /// </summary>
        Invalid,
        Valid
    }
}
