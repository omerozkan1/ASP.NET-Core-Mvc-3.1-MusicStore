using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;

namespace MusicStore.DataAccess.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext db;
        public OrderDetailRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            db.Update(orderDetail);
        }
    }
}
