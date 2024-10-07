namespace EscCase.Business.Models.Responses.Product
{
    public class ProductWithoutSensetiveData
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
