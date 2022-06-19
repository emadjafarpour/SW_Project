using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using BitakShop.Core.Models;
using BitakShop.Infrastructure.Extensions;

namespace BitakShop.Infrastructure.Repositories
{
    public class DiscountsRepository : BaseRepository<Discount, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public DiscountsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<Discount> GetDistinctedDiscounts()
        {
            return _context.Discounts.Where(d => d.IsDeleted == false).DistinctBy(d=>d.GroupIdentifier).ToList();
        }

        public List<Discount> GetBannerDiscounts()
        {
            return _context.Discounts.Where(d => d.IsDeleted == false && d.DiscountLocationType > 0).DistinctBy(d => d.GroupIdentifier).ToList();
        }

        public Discount GetMainOffer()
        {
                var offers = _context.Discounts
                .Include(d => d.Product)
                .Where(d => d.IsDeleted == false && (int)d.DiscountLocationType == 5).DistinctBy(d => d.GroupIdentifier)
                .ToList();
            // var res = _context.Discounts.ToList();

            if (offers == null || offers.Count < 1)
            {
                return new Discount();
            }

            return offers[0];
            //if (string.IsNullOrEmpty(offers.Image))
            //{
            //    offers.Image = "default.jpg";
            //}

        }

            

        public List<Discount> GetDiscountGroup(int id)
        {
            var discount = _context.Discounts.Find(id);
            return _context.Discounts.Where(d => d.IsDeleted == false && d.GroupIdentifier == discount.GroupIdentifier).ToList();
        }
        public List<Discount> GetDiscountsByGroupIdentifier(string groupIdentifier)
        {
            return _context.Discounts.Where(d => d.IsDeleted == false && d.GroupIdentifier == groupIdentifier).ToList();
        }

        public Discount GetProductDiscount(int productId)
        {
            return _context.Discounts.FirstOrDefault(d => d.IsDeleted == false && d.ProductId == productId);
        }
        public Discount GetProductGroupDiscount(int productGroupId)
        {
            return _context.Discounts.FirstOrDefault(d => d.IsDeleted == false && d.ProductGroupId == productGroupId);
        }
        public Discount GetBrandDiscount(int brandId)
        {
            return _context.Discounts.FirstOrDefault(d => d.IsDeleted == false && d.BrandId == brandId);
        }
    }
}
