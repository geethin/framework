using Core.Entity;
using Share.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services;
using Core.Services.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using App.Agreement;

namespace App.Api.Controllers
{
    /// <summary>
    /// Blog
    /// </summary>
    public class BlogController : ApiController<BlogRepository, Blog, BlogAddDto, BlogUpdateDto, BlogFilter, BlogDto>
    {
        public BlogController(
            ILogger<BlogController> logger,
            BlogRepository repository) : base(logger, repository)
        {
        }

        /// <summary>
        /// 添加Blog
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<ActionResult<Blog>> AddAsync([FromBody] BlogAddDto form)
        {
            // if (_repos.Any(e => e.Name == form.Name))
            // {
            //     return Conflict();
            // }
            return await _repos.AddAsync(form);
        }

        /// <summary>
        /// 分页筛选Blog
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("filter")]
        public override async Task<ActionResult<PageResult<BlogDto>>> FilterAsync(BlogFilter filter)
        {
            return await _repos.GetListWithPageAsync(filter);
        }

        /// <summary>
        /// 更新Blog
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public override async Task<ActionResult<Blog>> UpdateAsync([FromRoute] Guid id, [FromBody] BlogUpdateDto form)
        {
            if (_repos.Any(e => e.Id == id))
            {
                // 名称不可以修改成其他已经存在的名称
                // if (_repos.Any(e => e.Name == form.Name && e.Id != id))
                // {
                //    return Conflict();
                // }
                return await _repos.UpdateAsync(id, form);
            }
            return NotFound();
        }
    }
}