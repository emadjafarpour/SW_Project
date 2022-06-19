using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure.Helpers;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Infratructure.Repositories;
using BitakShop.Infratructure.Services;
using BitakShop.Web.ViewModels;
using Microsoft.AspNet.Identity;

namespace BitakShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly StaticContentDetailsRepository _staticContentRepo;
        private readonly OffersRepository _offersRepo;
        private readonly ProductService _productService;
        private readonly TestimonialsRepository _testimonialRepo;
        private readonly PartnersRepository _partnersRepo;
        private readonly ArticlesRepository _articlesRepo;
        private readonly ContactFormsRepository _contactFormRepo;
        private readonly ProductGroupsRepository _productGroupRepo;
        private readonly DiscountsRepository _discountRepo;
        private readonly TeamMembersRepository _teamMembersRepo;

        public HomeController(StaticContentDetailsRepository staticContentRepo,
            OffersRepository offersRepo,
            ProductService productService,
            TestimonialsRepository testimonialRepo,
            PartnersRepository partnersRepo,
            ArticlesRepository articlesRepo,
            ContactFormsRepository contactFormRepo, ProductGroupsRepository productGroupRepo,
            DiscountsRepository discountsRepository, TeamMembersRepository teamMembersRepository)
        {
            _staticContentRepo = staticContentRepo;
            _offersRepo = offersRepo;
            _productService = productService;
            _testimonialRepo = testimonialRepo;
            _partnersRepo = partnersRepo;
            _articlesRepo = articlesRepo;
            _contactFormRepo = contactFormRepo;
            _productGroupRepo = productGroupRepo;
            _discountRepo = discountsRepository;
            _teamMembersRepo = teamMembersRepository;
        }

        public ActionResult Index()
        {
            /*
            var userIsLoggedIn = User.Identity.IsAuthenticated;
            if (userIsLoggedIn)
            {
                return RedirectToAction("Index", "Dashboard", new {area = "Admin"});
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }*/

            var Discounts = _discountRepo.GetBannerDiscounts();
            var sidebarImages = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.SidebarImages);
            ViewBag.CollectionBanner = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.CollectionBanner);
            ViewBag.SidebarImages = sidebarImages.OrderBy(s => s.Id).ToList();

            return View(Discounts);
        }

        public ActionResult HeaderSection()
        {
            var mainProductGroups = _productGroupRepo.GetChildrenProductGroups();
            ViewBag.Phone = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Phone);
            ViewBag.MainProductGroups = mainProductGroups;
            ViewBag.SocialMedia = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.SocialMedia);
            return PartialView();
        }

        public ActionResult FooterSection()
        {
            var map = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.ContactUsMap);
            var phone = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Phone);
            var email = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Email);
            var address = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Address);
            var vm = new ContactUsViewModel()
            {
                Map = map.Description,
                Phone = phone.Title,
                Email = email.Title,
                Address = address.Title
            };
            ViewBag.AboutUsData = vm;

            ViewBag.SocialMedia = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.SocialMedia);
            return PartialView();
        }

        public ActionResult SideCategoriesMenu()
        {
            var mainProductGroups = _productGroupRepo.GetChildrenProductGroups();
            return PartialView(mainProductGroups);
        }

        public ActionResult CartSection()
        {
            try
            {
                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

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

                return PartialView(cartModel);

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;

                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                return PartialView(cartModel);

            }
        }
        public ActionResult SliderSection()
        {
            var content = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.Slider);
            return PartialView(content);
        }

        public ActionResult OffersSection()
        {
            var offers = _offersRepo.GetAll();
            offers = offers.OrderBy(o => o.Id).ToList();
            return PartialView(offers);
        }

        public ActionResult TopSoldProductsSection(int take)
        {
            var products = _productService.GetTopSoldProductsWithPrice(take);
            var vm = new List<ProductWithPriceViewModel>();
            foreach (var product in products)
                vm.Add(new ProductWithPriceViewModel(product));

            return PartialView(vm);
        }

        public ActionResult TestimonialsSection()
        {
            var testimonials = _testimonialRepo.GetAll();
            var vm = testimonials.Select(testimonial => new TestimonialViewModel(testimonial)).ToList();

            return PartialView(vm);
        }
        public ActionResult LatestProductsSection(int take=8, string viewType="sidebar")
        {
            var products = _productService.GetLatestProductsWithPrice(take);
            var vm = new List<ProductWithPriceViewModel>();
            foreach (var product in products)
                vm.Add(new ProductWithPriceViewModel(product));

            ViewBag.ViewType = viewType;
            return PartialView(vm);
        }
        public ActionResult PartnersSection()
        {
            var partners = _partnersRepo.GetAll();
            return PartialView(partners);
        }

        public ActionResult LatestArticlesSection()
        {
            var articles = _articlesRepo.GetLatestArticles(3);
            var vm = articles.Select(item => new LatestArticlesViewModel(item)).ToList();

            return PartialView(vm);
        }

        public ActionResult LatestArticlesSideSection(int take)
        {
            var articles = _articlesRepo.GetLatestArticles(take);
            var vm = articles.Select(item => new LatestArticlesViewModel(item)).ToList();

            return PartialView(vm);
        }

        public ActionResult SpecialOffer()
        {
           var SpecialOffer = _discountRepo.GetMainOffer();
            return PartialView(SpecialOffer);
        }

        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var vFileName = DateTime.Now.ToString("yyyyMMdd-HHMMssff") +
                                    Path.GetExtension(upload.FileName).ToLower();
                    var vFolderPath = Server.MapPath("/Upload/");
                    if (!Directory.Exists(vFolderPath))
                    {
                        Directory.CreateDirectory(vFolderPath);
                    }
                    vFilePath = Path.Combine(vFolderPath, vFileName);
                    upload.SaveAs(vFilePath);
                    vImagePath = Url.Content("/Upload/" + vFileName);
                    vMessage = "Image was saved correctly";
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(vOutput);
        }

        [Route("AboutUs")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            var aboutUs = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.AboutUs);

            ViewBag.TeamMembers = _teamMembersRepo.GetTeamMembers();

            return View(aboutUs);
        }

        [Route("ContactUs")]
        public ActionResult Contact()
        {
            var map = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.ContactUsMap);
            var phone = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Phone);
            var email = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Email);
            var address = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Address);
            var vm = new ContactUsViewModel()
            {
                Map = map.Description,
                Phone = phone.Title,
                Email = email.Title,
                Address = address.Title
            };
            return View(vm);
        }
        public ActionResult ContactUsSummary()
        {
            var url = "";
            try
            {
                url = Request.QueryString["ReturnPath"];
            }
            catch
            {

            }

            ViewBag.URL = url;

            return View();
        }

        [Route("NotFound")]
        public ActionResult NotFound()
        {
            return View();
        }


        public ActionResult ProductCategoriesSection()
        {
            var mainProductGroups = _productGroupRepo.GetChildrenProductGroups();
            ViewBag.MainProductGroups = mainProductGroups;
            return PartialView();
        }


    }
}