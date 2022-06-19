using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitakShop.Core.Models;

namespace BitakShop.Web.ViewModels
{
    public class NewProductGroupViewModel
    {
        public string Title { get; set; }
        public List<int> BrandIds { get; set; }
        public int ParentGroupId { get; set; }
        public List<int> ProductGroupFeatureIds { get; set; }
    }
    public class UpdateProductGroupViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> BrandIds { get; set; }
        public int ParentGroupId { get; set; }
        public List<int> ProductGroupFeatureIds { get; set; }
    }

    public class FeaturesObjViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class BrandsObjViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SubFeaturesObjViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
    public class ProductListViewModel
    {
        public int? SelectedGroupId { get; set; }
        public int ProductCount { get; set; }
        public List<ProductGroup> ProductGroups { get; set; }
        public List<Feature> Features { get; set; }
        public List<Brand> Brands { get; set; }

    }
}