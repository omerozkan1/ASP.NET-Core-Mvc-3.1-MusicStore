using System;

namespace MusicStore.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IAppUserRepository AppUser { get; }
        ISPCallRepository sp_call { get; }
        void Save();
    }
}
