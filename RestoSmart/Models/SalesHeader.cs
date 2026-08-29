using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("SALES_HEADER")]
public class SalesHeader
{
    [Key]
    [Column("SH_BILLID")]
    public int BillId { get; set; }
    [Column("SH_DATE")]
    public DateTime Date { get; set; }

    [Column("SH_TOTALAMOUNT")]
    public decimal TotalAmount { get; set; }

    [Column("SH_ORDERTYPE")]
    public string? OrderType { get; set; } 
    [Column("RT_TABLEID")]
    public int? TableId { get; set; } 

    [Column("U_USERID")]
    public int? UserId { get; set; } 

    [Column("SH_STATUS")]
    public string? Status { get; set; } 
}