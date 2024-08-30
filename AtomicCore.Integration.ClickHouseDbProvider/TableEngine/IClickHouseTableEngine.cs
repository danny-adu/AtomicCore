using AtomicCore.DbProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse Table Engine
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public interface IClickHouseTableEngine<M>
        where M : IClickHouseDbModel, new()
    {
        /// <summary>
        /// 指定更新操作
        /// </summary>
        /// <param name="whereExp"></param>
        /// <param name="updatePropertys"></param>
        /// <returns></returns>
        DbNonRecord Update(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys);
    }
}
