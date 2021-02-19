using MusicStore.Models.DbModels;

namespace MusicStore.DataAccess.Interfaces
{
    public interface ICoverTypeRepository : IGenericRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}
