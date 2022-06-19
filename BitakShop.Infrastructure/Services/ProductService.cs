using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Infratructure.Dtos.Product;

namespace BitakShop.Infratructure.Services
{
    public class ProductService
    {
        private readonly InvoicesRepository _InvoiceRepo;
        private readonly ProductsRepository _productRepo;
        private readonly ProductMainFeaturesRepository _productMainFeatureRepo;
        private readonly DiscountsRepository _discountRepo;
        private readonly MyDbContext _context;
        public ProductService(ProductsRepository productRepo, InvoicesRepository invoiceRepo, ProductMainFeaturesRepository productMainFeatureRepo, DiscountsRepository discountRepo, MyDbContext context)
        {
            _productRepo = productRepo;
            _InvoiceRepo = invoiceRepo;
            _productMainFeatureRepo = productMainFeatureRepo;
            _discountRepo = discountRepo;
            _context = context;
        }
        public List<Product> GetTopSoldProducts(int take)
        {
            var products = new List<Product>();
            var TopSoldProducts = _InvoiceRepo.GertTopSoldProducts(take);
            if (TopSoldProducts.Any() == false)
                products = _productRepo.GetNewestProducts(take);
            else
            {
                products = TopSoldProducts;
            }

            return products;
        }

        public int GetProductSoldCount(Product product)
        {
            var amount = 0;
            var invoices = _context.InvoiceItems.Where(i => i.ProductId == product.Id && i.IsDeleted == false ).ToList();
            if (invoices.Any())
                amount += invoices.Sum(i => i.Quantity);
            return amount;
        }

