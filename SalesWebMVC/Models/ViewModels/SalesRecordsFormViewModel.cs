using Microsoft.Build.Framework;
using SalesWebMVC.Models.Enums;

namespace SalesWebMVC.Models.ViewModels
{
    public class SalesRecordsFormViewModel
    {
        public int SellerId { get; set; }

        [Required]
        public SalesRecord TemplateSale { get; set; }
        public ICollection<SalesRecord>? AllSales { get; set; }

    }

    
}
