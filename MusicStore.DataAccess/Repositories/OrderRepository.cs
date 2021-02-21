using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;

namespace MusicStore.DataAccess.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext db;
        public OrderRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(Order order)
        {
            db.Update(order);
        }
    }
}
