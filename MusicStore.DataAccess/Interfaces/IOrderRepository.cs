using MusicStore.Models.DbModels;

namespace MusicStore.DataAccess.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        void Update(Order order);
    }
}
