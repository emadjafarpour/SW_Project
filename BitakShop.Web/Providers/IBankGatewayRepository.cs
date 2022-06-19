using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitakShop.Web.Providers
{
    public interface IBankGatewayRepository
    {
        RequestResponse SendInitialRequest(IDictionary<string, string> data);
        string GetPaymentURL(string paymentId);
        RequestResponse CheckPeyment(System.Collections.Specialized.NameValueCollection parameters);
        RequestResponse VerifyPeyment(IDictionary<string, string> data);
        string GetGatewayMessage(int code);
    }

    public class RequestResponse
    {
        public RequestResponseType RequestResponseType { get; set; }
        public int ResponseCode { get; set; }
        public string RequestResponseMessage { get; set; }
        public long PaymentID { get; set; }
        public string RedirectUrl { get; set; }
    }
    public enum RequestResponseType { OK, Error }

}