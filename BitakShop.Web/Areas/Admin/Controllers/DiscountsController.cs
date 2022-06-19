using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Web.ViewModels;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class DiscountsController : Controller
    {
        private readonly DiscountsRepository _repo;
        private readonly OffersRepository _offerRepo;
        private readonly BrandsRepository _brandRepo;
        private readonly ProductGroupsRepository _productGroupRepo;
        private readonly ProductsRepository _productRepo;

        public DiscountsController(DiscountsRepository repo, OffersRepository offerRepo, BrandsRepository brandRepo, ProductGroupsRepository productGroupRepo, ProductsRepository productRepo)
        {
            _repo = repo;
            _offerRepo = offerRepo;
            _brandRepo = brandRepo;
            _productGroupRepo = productGroupRepo;
            _productRepo = productRepo;
        }
        // GET: Admin/Discounts
        public ActionResult Index()
        {
            return View(_repo.GetDistinctedDiscounts());
        }
        public ActionResult Create()
        {
            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();
            return View();
        }
        [HttpPost]
        public string Create(DiscountFormViewModel newDiscount)
        {

            if (ModelState.IsValid)
            {
                var groupIdentifier = Guid.NewGuid().ToString();
                #region Adding Brands Discounts
                if (newDiscount.BrandIds != null)
                {
                    foreach (var item in newDiscount.BrandIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            DeadLine = ConvertPersianDateStrToDatetime(newDiscount.DeadLine),
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            BrandId = item,
                            GroupIdentifier = groupIdentifier,
                            DiscountLocationType = newDiscount.DiscountLocationType
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion
                #region Adding ProductGroups Discounts
                if (newDiscount.ProductGroupIds != null)
                {
                    foreach (var item in newDiscount.ProductGroupIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            DeadLine = ConvertPersianDateStrToDatetime(newDiscount.DeadLine),
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductGroupId = item,
                            GroupIdentifier = groupIdentifier,
                            DiscountLocationType = newDiscount.DiscountLocationType
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion

                #region Adding Products Discounts
                if (newDiscount.ProductIds != null)
                {
                    foreach (var item in newDiscount.ProductIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            DeadLine = ConvertPersianDateStrToDatetime(newDiscount.DeadLine),
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductId = item,
                            GroupIdentifier = groupIdentifier,
                            DiscountLocationType = newDiscount.DiscountLocationType
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion


                return groupIdentifier;
            }
            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();
            return "";
        }
        public ActionResult Edit(int id)
        {
            #region Edit Props

            var vm = new DiscountFormViewModel();
            var discountGroup = _repo.GetDiscountGroup(id);
            var groupIdentifier = discountGroup.FirstOrDefault().GroupIdentifier;
            vm.PreviousDiscounts = discountGroup;
            vm.GroupIdentifier = groupIdentifier;
            vm.Title = discountGroup.FirstOrDefault().Title;
            vm.OfferId = discountGroup.FirstOrDefault().OfferId;
            vm.DiscountType = discountGroup.FirstOrDefault().DiscountType;
            vm.Amount = discountGroup.FirstOrDefault().Amount;
            vm.DeadLine = GetPersianDate(discountGroup.FirstOrDefault().DeadLine);//discountGroup.FirstOrDefault().DeadLine.ToString();
            vm.Image = discountGroup.FirstOrDefault().Image;

            #endregion


            ViewBag.DiscountLocationType = (int)discountGroup.FirstOrDefault().DiscountLocationType;
            ViewBag.GroupImage = vm.Image;
            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();


            IDictionary<int, string> locationList = new Dictionary<int, string>();
            locationList.Add(0, "-- هیچکدام --");
            locationList.Add(1, "تخفیف سربرگ");
            locationList.Add(2, "تخفیف نوار کناری");
            locationList.Add(3, "تخفیف اصلی اول");
            locationList.Add(4, "تخفیف اصلی دوم");
            locationList.Add(5, "تخفیف زمان دار");
            ViewBag.LocationList = locationList;

            return View(vm);
        }
        [HttpPost]
        public string Edit(DiscountFormViewModel newDiscount)
        {
            if (ModelState.IsValid)
            {
                #region Removing All Previous Discounts
                var prevDicounts = _repo.GetDiscountsByGroupIdentifier(newDiscount.GroupIdentifier);

                foreach (var item in prevDicounts)
                    _repo.Delete(item.Id);

                #endregion

                var groupIdentifier = Guid.NewGuid().ToString();
                #region Adding Brands Discounts
                if (newDiscount.BrandIds != null)
                {
                    foreach (var item in newDiscount.BrandIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            DeadLine = ConvertPersianDateStrToDatetime(newDiscount.DeadLine),
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            BrandId = item,
                            GroupIdentifier = groupIdentifier,
                            DiscountLocationType = newDiscount.DiscountLocationType
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion
                #region Adding ProductGroups Discounts
                if (newDiscount.ProductGroupIds != null)
                {
                    foreach (var item in newDiscount.ProductGroupIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            DeadLine = ConvertPersianDateStrToDatetime(newDiscount.DeadLine),
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductGroupId = item,
                            GroupIdentifier = groupIdentifier,
                            DiscountLocationType = newDiscount.DiscountLocationType
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion

                #region Adding Products Discounts
                if (newDiscount.ProductIds != null)
                {
                    foreach (var item in newDiscount.ProductIds)
                    {
                        var discount = new Discount()
                        {
                            DiscountType = newDiscount.DiscountType,
                            Amount = newDiscount.Amount,
                            DeadLine = ConvertPersianDateStrToDatetime(newDiscount.DeadLine),
                            OfferId = newDiscount.OfferId,
                            Title = newDiscount.Title,
                            ProductId = item,
                            GroupIdentifier = groupIdentifier,
                            DiscountLocationType = newDiscount.DiscountLocationType
                        };
                        _repo.Add(discount);
                    }
                }
                #endregion

                return groupIdentifier;
            }

            ViewBag.Offers = _offerRepo.GetAll();
            ViewBag.Brands = _brandRepo.GetAll();
            ViewBag.ProductGroups = _productGroupRepo.GetAll();
            ViewBag.Products = _productRepo.GetAll();
            return "";
        }

        [HttpPost]
        public void UploadImage(FormCollection collection, string groupIdentifier = "")
        {
            var discounts = _repo.GetDiscountsByGroupIdentifier(groupIdentifier);

            

            var count = Request.Files.Count;
            var file = Request.Files["image"];
            if (file != null && discounts.Count > 0)
            {
                var previousFile = discounts[0].Image;

                var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                file.SaveAs(Server.MapPath("/Files/DiscountImages/Image/" + newFileName)); 
                foreach (var discount in discounts)
                {
                    discount.Image = newFileName;
                    _repo.Update(discount);
                }

                // remove previous image
                try
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/DiscountImages/Image/" + previousFile)))
                        System.IO.File.Delete(Server.MapPath("/Files/DiscountImages/Image/" + previousFile));
                }
                catch
                {

                }
            }
        }

        [HttpPost]
        public string ValidateDuplicateDiscount(DiscountFormViewModel newDiscount,string groupIdentifier = null)
        {
            var discounts = _repo.GetAll();
            #region Check for Duplicate Brands
            if (newDiscount.BrandIds!= null)
            {
                foreach (var item in newDiscount.BrandIds)
                {
                    if (discounts.Any(d => d.BrandId == item))
                    {
                        var brandName = _brandRepo.GetAll().FirstOrDefault(b => b.Id == item).Name;
                        var discountName = discounts.FirstOrDefault(d => d.BrandId == item).Title;
                        if (groupIdentifier != null)
                        {
                            if (discounts.FirstOrDefault(d => d.BrandId == item).GroupIdentifier != groupIdentifier)
                            {
                                return $"برای برند {brandName} قبلا تخفیف ثبت شده ( {discountName} )";
                            }
                        }
                        else
                        {
                            return $"برای برند {brandName} قبلا تخفیف ثبت شده ( {discountName} )";
                        }
                    }
                }
            }
            #endregion
            #region Check for Duplicate ProductGroups
            if (newDiscount.ProductGroupIds != null)
            {
                foreach (var item in newDiscount.ProductGroupIds)
                {
                    if (discounts.Any(d => d.ProductGroupId == item))
                    {
                        var productGroupName = _productGroupRepo.GetAll().FirstOrDefault(b => b.Id == item).Title;
                        var discountName = discounts.FirstOrDefault(d => d.ProductGroupId == item).Title;
                        if (groupIdentifier != null)
                        {
                            if (discounts.FirstOrDefault(d => d.ProductGroupId == item).GroupIdentifier != groupIdentifier)
                            {
                                return $"برای دسته {productGroupName} قبلا تخفیف ثبت شده ( {discountName} )";
                            }
                        }
                        else
                        {
                            return $"برای دسته {productGroupName} قبلا تخفیف ثبت شده ( {discountName} )";
                        }
                    }
                }
            }
            #endregion

            #region Check for Duplicate Products
            if (newDiscount.ProductIds != null)
            {
                foreach (var item in newDiscount.ProductIds)
                {
                    if (discounts.Any(d => d.ProductId == item))
                    {
                        var productName = _productRepo.GetAll().FirstOrDefault(b => b.Id == item).Title;
                        var discountName = discounts.FirstOrDefault(d => d.ProductId == item).Title;
                        if (groupIdentifier != null)
                        {
                            if (discounts.FirstOrDefault(d => d.ProductId == item).GroupIdentifier != groupIdentifier)
                            {
                                return $"برای محصول {productName} قبلا تخفیف ثبت شده ( {discountName} )";
                            }
                        }
                        else
                        {
                            return $"برای محصول {productName} قبلا تخفیف ثبت شده ( {discountName} )";
                        }

                    }
                }
            }
            #endregion
            return "valid";
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var discount = _repo.Get(id.Value);
            if (discount == null)
            {
                return HttpNotFound();
            }
            return PartialView(discount);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var discount = _repo.Get(id);
            var discountGroup = _repo.GetDiscountGroup(id);

            var groupImage = discountGroup.Count > 0 ? discountGroup[0].Image : "";

            foreach (var item in discountGroup)
                _repo.Delete(item.Id);


            try
            {
                if (System.IO.File.Exists(Server.MapPath("/Files/DiscountImages/Image/" + groupImage)))
                    System.IO.File.Delete(Server.MapPath("/Files/DiscountImages/Image/" + groupImage));
            }
            catch
            {

            }

            return RedirectToAction("Index");
        }

        private DateTime ConvertPersianDateStrToDatetime(string strDatetime)
        {
            DateTime dt;

            PersianCalendar pc = new PersianCalendar();
            var strDate = strDatetime.Split(' ')[0];
            var strTime = strDatetime.Split(' ')[1];

            int year = int.Parse(strDate.Split('/')[0]);
            int month = int.Parse(strDate.Split('/')[1]);
            int day = int.Parse(strDate.Split('/')[2]);
            int hour = int.Parse(strTime.Split(':')[0]);
            int minute = int.Parse(strTime.Split(':')[1]);
            int second = 0;
            int millisecond = 0;
            dt = pc.ToDateTime(year, month, day, hour, minute, second, millisecond);


            return dt;
        }

        private string GetPersianDate(DateTime dtime)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();

            string date = pc.GetYear(dtime).ToString();
            date += "/";
            date += pc.GetMonth(dtime) < 10 ? "0" + pc.GetMonth(dtime) : pc.GetMonth(dtime).ToString();
            date += "/";
            date += pc.GetDayOfMonth(dtime) < 10 ? "0" + pc.GetDayOfMonth(dtime) : pc.GetDayOfMonth(dtime).ToString();

            date += " ";
            date += pc.GetHour(dtime) < 10 ? "0" + pc.GetHour(dtime) : pc.GetHour(dtime).ToString();
            date += ":";
            date += pc.GetMinute(dtime) < 10 ? "0" + pc.GetMinute(dtime) : pc.GetMinute(dtime).ToString();
            date += ":";
            date += pc.GetSecond(dtime) < 10 ? "0" + pc.GetSecond(dtime) : pc.GetSecond(dtime).ToString();

            return date;
        }


    }
}