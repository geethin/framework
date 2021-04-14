using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Share.Models;
using Core.Entity;
namespace Share.Models
{
    public class BlogDetailDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        [Key]
[Column(TypeName = "char(36)")]
        public Guid Id { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedTime { get; set; }
    
    }
}