namespace EscCase.Business.Models.Requests
{
    public class ProductUpdateRequest
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public Guid Id { get; set; }
    }
}
