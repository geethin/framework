using GT.Agreement.Entity;
using System;
using System.ComponentModel.DataAnnotations;

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
