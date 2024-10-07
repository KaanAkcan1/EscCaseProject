using EscCase.Common.Entities.Common;
using System.Text.Json.Serialization;

namespace EscCase.Data.Models
{
    public class Product : BaseEntity
    {
        [JsonPropertyName("ProductName")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("ProductCode")]
        public string Code { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
