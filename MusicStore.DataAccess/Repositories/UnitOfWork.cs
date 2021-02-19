using MusicStore.DataAccess.Interfaces;
using MusicStore.Web.Data;

namespace MusicStore.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;
        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            category = new CategoryRepository(db);
            coverType = new CoverTypeRepository(db);
            sp_call = new SPCallRepository(db);
        }
        public ICategoryRepository category { get; private set; }
        public ICoverTypeRepository coverType { get; private set; }
        public ISPCallRepository sp_call { get; private set; }

        public void Dispose()
        {
            db.Dispose();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
