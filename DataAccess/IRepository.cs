using Portfolio.DataAccess;
using System;

namespace DataAccess
{
    public interface IRepository<TRepository>
    {
        
        Task<long> addNew(TRepository Person);
        Task<bool> updateById(TRepository Person);
        Task<bool> deleteById(long ID);
        Task<TRepository> getById(long ID);
        Task<List<TRepository>> getAll();
    }
}
