﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Share.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace Core.Agreement
{
    /// <summary>
    /// 基础curd操作实现
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TAddForm"></typeparam>
    /// <typeparam name="TUpdatForm"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    /// <typeparam name="TData"></typeparam> 
    /// <typeparam name="TKey"></typeparam>
    public class RepositoryBase<TContext, TEntity, TAddForm, TUpdatForm, TFilter, TData, TKey>
        : IQueryable<TEntity>,
        IRepositoryBase<TEntity, TAddForm, TUpdatForm, TFilter, TData, TKey>
        where TContext : DbContext
        where TEntity : class
        where TFilter : class
    {
        public TContext _context;
        public DbSet<TEntity> _db;
        protected IMapper _mapper;
        protected IQueryable<TEntity> _query;

        public RepositoryBase(TContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _db = _context.Set<TEntity>();
            _query = _db.AsQueryable();
        }

        public Type ElementType => _query.ElementType;

        public Expression Expression => _query.Expression;

        public IQueryProvider Provider => _query.Provider;

        /// <summary>
        /// 默认添加
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> AddAsync(TAddForm form)
        {
            var data = _mapper.Map<TAddForm, TEntity>(form);
            _db.Add(data);
            await _context.SaveChangesAsync();
            return data;
        }

        /// <summary>
        /// 删除，有关联的重写该方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> DeleteAsync(TKey id)
        {
            var data = await _db.FindAsync(id);
            _db.Remove(data);
            await _context.SaveChangesAsync();
            return data;
        }

        /// <summary>
        /// 默认以id判断
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> Exist(object o)
        {
            var pi = o.GetType().GetProperty("Id");
            var id = (Guid)(pi.GetValue(o, null));
            var exist = await _db.FindAsync(id);
            return exist;
        }

        public virtual async Task<TEntity> GetDetailAsync(TKey id)
        {
            var data = await _db.FindAsync(id);
            return data;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _query.GetEnumerator();
        }

        public virtual Task<List<TData>> GetListAsync(TFilter filter)
        {
            throw new NotImplementedException();
        }

        public virtual Task<PageResult<TData>> GetListWithPageAsync(TFilter filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 仅更新实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(TKey id, TUpdatForm form)
        {
            var currentData = await _db.FindAsync(id);
            _context.Entry(currentData).CurrentValues.SetValues(form);
            await _context.SaveChangesAsync();
            return currentData;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_query).GetEnumerator();
        }
    }
}
