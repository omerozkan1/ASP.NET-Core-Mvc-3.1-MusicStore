using MusicStore.Models.DbModels;

namespace MusicStore.DataAccess.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        void Update(Product product);
    }
}
