using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitakShop.Core.Models;
using BitakShop.Infrastructure;
using BitakShop.Infrastructure.Repositories;

namespace BitakShop.Infratructure.Repositories
{
    public class ShoppingRepository
    {
        private readonly MyDbContext _context;
        public ShoppingRepository(MyDbContext context)
        {
            _context = context;
        }

        public DiscountCode GetActiveDiscountCode(string discountCodeStr, int customerId)
        {
            DateTime today = DateTime.Now;
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.DiscountCodeStr == discountCodeStr && dc.CustomerId == customerId
            && dc.IsActive == true && dc.ActivationStartDate <= today && dc.ActivationEndDate >= today);
            return discountCode;
        }

        public bool AddDiscountCode(DiscountCode discountCode)
        {
            try
            {
                _context.DiscountCodes.Add(discountCode);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void DeactiveDiscountCode(string discountCodeStr)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.DiscountCodeStr == discountCodeStr);
            discountCode.IsActive = false;
            _context.SaveChanges();
        }

        public void DeactiveDiscountCode(int discountId)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.Id == discountId);
            discountCode.IsActive = false;
            _context.SaveChanges();
        }

        public void ActivateDiscountCode(string discountCodeStr)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.DiscountCodeStr == discountCodeStr);
            discountCode.IsActive = true;
            _context.SaveChanges();
        }

        public void ActivateDiscountCode(int discountId)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.Id == discountId);
            discountCode.IsActive = true;
            _context.SaveChanges();
        }
    }
}
