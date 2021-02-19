using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;
using System.Linq;

namespace MusicStore.DataAccess.Repositories
{
    public class CoverTypeRepository : GenericRepository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext db;
        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(CoverType coverType)
        {
            var data = db.CoverTypes.FirstOrDefault(x => x.Id == coverType.Id);
            if (data != null)
                data.Name = coverType.Name;
        }
    }
}
