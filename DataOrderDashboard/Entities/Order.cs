namespace DataOrderDashboard.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public byte Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; }
        public string OrderNotes { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
