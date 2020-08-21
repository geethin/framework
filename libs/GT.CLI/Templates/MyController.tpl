using Core.Entities;
using GT.Agreement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using Services;
using Services.Models;
using Services.Models.Shared;
using Services.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    /// <summary>
    /// {$Description}
    /// </summary>
    [OpenApiTag("{$EntityName}", Description = "{$Description}")]
    public class {$EntityName}Controller : ApiController<{$EntityName}Repository, {$EntityName}, {$EntityName}AddDto, {$EntityName}UpdateDto, {$EntityName}Filter, {$EntityName}Dto>
    {
        public {$EntityName}Controller(
            ILogger<{$EntityName}Controller> logger,
            {$EntityName}Repository repository) : base(logger, repository)
        {
        }

        /// <summary>
        /// 添加{$Description}
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<ActionResult<{$EntityName}>> AddAsync([FromBody] {$EntityName}AddDto form)
        {
            // if (_repos.Any(e => e.Name == form.Name))
            // {
            //     return Conflict();
            // }
            var design = await _repos.AddAsync(form);
            return design;
        }

        [HttpPost("filter")]
        public override async Task<ActionResult<PageResult<{$EntityName}Dto>>> FilterAsync({$EntityName}Filter filter)
        {
            return await _repos.GetListWithPageAsync(filter);
        }

        [HttpPut("{id}")]
        public override async Task<ActionResult<{$EntityName}>> UpdateAsync([FromRoute] Guid id, [FromBody] {$EntityName}UpdateDto form)
        {
            if (_repos.Any(e => e.Id == id))
            {
                // 名称不可以修改成其他已经存在的名称
                // if (_repos.Any(e => e.Name == form.Name && s;e.Id != id))
                // {
                //    return Conflict();
                // }
                return await _repos.UpdateAsync(id, form);
            }
            return NotFound();
        }
    }
}