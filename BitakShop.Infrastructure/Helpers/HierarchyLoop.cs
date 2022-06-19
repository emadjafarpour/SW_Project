using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitakShop.Core.Models;

namespace BitakShop.Infrastructure.Helpers
{
    public static class HierarchyLoop
    {
        public static string GetProductGroupHierarchy(ProductGroup entity, int? selectedItemParent = null, int? selectedItem = null)
        {
            selectedItemParent = selectedItemParent ?? 0;
            selectedItem = selectedItem ?? 0;
            var content = string.Empty;
            if (entity.Id == selectedItemParent)
                content += "<li data-jstree='{ \"selected\" : true,\"opened\": true }' id=\"pg_" + entity.Id + "\">" + entity.Title;
            else if (entity.Id == selectedItem)
                content += "<li data-jstree='{ \"disabled\" : true }' id=\"pg_" + entity.Id + "\">" + entity.Title;
            else
                content += $"<li id='pg_{entity.Id}'>{entity.Title}";

            if (entity.Children.Any())
            {
                entity.Children.ToList().ForEach(item =>
                {
                    if (item.IsDeleted == false)
                    {
                        content += "<ul>" + GetProductGroupHierarchy(item, selectedItemParent, selectedItem) + "</ul>";
                    }
                });
            }
            content += "</li>";

            return content;
        }
        public static string GetProductGroupHierarchyForProducts(ProductGroup entity, int? selectedItemParent = null, int? selectedItem = null)
        {
            selectedItemParent = selectedItemParent ?? 0;
            selectedItem = selectedItem ?? 0;
            var content = string.Empty;
            if (entity.Id == selectedItemParent)
                content += "<li onclick=\"getProductGroupFeatures("+entity.Id+")\" data-jstree='{ \"selected\" : true,\"opened\": true }' id=\"pg_" + entity.Id + "\">" + entity.Title;
            else if (entity.Id == selectedItem)
                content += "<li onclick=\"getProductGroupFeatures(" + entity.Id + ")\" data-jstree='{ \"disabled\" : true }' id=\"pg_" + entity.Id + "\">" + entity.Title;
            else
                content += $"<li onclick=\"getProductGroupFeatures({entity.Id})\" id='pg_{entity.Id}'>{entity.Title}";

            if (entity.Children.Any())
            {
                entity.Children.ToList().ForEach(item =>
                {
                    if (item.IsDeleted == false)
                    {
                        content += "<ul>" + GetProductGroupHierarchyForProducts(item, selectedItemParent, selectedItem) + "</ul>";
                    }
                });
            }
            content += "</li>";

            return content;
        }
    }
}
