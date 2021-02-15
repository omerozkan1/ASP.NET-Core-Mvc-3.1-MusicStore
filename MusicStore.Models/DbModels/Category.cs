using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models.DbModels
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori Adı alanı boş geçilemez.")]
        [StringLength(250,MinimumLength = 3, ErrorMessage = "Lütfen girdiğiniz değeri kontrol ediniz.")]
        public string CategoryName { get; set; }
    }
}
