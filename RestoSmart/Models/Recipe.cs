using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSmart.Models
{
    [Table("RECIPES")]
    public class Recipe
    {
        [Key]
        [Column("R_RECIPEID")] 
        public int RecipeId { get; set; }

        [Column("MI_DISHID")]
        public int DishId { get; set; }

        [Column("I_INGREDIENTID")] 
        public int RawMaterialId { get; set; }

        [Column("R_QTYREQUIRED")] 
        public decimal QuantityRequired { get; set; }
    }
}