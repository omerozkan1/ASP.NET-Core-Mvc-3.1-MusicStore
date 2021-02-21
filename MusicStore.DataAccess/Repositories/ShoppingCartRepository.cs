using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;

namespace MusicStore.DataAccess.Repositories
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext db;
        public ShoppingCartRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            db.Update(shoppingCart);
        }
    }
}
