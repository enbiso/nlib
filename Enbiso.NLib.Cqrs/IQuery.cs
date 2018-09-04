using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enbiso.NLib.Cqrs
{
    public interface IQuery<in TKey, in TSearch, TQueryModel, TSummeryQueryModel>
        where TQueryModel : IQueryModel
        where TSummeryQueryModel : ISummaryQueryModel
    {
        Task<TQueryModel> GetByIdAsync(TKey key);
        Task<IEnumerable<TSummeryQueryModel>> ListAsync(TSearch search);
    }

    public interface IQuery<in TKey, in TSearch> : IQuery<TKey, TSearch, IQueryModel, ISummaryQueryModel>
    {
    }

    public interface IQueryModel
    {
    }

    public interface ISummaryQueryModel
    {
    }
}
