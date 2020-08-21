using GT.Agreement.Models;
using System;
using System.Threading.Tasks;

namespace GT.Agreement.AppService
{
    /// <summary>
    /// 定义基本restful方法
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TAddForm"></typeparam>
    /// <typeparam name="TUpdateForm"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="Tkey"></typeparam>
    public interface IApiServiceBase<TEntity, TAddForm, TUpdateForm, TFilter, TDto, Tkey>
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns></returns>
        Task<TEntity> AddAsync(TAddForm form);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> DeleteAsync(Tkey id);
        /// <summary>
        /// 分页筛选
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<PageResult<TDto>> FilterAsync(TFilter filter);
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetDetailAsync(Tkey id);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(Tkey id, TUpdateForm form);
    }
}