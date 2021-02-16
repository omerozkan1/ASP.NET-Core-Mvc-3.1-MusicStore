using MusicStore.Models.DbModels;

namespace MusicStore.DataAccess.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        void Update(Category category);
    }
}
