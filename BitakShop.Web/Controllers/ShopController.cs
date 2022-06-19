using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Infratructure.Dtos.Product;
using BitakShop.Infratructure.Repositories;
using BitakShop.Infratructure.Services;
using BitakShop.Web.ViewModels;
using BitakShop.Web.Providers;
using Newtonsoft.Json;

namespace BitakShop.Web.Controllers
{
    public class ShopController :Controller
    {
        private readonly ProductService _productService;
        private readonly ProductsRepository _productsRepo;
        private readonly ProductGroupsRepository _productGroupRepo;
        private readonly FeaturesRepository _featuresRepo;
        private readonly BrandsRepository _brandsRepo;
        private readonly ProductGalleriesRepository _productGalleryRepo;
        private readonly ProductCommentsRepository _productCommentsRepository;
        private readonly ProductMainFeaturesRepository _productMainFeaturesRepo;
        private readonly ProductFeatureValuesRepository _productFeatureValueRepo;
        private readonly GeoDivisionsRepository _geoDivisionRepo;
        private readonly DiscountsRepository _discountsRepo;
        private UsersRepository _usersRepo;
        private CustomersRepository _customerRepo;
        private ShoppingRepository _shoppingRepo;
        private readonly StaticContentDetailsRepository _staticContentRepo;
        private readonly InvoicesRepository _invoicesRepository;

        public ShopController(ProductService productService,
            ProductGroupsRepository productGroupRepo,
            FeaturesRepository featuresRepo,
            BrandsRepository brandsRepo, ProductsRepository productsRepo,
            ProductGalleriesRepository productGalleryRepo,
            ProductCommentsRepository productCommentsRepository,
            ProductMainFeaturesRepository productMainFeaturesRepo,
            ProductFeatureValuesRepository productFeatureValueRepo,
            GeoDivisionsRepository geoDivisionRepo,
            UsersRepository usersRepo,
            CustomersRepository customerRepo,
            ShoppingRepository shoppingRepo,
            DiscountsRepository discountsRepository,
            StaticContentDetailsRepository staticContentDetailsRepository,
            InvoicesRepository invoicesRepository)
        {
            _productService = productService;
            _productGroupRepo = productGroupRepo;
            _featuresRepo = featuresRepo;
            _brandsRepo = brandsRepo;
            _productsRepo = productsRepo;
            _productGalleryRepo = productGalleryRepo;
            _productCommentsRepository = productCommentsRepository;
            _productMainFeaturesRepo = productMainFeaturesRepo;
            _productFeatureValueRepo = productFeatureValueRepo;
            _geoDivisionRepo = geoDivisionRepo;
            _usersRepo = usersRepo;
            _customerRepo = customerRepo;
            _shoppingRepo = shoppingRepo;
            _discountsRepo = discountsRepository;
            _staticContentRepo = staticContentDetailsRepository;
            _invoicesRepository = invoicesRepository;
        }
        // GET: Products
        [Route("Shop/ProductList/{id}/{title}")]
        [Route("Shop/ProductList/{id}")]
        [Route("Shop/ProductList")]
        [Route("Shop/")]
        [Route("Shop/ProductList/Search/{searchString}")]
        public ActionResult Index(int? id, string searchString = null)
        {
            var vm = new ProductListViewModel();
            vm.SelectedGroupId = id ?? 0;
            var productGroups = new List<ProductGroup>();
            if (id == null)
            {
                vm.Features = _featuresRepo.GetAllFeatures();
                vm.Brands = _brandsRepo.GetAll();

                var childrenGroups = _productGroupRepo.GetChildrenProductGroups();
                vm.ProductGroups = childrenGroups;
                ViewBag.Title = "محصولات";
            }
            else
            {
                vm.Features = _featuresRepo.GetAllGroupFeatures(id.Value);
                vm.Brands = _brandsRepo.GetAllGroupBrands(id.Value);
                var selectedProductGroup = _productGroupRepo.Get(id.Value);
                var childrenGroups = _productGroupRepo.GetChildrenProductGroups(id.Value);
                vm.ProductGroups = childrenGroups;
                ViewBag.ProductGroupName = selectedProductGroup.Title;
                ViewBag.ProductGroupId = selectedProductGroup.Id;
                ViewBag.Title = $"محصولات {selectedProductGroup.Title}";
            }

            ViewBag.SearchString = searchString;
            var sidebarImages = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.SidebarImages);
            ViewBag.SidebarImages = sidebarImages.OrderBy(s => s.Id).ToList();
            return View(vm);
        }

        [Route("Shop/Discounts/{groupId}")]
        [Route("Shop/Discounts/")]
        public ActionResult Discounts(string groupId="")
        {
            if (string.IsNullOrEmpty(groupId))
            {
                var discountList = _discountsRepo.GetDistinctedDiscounts();
                groupId = discountList.Count > 0 ? discountList[0].GroupIdentifier : "";
            }

            // if there are no discounts
            if (string.IsNullOrEmpty(groupId))
            {
                return Redirect("/Shop");
            }

            var vm = new ProductListViewModel();

            vm.Features = _featuresRepo.GetAllFeatures();
            vm.Brands = _brandsRepo.GetAll();
            vm.ProductGroups = new List<ProductGroup>();

            var discounts = _discountsRepo.GetDiscountsByGroupIdentifier(groupId);
            ViewBag.Title = discounts != null ? discounts[0].Title : "";
            ViewBag.GroupIdentifier = groupId;

            

            return View(vm);
        }

