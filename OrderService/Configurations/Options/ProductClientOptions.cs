using System.ComponentModel.DataAnnotations;

namespace OrderService.Configurations.Options
{
    public class ProductClientOptions
    {
        public const string Section = "ProductClient";

        [Required]
        [Url]
        public string BaseAddress { get; set; } = string.Empty;
    }
}
