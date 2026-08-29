using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSmart.Models
{
    [Table("SALES_DETAILS")]
    public class SalesDetail
    {
        [Key]
        [Column("SD_DETAILID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int DetailId { get; set; }

        [Column("SH_BILLID")]
        public int SH_BillID { get; set; }

        [Column("MI_DISHID")]
        public int DishId { get; set; }

        [Column("SD_QTYSOLD")]
        public int QtySold { get; set; }
    }
}