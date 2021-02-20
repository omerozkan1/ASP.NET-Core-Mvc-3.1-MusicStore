using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }
    }
}
