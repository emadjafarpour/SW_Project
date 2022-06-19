using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Infratructure.Dtos.Product
{
    public class ProductGridDto
    {
        public List<ProductWithPriceDto> Products { get; set; }
        public int Count { get; set; }
    }
    public class ProductWithPriceDto
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string ShortTitle { get; set; }
        public long Price { get; set; }
        public long PriceAfterDiscount { get; set; }
    }
}
