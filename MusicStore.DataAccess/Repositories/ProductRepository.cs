using MusicStore.DataAccess.Interfaces;
using MusicStore.Models.DbModels;
using MusicStore.Web.Data;
using System.Linq;

namespace MusicStore.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(Product product)
        {
            var data = db.Products.FirstOrDefault(x => x.Id == product.Id);
            if (data != null)
            {
                if (product.ImageUrl != null)
                {
                    data.ImageUrl = product.ImageUrl;
                }
                data.ISBN = product.ISBN;
                data.Price = product.Price;
                data.Price50 = product.Price50;
                data.Price100 = product.Price100;
                data.ListPrice = product.ListPrice;
                data.Title = product.Title;
                data.Description = product.Description;
                data.CategoryId = product.CategoryId;
                data.CoverTypeId = product.CoverTypeId;
                data.Author = product.Author;
            }
        }
    }
}
