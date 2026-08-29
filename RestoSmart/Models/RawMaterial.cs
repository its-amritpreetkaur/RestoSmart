using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSmart.Models
{
    [Table("RAW_MATERIALS")]
    public class RawMaterial
    {
        [Key]
        [Column("RM_ID")]
        public int Id { get; set; }

        [Column("RM_NAME")]
        public string Name { get; set; }

        [Column("RM_UNIT")]
        public string Unit { get; set; }

        [Column("RM_CURRENTSTOCK")]
        public decimal CurrentStock { get; set; }

        [Column("RM_MINREORDER")]
        public decimal MinReorder { get; set; }
    }
}

