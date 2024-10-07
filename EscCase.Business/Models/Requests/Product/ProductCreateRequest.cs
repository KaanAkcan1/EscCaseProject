namespace EscCase.Business.Models.Requests.Product
{
    public class ProductCreateRequest
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
