using MusicStore.Models.DbModels;

namespace MusicStore.DataAccess.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        void Update(Company company);
    }
}
