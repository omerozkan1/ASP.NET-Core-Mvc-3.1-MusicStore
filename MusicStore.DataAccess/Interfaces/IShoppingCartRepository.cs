using MusicStore.Models.DbModels;

namespace MusicStore.DataAccess.Interfaces
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
    }
}
