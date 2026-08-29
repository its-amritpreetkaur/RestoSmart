using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSmart.Models
{
    [Table("MENU_ITEMS")]
    public class MenuItem
    {
        [Key]
        [Column("MI_DISHID")]
        public int DishId { get; set; }

        [Required]
        [Column("MI_NAME")]
        public string Name { get; set; }

        [Column("MI_PRICE")]
        public decimal Price { get; set; }

        [Required]
        [Column("MI_CATEGORY")]
        public string Category { get; set; }

        [Column("MI_IMAGEURL")]
        public string? ImageUrl { get; set; }
    }
}