        public int GetProductStockCount(int productId)
        {
            var inStock = 0;
            var mainFeature = _productMainFeatureRepo.GetByProductId(productId);
            if (mainFeature != null)
                inStock += mainFeature.Quantity;
            return inStock;
        }
        public int GetProductStockCount(int productId,int mainFeatureId)
        {
            var inStock = 0;
            var mainFeature = _productMainFeatureRepo.Get(mainFeatureId);
            if (mainFeature != null)
                inStock += mainFeature.Quantity;
            return inStock;
        }
        public long GetProductPrice(Product product)
        {
            long price = 0;
            var mainFeature = _productMainFeatureRepo.GetByProductId(product.Id);
            if (mainFeature != null && mainFeature.Quantity > 0)
            {
                price = mainFeature.Price;
            }

            return price;
        }
        public long GetProductPrice(Product product,int mainFeatureId)
        {
            long price = 0;
            var mainFeature = _productMainFeatureRepo.GetByProductId(product.Id,mainFeatureId);
            if (mainFeature != null && mainFeature.Quantity > 0)
            {
                price = mainFeature.Price;
            }

            return price;
        }
        public long GetProductPriceAfterDiscount(Product product)
        {
            var productPrice = GetProductPrice(product);
            var priceAfterDiscount = productPrice;

            // Checking For Product Discount
            var discount = _discountRepo.GetProductDiscount(product.Id);

            // Checking For ProductGroupDiscount
            if (discount == null)
                discount = _discountRepo.GetProductGroupDiscount(product.ProductGroupId ?? 0);

            // Checking For Brand Discount
            if (discount == null)
                discount = _discountRepo.GetBrandDiscount(product.BrandId ?? 0);

            if (discount != null)
            {
                if (discount.DiscountType == (int)DiscountType.Amount)
                {
                    priceAfterDiscount -= discount.Amount;
                }
                else if (discount.DiscountType == (int)DiscountType.Percentage)
                {
                    var discountAmount = (discount.Amount * productPrice / 100);
                    priceAfterDiscount -= discountAmount;
                }
            }

            return priceAfterDiscount;
        }
        public long GetProductPriceAfterDiscount(Product product,int mainFeatureId)
        {
            var productPrice = GetProductPrice(product,mainFeatureId);
            var priceAfterDiscount = productPrice;

            // Checking For Product Discount
            var discount = _discountRepo.GetProductDiscount(product.Id);

            // Checking For ProductGroupDiscount
            if (discount == null)
                discount = _discountRepo.GetProductGroupDiscount(product.ProductGroupId ?? 0);

            // Checking For Brand Discount
            if (discount == null)
                discount = _discountRepo.GetBrandDiscount(product.BrandId ?? 0);

            if (discount != null)
            {
                if (discount.DiscountType == (int)DiscountType.Amount)
                {
                    priceAfterDiscount -= discount.Amount;
                }
                else if (discount.DiscountType == (int)DiscountType.Percentage)
                {
                    var discountAmount = (discount.Amount * productPrice / 100);
                    priceAfterDiscount -= discountAmount;
                }
            }

            return priceAfterDiscount;
        }
        public List<ProductWithPriceDto> GetTopSoldProductsWithPrice(int take)
        {
            var productsDto = new List<ProductWithPriceDto>();
            var products = GetTopSoldProducts(take);

            #region Getting Product Price And Discount

            foreach (var product in products)
            {
                var price = GetProductPrice(product);
                var priceAfterDiscount = GetProductPriceAfterDiscount(product);
                var productDto = new ProductWithPriceDto()
                {
                    Id = product.Id,
                    ShortTitle = product.ShortTitle,
                    Image = product.Image,
                    Price = price,
                    PriceAfterDiscount = priceAfterDiscount
                };
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }
        public List<ProductWithPriceDto> GetRelatedProducts(int productId,int take)
        {
            var productt = _productRepo.Get(productId);
            var relatedProducts = _context.Products
                .Where(p => p.ProductGroupId == productt.ProductGroupId && p.IsDeleted == false && p.Id != productId)
                .Take(take).ToList();
            var productsDto = new List<ProductWithPriceDto>();

            #region Getting Product Price And Discount

            foreach (var product in relatedProducts)
            {
                var price = GetProductPrice(product);
                var priceAfterDiscount = GetProductPriceAfterDiscount(product);
                var productDto = new ProductWithPriceDto()
                {
                    Id = product.Id,
                    ShortTitle = product.ShortTitle,
                    Image = product.Image,
                    Price = price,
                    PriceAfterDiscount = priceAfterDiscount
                };
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }
        public List<ProductWithPriceDto> GetLatestProductsWithPrice(int take)
        {
            var productsDto = new List<ProductWithPriceDto>();
            var products = _productRepo.GetNewestProducts(take);

            #region Getting Product Price And Discount

            foreach (var product in products)
            {
                var price = GetProductPrice(product);
                var priceAfterDiscount = GetProductPriceAfterDiscount(product);
                var productDto = new ProductWithPriceDto()
                {
                    Id = product.Id,
                    ShortTitle = product.ShortTitle,
                    Image = product.Image,
                    Price = price,
                    PriceAfterDiscount = priceAfterDiscount
                };
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }
        public ProductWithPriceDto CreateProductWithPriceDto(int productId)
        {
            var product = _productRepo.Get(productId);
            var price = GetProductPrice(product);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product);
            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.ShortTitle,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount
            };
            return productDto;
        }
        public ProductWithPriceDto CreateProductWithPriceDto(int productId,int mainFeatureId)
        {
            var product = _productRepo.Get(productId);
            var price = GetProductPrice(product,mainFeatureId);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product,mainFeatureId);
            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.ShortTitle,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount
            };
            return productDto;
        }
        public ProductWithPriceDto CreateProductWithPriceDto(Product product)
        {
            var price = GetProductPrice(product);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product);
            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.ShortTitle,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount
            };
            return productDto;
        }
        public ProductWithPriceDto CreateProductWithPriceDto(Product product, int mainFeatureId)
        {
            var price = GetProductPrice(product, mainFeatureId);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product, mainFeatureId);
            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.ShortTitle,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount
            };
            return productDto;
        }
        public List<int> GetAllChildrenProductGroupIds(int id)
        {
            var ids = new List<int>();
            ids.AddRange(_context.ProductGroups.Where(p => p.IsDeleted == false && p.ParentId == id).Select(p => p.Id).ToList());
            foreach (var item in ids.ToList())
            {
                var childIds = GetAllChildrenProductGroupIds(item);
                if (childIds.Any())
                {
                    ids.AddRange(childIds);
                }
            }
            return ids;
        }
        #region Get Products Grid

        public List<Product> GetProductsGrid(int? productGroupId, List<int> brandIds = null, List<int> subFeatureIds = null,long? fromPrice = null,long? toPrice = null,string searchString = null)
        {
            var products = new List<Product>();
            var count = 0;
            if (productGroupId == null || productGroupId == 0)
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    products = _context.Products.Include(p => p.ProductMainFeatures).Include(p => p.ProductFeatureValues).Where(p => p.IsDeleted == false).OrderByDescending(p => p.InsertDate).ToList();
                }
                else
                {
                 products = _context.Products.Include(p => p.ProductMainFeatures)
                     .Include(p => p.ProductFeatureValues)
                     .Where(p => p.IsDeleted == false && (p.ShortTitle.Trim().ToLower().Contains(searchString.Trim().ToLower()) || p.Title.Trim().ToLower().Contains(searchString.Trim().ToLower())))
                     .OrderByDescending(p => p.InsertDate).ToList();
                }
            }
            else
            {
                products = _context.Products.Include(p=>p.ProductMainFeatures).Include(p=>p.ProductFeatureValues).Where(p => p.IsDeleted == false && p.ProductGroupId == productGroupId).OrderByDescending(p => p.InsertDate).ToList();

                var allChildrenGroups = GetAllChildrenProductGroupIds(productGroupId.Value);
                foreach (var groupId in allChildrenGroups)
                    products.AddRange(_context.Products.Where(p => p.IsDeleted == false && p.ProductGroupId == groupId).OrderByDescending(p => p.InsertDate).ToList());
                if (string.IsNullOrEmpty(searchString) == false)
                {
                    products = products
                        .Where(p => p.IsDeleted == false && (p.ShortTitle.Trim().ToLower().Contains(searchString.Trim().ToLower()) || p.Title.Trim().ToLower().Contains(searchString.Trim().ToLower())))
                        .OrderByDescending(p => p.InsertDate).ToList();
                }
            }

            if (brandIds != null && brandIds.Any())
            {
                var productsFilteredByBrand = new List<Product>();
                foreach (var brand in brandIds)
                    productsFilteredByBrand.AddRange(products.Where(p => p.IsDeleted == false && p.BrandId == brand).OrderByDescending(p => p.InsertDate).ToList());
                products = productsFilteredByBrand;
            }
            if (subFeatureIds != null && subFeatureIds.Any(f=>f != 0))
            {
                var productsFilteredByFeature = new List<Product>();
                foreach (var subFeature in subFeatureIds.Where(f=>f != 0))
                    productsFilteredByFeature.AddRange(products.Where(p => p.ProductFeatureValues.Any(pf => pf.SubFeatureId == subFeature) || p.ProductMainFeatures.Any(pf => pf.SubFeatureId == subFeature)).OrderByDescending(p => p.InsertDate).ToList());
                products = productsFilteredByFeature;
            }

            if (fromPrice != null)
                products = products.Where(p => GetProductPriceAfterDiscount(p) >= fromPrice).ToList();

            if (toPrice != null)
                products = products.Where(p => GetProductPriceAfterDiscount(p) <= toPrice).ToList();

            return products;
        }
        #endregion


        public List<Product> GetDiscountProducts(string groupIdentifier, List<int> subFeatureIds = null, long? fromPrice = null, long? toPrice = null, string searchString = null)
        {

            var discounts = _discountRepo.GetDiscountsByGroupIdentifier(groupIdentifier);

            var productList = new List<Product>();

            foreach(var discount in discounts)
            {
                // processes when the discount is a product
                if(discount.ProductId!=null)
                {
                    var product = _context.Products.Include(p => p.ProductMainFeatures)
                     .Include(p => p.ProductFeatureValues).FirstOrDefault(p => p.Id == discount.ProductId);

                    productList.Add(product);
                }

                // processes when the discount is a brand
                if(discount.BrandId != null)
                {
                    var productsFilteredByBrand = new List<Product>();
                    productsFilteredByBrand.AddRange(_context.Products.Where(p => p.IsDeleted == false && p.BrandId == discount.BrandId).OrderByDescending(p => p.InsertDate).ToList());
                    productList.AddRange(productsFilteredByBrand);
                }

                // processes when the discount is a productGroup
                if (discount.ProductGroupId != null)
                {
                    var productsFilteredByGroup = _context.Products.Include(p => p.ProductMainFeatures).Include(p => p.ProductFeatureValues).Where(p => p.IsDeleted == false && p.ProductGroupId == discount.ProductGroupId).OrderByDescending(p => p.InsertDate).ToList();
                    productList.AddRange(productsFilteredByGroup);
                }

            }

            // applying filters
            if(!string.IsNullOrEmpty(searchString))
                productList = productList.Where(p => (p.ShortTitle.Trim().ToLower().Contains(searchString.Trim().ToLower()) || p.Title.Trim().ToLower().Contains(searchString.Trim().ToLower()))).ToList();

            if (subFeatureIds != null && subFeatureIds.Any(f => f != 0))
            {
                var productsFilteredByFeature = new List<Product>();
                foreach (var subFeature in subFeatureIds.Where(f => f != 0))
                    productsFilteredByFeature.AddRange(productList.Where(p => p.ProductFeatureValues.Any(pf => pf.SubFeatureId == subFeature) || p.ProductMainFeatures.Any(pf => pf.SubFeatureId == subFeature)).OrderByDescending(p => p.InsertDate).ToList());
                productList = productsFilteredByFeature;
            }


            if (fromPrice != null)
                productList = productList.Where(p => GetProductPriceAfterDiscount(p) >= fromPrice).ToList();

            if (toPrice != null)
                productList = productList.Where(p => GetProductPriceAfterDiscount(p) <= toPrice).ToList();

            return productList;

        }

        public void DecreaseStockProductCount(int productId, int mainFeatureId, int count)
        {
            var inStock = 0;
            var mainFeature = _productMainFeatureRepo.Get(mainFeatureId);
            if (mainFeature != null)
            {
                mainFeature.Quantity -= count;
                _productMainFeatureRepo.Update(mainFeature);
            }
        }



    }
}
