using System;
using System.ComponentModel.DataAnnotations;
using GT.Agreement.Entity;

namespace Core.Entity
{
    // 定义你的实体模型基本字段
    public class BaseDB : EntityBase<Guid>
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        public override DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    }
}
