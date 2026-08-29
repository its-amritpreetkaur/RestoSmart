using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSmart.Models
{
    [Table("INVENTORY")] 
    public class Inventory
    {
        [Key]
        [Column("I_INGREDIENTID")]
        public int Id { get; set; }

        [Column("I_NAME")]
        public string Name { get; set; }

        [Column("I_STOCKQTY")]
        public decimal StockQty { get; set; }

        [Column("I_UNIT")]
        public string Unit { get; set; }

        [Column("I_COST")]
        public decimal Cost { get; set; }
    }
}