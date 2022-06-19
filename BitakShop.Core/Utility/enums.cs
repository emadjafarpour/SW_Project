using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Utility
{
    public enum DiscountType
    {
        Percentage = 1,
        Amount = 2
    }
    public enum GeoDivisionType
    {
        Country = 0,
        State = 1,
        City = 2,
    }

    public enum StaticContentTypes
    {
        Slider = 1,
        ContactUsMap = 2,
        Address = 6,
        Email = 7,
        Phone = 8,
        Guide = 9,
        SidebarImages = 10,
        AboutUs = 11,
        CollectionBanner = 12,
        SocialMedia = 13
    }

    public enum DiscountLocationType
    {
        Header = 1,
        Sidebar = 2,
        Body1 = 3,
        Body2 = 4,
        Body3 =5
    }

    public enum PaymentStatus
    {
        Unprocessed = 1,
        Failed = 2,
        Succeed = 3,
        Expired = 4
    }


}
