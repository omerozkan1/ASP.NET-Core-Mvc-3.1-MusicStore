using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository category { get; }
        ISPCallRepository sp_call { get; }
        void Save();
    }
}
