using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;

namespace MusicStore.DataAccess.Repositories
{
    public class AppUserRepository : GenericRepository<AppUser>, IAppUserRepository
    {
        private readonly ApplicationDbContext db;
        public AppUserRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }
    }
}
