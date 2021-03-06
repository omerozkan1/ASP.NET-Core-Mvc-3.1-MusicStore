using MusicStore.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
