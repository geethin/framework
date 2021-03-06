﻿using Core.Agreement;
using Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Agreement
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiControllerBase<TContext, TRepository, TEntity, TAddForm, TUpdateForm, TFilter, TDto>
        : ApiServiceBase, IApiControllerBase<TEntity, TAddForm, TUpdateForm, TFilter, TDto, Guid>
        where TRepository : RepositoryBase<TContext, TEntity, TAddForm, TUpdateForm, TFilter, TDto, Guid>
        where TContext : DbContext
        where TFilter : FilterBase
        where TEntity : BaseDB
    {
        protected ILogger _logger;
        protected TRepository _repos;

        public ApiControllerBase(ILogger logger, TRepository repos)
        {
            _repos = repos;
            _logger = logger;
        }

        [HttpPost]
        public virtual async Task<ActionResult<TEntity>> AddAsync(TAddForm form)
        {
            return await _repos.AddAsync(form);
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TEntity>> DeleteAsync([FromRoute] Guid id)
        {
            if (_repos.Any(d => d.Id == id))
            {
                return await _repos.DeleteAsync(id);
            }
            return NotFound();
        }

        [HttpPost("filter")]
        public virtual async Task<ActionResult<PageResult<TDto>>> FilterAsync(TFilter filter)
        {
            return await _repos.GetListWithPageAsync(filter);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntity>> GetDetailAsync([FromRoute] Guid id)
        {
            return await _repos.GetDetailAsync(id);
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TEntity>> UpdateAsync([FromRoute] Guid id, TUpdateForm form)
        {
            if (_repos.Any(d => d.Id == id))
            {
                return await _repos.UpdateAsync(id, form);
            }
            return NotFound();
        }
    }
}
