using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;
using System.Linq;

namespace MusicStore.DataAccess.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;
        public CompanyRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(Company company)
        {
            db.Update(company);
        }
    }
}
