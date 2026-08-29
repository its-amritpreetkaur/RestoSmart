namespace RestoSmart.Models
{
    public class OrderDashboardViewModel
    {
        public int SH_BillID { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public int TableId { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public string DishName { get; set; }
        public int Qty { get; set; }
    }
}