        public ActionResult LatestProductsSidebar(int take)
        {
            var products = _productService.GetTopSoldProductsWithPrice(take);
            var vm = new List<ProductWithPriceViewModel>();
            foreach (var product in products)
                vm.Add(new ProductWithPriceViewModel(product));

            return PartialView(vm);
        }
        [Route("ProductsGrid")]
        public ActionResult ProductsGrid(GridViewModel grid)
        {
            var products = new List<Product>();

            var brandsIntArr = new List<int>();

            if (string.IsNullOrEmpty(grid.brands) == false)
            {
                var brandsArr = grid.brands.Split('-').ToList();
                brandsArr.ForEach(b => brandsIntArr.Add(Convert.ToInt32(b)));
            }

            var subFeaturesIntArr = new List<int>();
            if (string.IsNullOrEmpty(grid.subFeatures) == false)
            {
                var subFeaturesArr = grid.subFeatures.Split('-').ToList();
                subFeaturesArr.ForEach(b => subFeaturesIntArr.Add(Convert.ToInt32(b)));
            }

            if (string.IsNullOrEmpty(grid.discountGroupIdentifier))
                products = _productService.GetProductsGrid(grid.categoryId, brandsIntArr, subFeaturesIntArr, grid.priceFrom, grid.priceTo, grid.searchString);
            else
                products = _productService.GetDiscountProducts(grid.discountGroupIdentifier, subFeaturesIntArr, grid.priceFrom, grid.priceTo, grid.searchString);

            #region Sorting

            if (grid.sort != "date")
            {
                switch (grid.sort)
                {
                    case "name":
                        products = products.OrderBy(p => p.Title).ToList();
                        break;
                    case "sale":
                        products = products.OrderByDescending(p => _productService.GetProductSoldCount(p)).ToList();
                        break;
                    case "price-high-to-low":
                        products = products.OrderByDescending(p => _productService.GetProductPriceAfterDiscount(p)).ToList();
                        break;
                    case "price-low-to-high":
                        products = products.OrderBy(p => _productService.GetProductPriceAfterDiscount(p)).ToList();
                        break;
                }
            }
            #endregion

            var count = products.Count;
            var skip = grid.pageNumber * grid.take - grid.take;
            int pageCount = (int)Math.Ceiling((double)count / grid.take);
            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = grid.pageNumber;

            products = products.Skip(skip).Take(grid.take).ToList();

            var vm = new List<ProductWithPriceDto>();
            foreach (var product in products)
                vm.Add(_productService.CreateProductWithPriceDto(product));

            return PartialView(vm);
        }

        [Route("Shop/Product/{id}/{title}")]
        [Route("Shop/Product/{id}")]
        [Route("Shop/ProductDetails/{id}")]
        public ActionResult ProductDetails(int id)
        {
            var product = _productsRepo.GetProduct(id);
            var productGallery = _productGalleryRepo.GetProductGalleries(id);
            var productMainFeatures = _productMainFeaturesRepo.GetProductMainFeatures(id);
            var productFeatureValues = _productFeatureValueRepo.GetProductFeatures(id);
            var price = _productService.GetProductPrice(product);
            var priceAfterDiscount = _productService.GetProductPriceAfterDiscount(product);
            var productComments = _productCommentsRepository.GetProductComments(id);
            var productCommentsVm = new List<ProductCommentViewModel>();

            foreach (var item in productComments)
                productCommentsVm.Add(new ProductCommentViewModel(item));

            var vm = new ProductDetailsViewModel()
            {
                Product = product,
                ProductGalleries = productGallery,
                ProductMainFeatures = productMainFeatures,
                ProductFeatureValues = productFeatureValues,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount,
                ProductComments = productCommentsVm
            };
            return View(vm);
        }

