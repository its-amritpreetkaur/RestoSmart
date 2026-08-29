using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSmart.Models
{
    [Table("RESTAURANT_TABLES")]
    public class RestaurantTable
    {
        [Key]
        [Column("RT_TABLEID")]
        public int TableId { get; set; }

        [Column("RT_NAME")]
        public string Name { get; set; }

        [Column("RT_STATUS")]
        public string Status { get; set; } 
    }
}