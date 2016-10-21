using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace InspurOA.BLL
{
    public static class Extensions
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">数据源</param>
        /// <param name="KeySelector">The key selector.</param>
        /// <param name="count">记录总数：总共有多少条记录</param>
        /// <param name="offset">偏移量：当前查询的页码</param>
        /// <param name="limit">查询记录数:默认为10条</param>
        /// <returns></returns>
        public static IQueryable<T> QueryByPage<T>(this IQueryable<T> list, Expression<Func<T, string>> KeySelector, out int totalCount, out int pageCount, int offset, int limit = 10)
        {
            totalCount = 0;
            pageCount = 0;
            if (offset < 0)
            {
                throw new ArgumentException("参数不能小于0", "offset");
            }

            if (limit <= 0)
            {
                throw new ArgumentException("参数必须大于0", "limit");
            }

            totalCount = list.Count();
            pageCount = totalCount / limit;
            if (totalCount % limit > 0)
            {
                pageCount++;
            }

            var result = list.OrderBy(KeySelector).Skip(offset * limit).Take(limit);
            return result;
        }

        public static IQueryable<T> QueryByPage<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> whereSelector, Expression<Func<T, string>> KeySelector, out int totalCount, out int pageCount, int offset, int limit = 10) where T : class
        {
            totalCount = 0;
            pageCount = 0;
            if (offset < 0)
            {
                throw new ArgumentException("参数不能小于0", "offset");
            }

            if (limit <= 0)
            {
                throw new ArgumentException("参数必须大于0", "limit");
            }

            if (whereSelector != null)
            {
                return dbSet.Where(whereSelector).QueryByPage(KeySelector, out totalCount, out pageCount, offset, limit);
            }
            else
            {
                return dbSet.Where(t => true).QueryByPage(KeySelector, out totalCount, out pageCount, offset, limit);
            }
        }
    }
}