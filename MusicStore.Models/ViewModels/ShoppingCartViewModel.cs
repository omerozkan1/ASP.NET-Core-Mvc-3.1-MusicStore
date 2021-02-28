using MusicStore.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> CartList { get; set; }
        public Order Order { get; set; }
    }
}