        public ActionResult RelatedProductsSection(int productId, int take)
        {
            var products = _productService.GetRelatedProducts(productId, take);
            return PartialView(products);
        }
        [HttpPost]
        public ActionResult PostComment(CommentFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var comment = new ProductComment()
                {
                    ProductId = form.ProductId.Value,
                    ParentId = form.ParentId,
                    Rate = form.Rate,
                    Name = form.Name,
                    Email = form.Email,
                    Message = form.Message,
                    AddedDate = DateTime.Now,
                };
                _productCommentsRepository.Add(comment);
                return RedirectToAction("ContactUsSummary", "Home");
            }
            return RedirectToAction("ProductDetails", new { id = form.ProductId });
        }
        public string GetProductPrice(int productId, int mainFeatureId)
        {
            var product = _productsRepo.Get(productId);
            var price = _productService.GetProductPrice(product, mainFeatureId);
            var priceAfterDiscount = _productService.GetProductPriceAfterDiscount(product, mainFeatureId);
            var result = new
            {
                price = price.ToString("##,###"),
                priceAfterDiscount = priceAfterDiscount.ToString("##,###")
            };
            var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonStr;
        }

        public ActionResult AddItemToCart(int productId, int? mainFeatureId, int count = 1)
        {

            count = count <= 0 ? 1 : count;
            var cartModel = new CartModel();
            var cartItemsModel = new List<CartItemModel>();
            if (Session["cart"] == null)
            {
                Session.Add("cart", "");
            }

            var response = "ran out";
            try
            {

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (Session["cart"] != null && Session["cart"].ToString() != "")
                {
                    string cartJsonStr = Session["cart"].ToString();
                    cartModel = new CartModel(cartJsonStr);
                    cartItemsModel = cartModel.CartItems;
                }
                else if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                    cartItemsModel = cartModel.CartItems;
                }
                #endregion

                ProductWithPriceDto product;
                int productStockCount;
                if (mainFeatureId == null)
                {
                    mainFeatureId = _productMainFeaturesRepo.GetByProductId(productId).Id;
                }
                product = _productService.CreateProductWithPriceDto(productId, mainFeatureId.Value);
                productStockCount = _productService.GetProductStockCount(productId, mainFeatureId.Value);

                if (productStockCount > 0)
                {
                    if (cartItemsModel.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value))
                    {
                        if (((cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity) + count) <= productStockCount)
                        {
                            cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity += count;
                            cartModel.TotalPrice += (product.PriceAfterDiscount * count);
                            response = "added";
                        }
                        else
                        {
                            response = "ran out";
                        }
                    }
                    else
                    {
                        if (productStockCount >= count)
                        {
                            cartItemsModel.Add(new CartItemModel()
                            {
                                Id = product.Id,
                                ProductName = product.ShortTitle,
                                Price = product.PriceAfterDiscount,
                                Quantity = count,
                                MainFeatureId = mainFeatureId.Value,
                                Image = product.Image
                            });
                            cartModel.TotalPrice += (product.PriceAfterDiscount * count);
                            response = "added";
                        }
                        else
                        {

                            response = "ran out";
                        }
                    }
                    cartModel.CartItems = cartItemsModel;
                    var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                    cartCookie.Values.Set("cart", jsonStr);

                    cartCookie.Expires = DateTime.Now.AddHours(12);
                    cartCookie.SameSite = SameSiteMode.Lax;

                    Response.Cookies.Add(cartCookie);

                    Session.Remove("cart");
                    Session.Add("cart", JsonConvert.SerializeObject(cartModel));
                }
                else
                {
                    response = "ran out";
                }
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
                response = "error";
            }


            return Redirect("/Shop/Cart");
        }

        [HttpPost]
        public string AddToCart(int productId, int? mainFeatureId)
        {
            var cartModel = new CartModel();
            string message = "success";
            var cartItemsModel = new List<CartItemModel>();

            #region Checking for cookie
            HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

            if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
            {
                string cartJsonStr = cartCookie.Values["cart"];
                cartModel = new CartModel(cartJsonStr);
                cartItemsModel = cartModel.CartItems;
            }
            #endregion

            ProductWithPriceDto product;
            int productStockCount;
            if (mainFeatureId == null)
            {
                mainFeatureId = _productMainFeaturesRepo.GetByProductId(productId).Id;
            }
            product = _productService.CreateProductWithPriceDto(productId, mainFeatureId.Value);
            productStockCount = _productService.GetProductStockCount(productId, mainFeatureId.Value);

            if (productStockCount > 0)
            {
                if (cartItemsModel.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value))
                {
                    if (cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity < productStockCount)
                    {
                        cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity += 1;
                        cartModel.TotalPrice += product.PriceAfterDiscount;
                    }
                    else
                    {
                        message = "finished";
                    }
                }
                else
                {
                    cartItemsModel.Add(new CartItemModel()
                    {
                        Id = product.Id,
                        ProductName = product.ShortTitle,
                        Price = product.PriceAfterDiscount,
                        OffAmount = product.Price - product.PriceAfterDiscount,
                        Quantity = 1,
                        MainFeatureId = mainFeatureId.Value,
                        Image = product.Image
                    });
                    cartModel.TotalPrice += product.PriceAfterDiscount;
                }
                cartModel.CartItems = cartItemsModel;
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);

                cartCookie.Expires = DateTime.Now.AddHours(12);

                Response.Cookies.Add(cartCookie);
            }
            else
            {
                message = "finished";
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(message);


        }
        public ActionResult RemoveItemFromCart(int productId, int? mainFeatureId, string complete = null)
        {

            try
            {
                var cartModel = new CartModel();

                if (Session["cart"] == null)
                {
                    Session.Add("cart", "");
                }

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (Session["cart"] != null && Session["cart"].ToString() != "")
                {
                    string cartJsonStr = Session["cart"].ToString();
                    cartModel = new CartModel(cartJsonStr);
                }
                else if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                #endregion

                if (cartModel.CartItems.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId))
                {
                    var itemToRemove = cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId);
                    if (complete == "true" || itemToRemove.Quantity < 2)
                    {
                        cartModel.TotalPrice -= itemToRemove.Price * itemToRemove.Quantity;
                        cartModel.CartItems.Remove(itemToRemove);
                    }
                    else if (complete == "false")
                    {
                        cartModel.TotalPrice -= itemToRemove.Price;
                        cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId).Quantity -= 1;
                    }
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);


                Session.Remove("cart");
                Session.Add("cart", JsonConvert.SerializeObject(cartModel));
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }

            return Redirect("/Shop/Cart");
        }


        [HttpPost]
        public void RemoveFromCart(int productId, int? mainFeatureId, string complete = null)
        {
            var cartModel = new CartModel();

            #region Checking for cookie
            HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

            if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
            {
                string cartJsonStr = cartCookie.Values["cart"];
                cartModel = new CartModel(cartJsonStr);
            }
            #endregion

            if (cartModel.CartItems.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId))
            {
                var itemToRemove = cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId);
                if (complete == "true" || itemToRemove.Quantity < 2)
                {
                    cartModel.TotalPrice -= itemToRemove.Price * itemToRemove.Quantity;
                    cartModel.CartItems.Remove(itemToRemove);
                }
                else if (complete == "false")
                {
                    cartModel.TotalPrice -= itemToRemove.Price;
                    cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId).Quantity -= 1;
                }
            }
            var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
            cartCookie.Values.Set("cart", jsonStr);
            cartCookie.Expires = DateTime.Now.AddHours(12);
            Response.Cookies.Add(cartCookie);
        }
        [HttpPost]
        public void EmptyCart()
        {
            HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

            if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
            {
                var cartModel = new CartModel();
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                Response.Cookies.Add(cartCookie);
            }
        }
        [HttpPost]
        public void AddToWishList(int productId)
        {
            var withListModel = new WishListModel();
            var withListItemsModel = new List<WishListItemModel>();

            #region Checking for cookie
            HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

            if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
            {
                string cartJsonStr = cartCookie.Values["wishList"];
                withListModel = new WishListModel(cartJsonStr);
                withListItemsModel = withListModel.WishListItems;
            }
            #endregion

            var product = _productsRepo.Get(productId);
            if (withListItemsModel.Any(i => i.Id == productId) == false)
            {
                withListItemsModel.Add(new WishListItemModel()
                {
                    Id = product.Id,
                    ProductName = product.ShortTitle,
                    Image = product.Image
                });
            }
            withListModel.WishListItems = withListItemsModel;
            var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
            cartCookie.Values.Set("wishList", jsonStr);

            cartCookie.Expires = DateTime.Now.AddHours(12);

            Response.Cookies.Add(cartCookie);
        }
        [HttpPost]
        public void RemoveFromWishList(int productId)
        {
            var withListModel = new WishListModel();

            #region Checking for cookie
            HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

            if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
            {
                string cartJsonStr = cartCookie.Values["wishList"];
                withListModel = new WishListModel(cartJsonStr);
            }
            #endregion

            if (withListModel.WishListItems.Any(i => i.Id == productId))
            {
                var itemToRemove = withListModel.WishListItems.FirstOrDefault(i => i.Id == productId);
                withListModel.WishListItems.Remove(itemToRemove);
            }
            var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
            cartCookie.Values.Set("wishList", jsonStr);
            cartCookie.Expires = DateTime.Now.AddHours(12);
            Response.Cookies.Add(cartCookie);
        }
        public ActionResult WishList()
        {
            return View();
        }
        public ActionResult WishListTable()
        {
            var wishListModel = new WishListModel();

            HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

            if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
            {
                string cartJsonStr = cartCookie.Values["wishList"];
                wishListModel = new WishListModel(cartJsonStr);
            }

            return PartialView(wishListModel);
        }
        public ActionResult Cart()
        {
            var cartModel = new CartModel();

            HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

            if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
            {
                string cartJsonStr = cartCookie.Values["cart"];
                cartModel = new CartModel(cartJsonStr);
            }
            return View(cartModel);
        }
        public ActionResult CartTable(string invoiceNumber = "")
        {
            var cartModel = new CartModel();
            cartModel.CartItems = new List<CartItemModel>();
            List<string> errors = new List<string>();
            long totalPrice = 0;
            string discountCode = "";
            long discountAmount = 0;
            bool orderExists = false;


            // Reading from database 

            var customer = _customerRepo.GetCurrentCustomer();
            var invoice = new Invoice();
            if(customer != null && !invoiceNumber.Equals(""))
                invoice = _invoicesRepository.GetInvoice(invoiceNumber, customer.Id);

            if (invoiceNumber.Equals("") || invoice == null)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (Session["cart"] != null && Session["cart"].ToString() != "")
                {
                    string cartJsonStr = Session["cart"].ToString();
                    cartModel = new CartModel(cartJsonStr);
                }

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    if (cartModel.CartItems == null || cartModel.CartItems.Count == 0) // sessions has not been used
                    {
                        string cartJsonStr = cartCookie.Values["cart"];
                        cartModel = new CartModel(cartJsonStr);
                    }
                }

                var cartItems = cartModel.CartItems ?? new List<CartItemModel>();
                foreach (var item in cartItems)
                {
                    var activeMainFeatureId = _productMainFeaturesRepo.GetLastActiveMainFeature(item.Id);

                    if(activeMainFeatureId != item.MainFeatureId)
                    {
                        errors.Add("ویژگی های محصول '" + item.ProductName + "' تغییر کرده و امکان ثبت سفارش وجود ندارد. لطفا این محصول را از سبد خود حذف و در صورت تمایل مجدد از فروشگاه آن را انتخاب کنید.");
                    }

                    var product = _productService.CreateProductWithPriceDto(item.Id, item.MainFeatureId);
                    var productStockCount = _productService.GetProductStockCount(item.Id, item.MainFeatureId);
                    if (item.Quantity > productStockCount)
                    {
                        errors.Add("در حال حاظر تنها " + productStockCount + " از محصول " + item.ProductName + "در انبار موجود است. لطفا سبد خرید خود را به روز کنید.");

                    }

                    item.Price = product.PriceAfterDiscount;
                    totalPrice += item.Quantity * item.Price;
                }

                cartModel.CartItems = cartItems;
                cartModel.TotalPrice = totalPrice;
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;

                Response.Cookies.Add(cartCookie);
                invoiceNumber = GenerateInvoiceNumber();
            } // new order
            else
            {
                orderExists = true;

                if (DateTime.Now.Subtract(invoice.AddedDate).TotalDays > 1)
                {
                    return Redirect("/Shop/Expired");
                }

                if (invoice.DiscountAmount > 0)
                {
                    discountAmount = invoice.DiscountAmount;
                    discountCode = invoice.DiscountCode.DiscountCodeStr;

                }

                foreach (var item in invoice.InvoiceItems)
                {
                    var product = _productService.CreateProductWithPriceDto(item.ProductId, item.MainFeatureId);
                    var productStockCount = _productService.GetProductStockCount(item.Id, item.MainFeatureId);
                    if (item.Quantity > productStockCount)
                    {
                        errors.Add("امکان ثبت این سفارش وجود ندارد. در حال حاظر تنها تعداد " + productStockCount + " مورد از محصول " + product.ShortTitle + " در انبار موجود است. ");

                    }

                    CartItemModel cartItem = new CartItemModel();
                    cartItem.Id = item.ProductId;
                    cartItem.Quantity = item.Quantity;
                    cartItem.MainFeatureId = item.MainFeatureId;
                    cartItem.Price = item.Price;
                    cartItem.ProductName = product.ShortTitle;
                    cartItem.Image = product.Image;

                    totalPrice += cartItem.Quantity * cartItem.Price;


                    cartModel.CartItems.Add(cartItem);
                }
                cartModel.TotalPrice = totalPrice;
            } // existing order


            cartModel.TotalPrice -= discountAmount;

            ViewBag.Today = new PersianDateTime(DateTime.Now).ToString("dddd d MMMM yyyy");
            ViewBag.InvoiceNumber = invoiceNumber;
            ViewBag.Errors = errors;
            ViewBag.DiscountCode = discountCode;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.OrderExists = orderExists;

            return PartialView(cartModel);
        }

        [CustomerAuthorize]
        public ActionResult Checkout(string invoiceNumber = "")
        {
            ViewBag.InvoiceNumber = invoiceNumber;
            return View();
        }

        [CustomerAuthorize]
        [HttpPost]
        public ActionResult Checkout(FormCollection collection)
        {
            var invoiceNumber = collection["InvoiceNumber"];
            var discountCodeStr = collection["DiscountCode"];

            if (string.IsNullOrEmpty(invoiceNumber))
                return Redirect("/Shop/Checkout");



            HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");
            var cartModel = new CartModel();
            long totalPricebeforeDiscountCode = 0;

            var currentInvoice = _invoicesRepository.GetInvoice(invoiceNumber);


            var customer = _customerRepo.GetCurrentCustomer();
            if (customer == null)// in case admin has logged in
                return Redirect("/Customer/Auth/Login/?returnUrl=/Shop/Checkout");

            // checkin' for discount code validity
            var discountCode = _shoppingRepo.GetActiveDiscountCode(discountCodeStr, customer.Id);
            long discountCodeAmount = 0;
            int? discountCodeId = null;
            if (discountCode != null)
            {
                discountCodeAmount = discountCode.Value;
                discountCodeId = discountCode.Id;
            }



            if (currentInvoice == null)
            {
                // adding new order
                currentInvoice = new Invoice();
                currentInvoice.InvoiceItems = new List<InvoiceItem>();

                currentInvoice.AddedDate = DateTime.Now;
                currentInvoice.CustomerId = customer.Id;
                currentInvoice.CustomerName = customer.User.FirstName + " " + customer.User.LastName;
                currentInvoice.GeoDivisionId = customer.GeoDivisionId;
                currentInvoice.Address = "";
                currentInvoice.Phone = customer.User.PhoneNumber;
                currentInvoice.PostalCode = "";
                currentInvoice.Email = customer.User.Email;
                currentInvoice.IsPayed = false;
                currentInvoice.InvoiceNumber = invoiceNumber;


                // calculate price for order and adding items to invoice

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                else
                {
                    return Redirect("/Shop/Checkout"); // basket is empty
                }

                var cartItems = cartModel.CartItems;
                if(cartItems.Count<1)
                {
                    return Redirect("/Shop/Checkout/?error=1"); // basket is empty
                }

                foreach (var item in cartItems)
                {
                    InvoiceItem invoiceItem = new InvoiceItem();

                    var activeMainFeatureId = _productMainFeaturesRepo.GetLastActiveMainFeature(item.Id);

                    if (activeMainFeatureId != item.MainFeatureId)
                    {
                        // product features have changed
                        return Redirect("/Shop/Checkout"); 
                    }

                    var product = _productService.CreateProductWithPriceDto(item.Id, item.MainFeatureId);
                    var productStockCount = _productService.GetProductStockCount(item.Id, item.MainFeatureId);
                    if (item.Quantity > productStockCount)
                    {
                        return Redirect("/Shop/Checkout"); // out of product 

                    }

                    item.Price = product.PriceAfterDiscount;
                    totalPricebeforeDiscountCode += item.Quantity * item.Price;

                    invoiceItem.Quantity = item.Quantity;
                    invoiceItem.Price = item.Price;
                    invoiceItem.TotalPrice = item.Quantity * item.Price;
                    invoiceItem.ProductId = product.Id;
                    invoiceItem.MainFeatureId = item.MainFeatureId;
                    invoiceItem.InsertDate = DateTime.Now;
                    invoiceItem.InsertUser = customer.User.UserName;

                    currentInvoice.InvoiceItems.Add(invoiceItem);

                }

                currentInvoice.DiscountAmount = discountCodeAmount;
                currentInvoice.TotalPriceBeforeDiscount = totalPricebeforeDiscountCode;
                currentInvoice.TotalPrice = totalPricebeforeDiscountCode - discountCodeAmount;
                currentInvoice.DiscountCodeId = discountCodeId;

                _invoicesRepository.Add(currentInvoice);
                if (discountCode != null) _shoppingRepo.DeactiveDiscountCode(discountCode.Id); // deactive discount code



                // remove all items from cart
                cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");
                var cart = new CartModel();
                cart.CartItems = new List<CartItemModel>();
                cart.TotalPrice = 0;
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
                cartCookie.Values.Set("cart", jsonStr);

                cartCookie.Expires = DateTime.Now.AddHours(-12);
                cartCookie.SameSite = SameSiteMode.Lax;

                Response.Cookies.Add(cartCookie);

            }
            else
            {
                // using existing order
                if (DateTime.Now.Subtract(currentInvoice.AddedDate).TotalDays > 1) // check if invoice is not expired
                {
                    return Redirect("/Shop/Expired");
                }

                // recalculate price for order

                foreach (var item in currentInvoice.InvoiceItems)
                {
                    var product = _productService.CreateProductWithPriceDto(item.ProductId, item.MainFeatureId);
                    var productStockCount = _productService.GetProductStockCount(item.Id, item.MainFeatureId);
                    if (item.Quantity > productStockCount)
                    {
                        return Redirect("/Shop/Checkout"); // out of product 
                    }
                    item.Price = product.PriceAfterDiscount;
                    totalPricebeforeDiscountCode += item.Quantity * item.Price;


                }

                // updating info

                if (discountCode != null)
                {
                    currentInvoice.DiscountAmount = discountCodeAmount;
                    currentInvoice.DiscountCodeId = discountCodeId;
                }

                currentInvoice.TotalPriceBeforeDiscount = totalPricebeforeDiscountCode;
                currentInvoice.TotalPrice = totalPricebeforeDiscountCode - currentInvoice.DiscountAmount;

                _invoicesRepository.Update(currentInvoice);
                if (discountCode != null) _shoppingRepo.DeactiveDiscountCode(discountCode.Id); // deactive discount code

            }

            return Redirect("/Shop/CheckoutAddress/?invoiceNumber=" + invoiceNumber);

        }

        [CustomerAuthorize]
        public ActionResult CheckoutAddress(string invoiceNumber)
        {
            if (string.IsNullOrEmpty(invoiceNumber))
                return Redirect("/Shop/Checkout");


            var customer = _customerRepo.GetCurrentCustomer();
            Invoice lastInvoice = _invoicesRepository.GetLatestPayedInvoice(customer.Id);
            Invoice currentInvoice = _invoicesRepository.GetInvoice(invoiceNumber);

            if (currentInvoice == null)
                return Redirect("/Shop/Checkout/?invoiceNumber=" + invoiceNumber);

            CheckoutForm CheckoutForm = new CheckoutForm();
            CheckoutForm.InvoiceNumber = invoiceNumber;
            CheckoutForm.Address = currentInvoice.Address ?? (lastInvoice == null ? "" : lastInvoice.Address);
            CheckoutForm.Email = currentInvoice.Email ?? (lastInvoice == null ? customer.User.Email : lastInvoice.Email);
            CheckoutForm.Name = currentInvoice.CustomerName ?? (lastInvoice == null ? (customer.User.FirstName + " " + customer.User.LastName) : lastInvoice.CustomerName);
            CheckoutForm.PostalCode = currentInvoice.PostalCode ?? (lastInvoice == null ? customer.PostalCode : lastInvoice.PostalCode);
            CheckoutForm.Phone = currentInvoice.Phone ?? (lastInvoice == null ? customer.User.PhoneNumber : lastInvoice.Phone);
            CheckoutForm.GeoDivisionId = currentInvoice.GeoDivisionId == null ? 1 : currentInvoice.GeoDivisionId.Value;
            CheckoutForm.DiscountCode = currentInvoice.DiscountCode != null ? currentInvoice.DiscountCode.DiscountCodeStr : "";


            ViewBag.GeoDivisionIds = new SelectList(_geoDivisionRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", CheckoutForm.GeoDivisionId);


            var sidebarImages = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.SidebarImages);
            ViewBag.SidebarImages = sidebarImages.OrderBy(s => s.Id).ToList();
            ViewBag.InvoiceNumber = invoiceNumber;
            return View(CheckoutForm);
        }

        [CustomerAuthorize]
        [HttpPost]
        public ActionResult CheckoutAddress(CheckoutForm checkoutForm)
        {
            if (ModelState.IsValid)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");
                var customer = _customerRepo.GetCurrentCustomer();


                // Update customer info with latest information
                customer.Address = checkoutForm.Address;
                _customerRepo.Update(customer);

                // checkin' for discount code validity
                var discountCode = _shoppingRepo.GetActiveDiscountCode(checkoutForm.DiscountCode, customer.Id);
                long discountCodeAmount = 0;
                int? discountCodeId = null;
                if (discountCode != null)
                {
                    discountCodeAmount = discountCode.Value;
                    discountCodeId = discountCode.Id;
                }

                // Add a new (if not exists, according to invoice number) or update invoice
                var cartModel = new CartModel();
                long totalPricebeforeDiscountCode = 0;

                Invoice currentInvoice = _invoicesRepository.GetInvoice(checkoutForm.InvoiceNumber, customer.Id);
                if (currentInvoice == null)
                {
                    // invoice have to exist already
                    return Redirect("/Shop/Checkout/?invoiceNumber=" + checkoutForm.InvoiceNumber);

                }
                else
                {
                    // using existing order
                    if (DateTime.Now.Subtract(currentInvoice.AddedDate).TotalDays > 1) // check if invoice is not expired
                    {
                        return Redirect("/Shop/Expired");
                    }

                    // recalculate price for order

                    foreach (var item in currentInvoice.InvoiceItems)
                    {
                        var product = _productService.CreateProductWithPriceDto(item.ProductId, item.MainFeatureId);
                        var productStockCount = _productService.GetProductStockCount(item.Id, item.MainFeatureId);
                        if (item.Quantity > productStockCount)
                        {
                            return Redirect("/Shop/Checkout"); // out of product 
                        }
                        item.Price = product.PriceAfterDiscount;
                        totalPricebeforeDiscountCode += item.Quantity * item.Price;


                    }

                    // updating info
                    //currentInvoice.AddedDate = DateTime.Now;
                    currentInvoice.CustomerId = customer.Id;
                    currentInvoice.CustomerName = checkoutForm.Name;
                    currentInvoice.GeoDivisionId = checkoutForm.GeoDivisionId;
                    currentInvoice.Address = checkoutForm.Address;
                    currentInvoice.Phone = checkoutForm.Phone;
                    currentInvoice.PostalCode = checkoutForm.PostalCode;
                    currentInvoice.Email = checkoutForm.Email;
                    currentInvoice.IsPayed = false;

                    if (discountCode != null)
                    {
                        currentInvoice.DiscountAmount = discountCodeAmount;
                        currentInvoice.DiscountCodeId = discountCodeId;
                    }

                    currentInvoice.TotalPriceBeforeDiscount = totalPricebeforeDiscountCode;
                    currentInvoice.TotalPrice = totalPricebeforeDiscountCode - currentInvoice.DiscountAmount;

                    _invoicesRepository.Update(currentInvoice);
                    if (discountCode != null) _shoppingRepo.DeactiveDiscountCode(discountCode.Id); // deactive discount code

                }

            }
            else
            {
                return Redirect("/Shop/CheckoutAddress/?error=1&invoiceNumber=" + checkoutForm.InvoiceNumber);
            }

            return Redirect("/Shop/PaymentMethod/?invoiceNumber=" + checkoutForm.InvoiceNumber);
        }


        [CustomerAuthorize]
        public ActionResult PaymentMethod(string invoiceNumber)
        {
            if (string.IsNullOrEmpty(invoiceNumber))
                return Redirect("/Shop/Checkout");

            var customer = _customerRepo.GetCurrentCustomer();

            var invoice = _invoicesRepository.GetInvoice(invoiceNumber, customer.Id);
            if(invoice == null)
                return Redirect("/Shop/Checkout");

            if(string.IsNullOrEmpty(invoice.Address))
            {
                return Redirect("/Shop/CheckoutAddress/?invoiceNumber="+invoiceNumber);
            }

            var sidebarImages = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.SidebarImages);
            ViewBag.SidebarImages = sidebarImages.OrderBy(s => s.Id).ToList();
            ViewBag.InvoiceNumber = invoiceNumber;

            return View();
        }

        [CustomerAuthorize]
        public ActionResult ConfirmOrder(FormCollection collection, string invoiceNumber)
        {

            if (string.IsNullOrEmpty(invoiceNumber))
                return Redirect("/Shop/Checkout");

            var bankGatewayId = collection["rdoBankGateway"];
            if (string.IsNullOrEmpty(bankGatewayId))
                bankGatewayId = "1";


            var customer = _customerRepo.GetCurrentCustomer();

            var invoice = _invoicesRepository.GetInvoice(invoiceNumber, customer.Id);


            var cartModel = new CartModel();
            cartModel.CartItems = new List<CartItemModel>();
            long totalPrice = 0;
            string discountCode = "";
            long discountAmount = 0;
            long priceBeforeDiscount = 0;




            if (DateTime.Now.Subtract(invoice.AddedDate).TotalDays > 1)
            {
                return Redirect("/Shop/Expired");
            }

            if (invoice.DiscountAmount > 0)
            {
                discountAmount = invoice.DiscountAmount;
                discountCode = invoice.DiscountCode.DiscountCodeStr;

            }

            foreach (var item in invoice.InvoiceItems)
            {
                var product = _productService.CreateProductWithPriceDto(item.ProductId, item.MainFeatureId);
                var productStockCount = _productService.GetProductStockCount(item.ProductId, item.MainFeatureId);
                if (item.Quantity > productStockCount)
                {
                    return Redirect("/Shop/Expired"); // since order been registered we can't change the number of products and because we don't have enough products in the stock, we can't process the order

                }

                CartItemModel cartItem = new CartItemModel();
                cartItem.Id = item.ProductId;
                cartItem.Quantity = item.Quantity;
                cartItem.MainFeatureId = item.MainFeatureId;
                cartItem.Price = item.Price;
                cartItem.ProductName = product.ShortTitle;
                cartItem.Image = product.Image;

                totalPrice += cartItem.Quantity * cartItem.Price;


                cartModel.CartItems.Add(cartItem);
            }
            cartModel.TotalPrice = totalPrice;
            priceBeforeDiscount = totalPrice;

            cartModel.TotalPrice -= discountAmount;


            var sidebarImages = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.SidebarImages);
            ViewBag.SidebarImages = sidebarImages.OrderBy(s => s.Id).ToList();
            ViewBag.CustomerInfoView = new CustomerInfoView(invoice, _geoDivisionRepo);
            ViewBag.Today = new PersianDateTime(invoice.AddedDate).ToString("dddd d MMMM yyyy");
            ViewBag.InvoiceNumber = invoiceNumber;
            ViewBag.DiscountCode = discountCode;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.BankGatewayId = bankGatewayId;
            ViewBag.PriceBeforeDiscount = priceBeforeDiscount;

            return View(cartModel);
        }

        public ActionResult Expired()
        {
            return View();
        }



        public string GenerateInvoiceNumber()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var code = "";
            for (int i = 0; code.Length <= 16 && i < bytes.Length; i++)
            {
                code += (bytes[i] % 10).ToString();
            }

            return code;
        }

        public ActionResult ProductCategories()
        {
            var productGroups = _productGroupRepo.GetProductGroups();

            ViewBag.ProductGroups = productGroups;

            return View();
        }

        [HttpPost]
        public string ApplyDiscountCode(string discountCodeStr, string invoiceNumber = "")
        {
            long finalPrice = 0;
            long discountAmount = 0;
            var invoice = _invoicesRepository.GetInvoice(invoiceNumber);

            if (invoice == null)
            {
                var cartModel = new CartModel();

                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }

                finalPrice = cartModel.TotalPrice;
            }
            else
            {
                finalPrice = invoice.TotalPrice;
            }

            DiscountCodeResponseViewModel discountCodeResponse = new DiscountCodeResponseViewModel();

            // first we need to check if "discountCode" is valid
            var customer = _customerRepo.GetCurrentCustomer();

            if (customer == null)
            {
                discountCodeResponse.Response = "login";
                discountCodeResponse.FinalPrice = finalPrice;
                return Newtonsoft.Json.JsonConvert.SerializeObject(discountCodeResponse);
            }

            // then we recalculate the cart items' price with regard to discountCode price
            var discountCode = _shoppingRepo.GetActiveDiscountCode(discountCodeStr, customer.Id);
            if (discountCode == null)
            {
                discountCodeResponse.Response = "invalid";
            }
            else
            {
                finalPrice -= discountCode.Value;
                discountAmount = discountCode.Value;
                discountCodeResponse.Response = "valid";
            }



            discountCodeResponse.FinalPrice = finalPrice;
            discountCodeResponse.DiscountAmount = discountAmount;

            return Newtonsoft.Json.JsonConvert.SerializeObject(discountCodeResponse);

        }

    }
}