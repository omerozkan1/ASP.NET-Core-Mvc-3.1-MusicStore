using System;

namespace MusicStore.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository category { get; }
        ICoverTypeRepository coverType { get; }
        IProductRepository product { get; }
        ISPCallRepository sp_call { get; }
        void Save();
    }
}
