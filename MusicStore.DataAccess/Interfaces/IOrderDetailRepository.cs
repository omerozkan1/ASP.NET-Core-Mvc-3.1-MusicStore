using MusicStore.Models.DbModels;

namespace MusicStore.DataAccess.Interfaces
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
