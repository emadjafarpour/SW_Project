using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BitakShop.Web.Providers
{
    public class PasargadGatewayRepository : IBankGatewayRepository
    {

        #region gateway info
        private readonly int TerminalCode = 0;
        private readonly int StoreCode = 0;
        private readonly string RSAKey = "<RSAKeyValue><Modulus>sXfJUA8CowVOBx90bPlUBFMFzFyTG2zA50owtbkKNnSS2kNhuGNnOFJ89hrOPXT3TGlJsd5YWnhYEHh9D/0CTUrVPfZUdf9n40rvG94tMpDECGX6exdbmwNzQCuSQ0CSdouOU0uZO6d5M22v415oOqrR0jpWCqfj7chalOwFZx8=</Modulus><Exponent>AQAB</Exponent><P>5+QOaK4abPxy4VIiyvmmdSQDWE3BuVBts+3wpnvjcpwpftvc54eDS1KhLn0j1wzy3a8v/mlc7I7bXevpdTPrbQ==</P><Q>w+s5ZM+NsHJ7+G5qZnVUMStYnMLUUtyBjd4pXZPUzEprZWbqDV1yMPGdoYrVrIrC2KqXReggwOrJhpUKJIKZOw==</Q><DP>ssCa1ti2qCKmD8i3500wooXjSjOOTOKR3ixh4IQJnXSBjDfBfnx4mhyVAPgYI5LouDhuP6hKqFOrCChtqxodtQ==</DP><DQ>sp50Vyq7fVEDIX5ZV0xFFAb25QTj2x/apeMUR5KOAisfOvXXEZROlbzTAAK5yHNCLZNqE3wM0JW+YgrjkmHXHQ==</DQ><InverseQ>zl4lDRvSbjrFM7vovmpuL67W4TiE+AFAWHKo/d6KMWep9X+L7L6Mad84GS7TMekT9/zbkbfEnEZJ1ShZma0sAQ==</InverseQ><D>njsgeko33qQ/MuifxP31JDnmBJVR8HTh9hbXIV/PtEfyG7tSaLXwdW5OzpuN4YvlZvLFa1rEyj1cv6q+T0AWCNjL1vQIud7OOxyjF0D8BRz9VDcExFxZC3kJkGJP6aLe3r5sBL/V0m55s9TRvxeDLdhPI+7AlfY8R6soAsdBYHk=</D></RSAKeyValue>";
        private readonly string callbackUrl = ConfigurationManager.AppSettings["PaymentCallbackUrl"];
        #endregion

        public PasargadGatewayRepository()
        {
        }

        /// <summary>
        /// SendInitialRequest
        /// </summary>
        /// <param name="data">name value dictionary. needs InvoiceNumber, InvoiceDate, OrderId, Mobile, Email </param>
        /// 
        public RequestResponse SendInitialRequest(IDictionary<string, string> data)
        {
            var amount = decimal.Parse(data["Amount"]) * 10; // convert to Rial
            RequestResponse requestResponse = new RequestResponse();

            var url = callbackUrl;//(callbackUrl + "?invoiceNumber=" + data["InvoiceNumber"]);

            var sendingData = "{ \"InvoiceNumber\": \"" + data["InvoiceNumber"] +
                "\", \"InvoiceDate\":\"" + new PersianDateTime(DateTime.Parse(data["InvoiceDate"])).ToString("yyyy/MM/dd") +
                "\",\"TerminalCode\": \"" + TerminalCode +
                "\", \"MerchantCode\": \"" + StoreCode +
                "\", \"Amount\":\"" + amount
                + "\",\"RedirectAddress\":\"" + url +
                "\",\"Timestamp\":\"" + data["Timestamp"] +
                "\", \"Action\":\"" + 1003 +
                "\",\"Mobile\":\"" + data["Mobile"] +
                "\", \"Email\":\"" + data["Email"] + "\" }";

            var content = new StringContent(sendingData, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://pep.shaparak.ir/Api/v1/Payment/GetToken"),
                Method = HttpMethod.Post,
                Content = content
            };
            request.Headers.Add("Sign", GetSign(sendingData));
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.SendAsync(request).Result;
            var result = Encoding.UTF8.GetString(response.Content.ReadAsByteArrayAsync().Result);

            requestResponse.RequestResponseMessage = result;
            requestResponse.RequestResponseType = RequestResponseType.OK;

            return requestResponse;
        }
        public string GetPaymentURL(string Token)
        {
            if (string.IsNullOrEmpty(Token))
                return "false";


            NameValueCollection collection = new NameValueCollection();
            collection.Add("Token", Token);
            string url = PreparePOSTForm("https://pep.shaparak.ir/payment.aspx", collection);
            return url;
        }
        public RequestResponse CheckPeyment(System.Collections.Specialized.NameValueCollection parameters)
        {
            RequestResponse requestResponse = new RequestResponse();


            string TransactionReferenceID = parameters["tref"];//TransactionReferenceID
            string InvoiceNumber = parameters["iN"];//InvoiceNumber
            string InvoiceDate = parameters["iD"];//InvoiceDate

            var sendingData = "";
            if (!string.IsNullOrEmpty(TransactionReferenceID))
            {
                sendingData = "{ \"TransactionReferenceID\": \"" + TransactionReferenceID + "\" }";
            }
            else
            {
                sendingData = "{ \"InvoiceNumber\": \"" + InvoiceNumber +
                "\", \"InvoiceDate\":\"" + InvoiceDate +
                "\",\"TerminalCode\": \"" + TerminalCode +
                "\", \"MerchantCode\": \"" + StoreCode + "\" }";
            }


            var content = new StringContent(sendingData, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://pep.shaparak.ir/Api/v1/Payment/CheckTransactionResult"),
                Method = HttpMethod.Post,
                Content = content
            };
            request.Headers.Add("Sign", GetSign(sendingData));
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.SendAsync(request).Result;
            var result = Encoding.UTF8.GetString(response.Content.ReadAsByteArrayAsync().Result);

            requestResponse.RequestResponseMessage = result;
            requestResponse.RequestResponseType = RequestResponseType.OK;



            return requestResponse;
        }

        public RequestResponse VerifyPeyment(IDictionary<string, string> data)
        {
            RequestResponse requestResponse = new RequestResponse();
            var amount = decimal.Parse(data["Amount"]) * 10; // convert to Rial


            string TransactionReferenceID = data["tref"];
            string InvoiceNumber = data["iN"];
            string InvoiceDate = data["iD"];

            var sendingData = "";
            sendingData = "{ \"InvoiceNumber\": \"" + InvoiceNumber +
                "\", \"InvoiceDate\":\"" + InvoiceDate +
                "\",\"TerminalCode\": \"" + TerminalCode +
                "\", \"MerchantCode\": \"" + StoreCode +
                "\",\"Amount\": \"" + amount +
                "\",\"TimeStamp\": \"" + data["Timestamp"] + "\" }";



            var content = new StringContent(sendingData, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://pep.shaparak.ir/Api/v1/Payment/VerifyPayment"),
                Method = HttpMethod.Post,
                Content = content
            };
            request.Headers.Add("Sign", GetSign(sendingData));
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.SendAsync(request).Result;
            var result = Encoding.UTF8.GetString(response.Content.ReadAsByteArrayAsync().Result);

            requestResponse.RequestResponseMessage = result;
            requestResponse.RequestResponseType = RequestResponseType.OK;



            return requestResponse;
        }

        public string GetGatewayMessage(int code)
        {
            string result = "";

            return result;
        }

        private string GetSign(string data)
        {
            var cs = new CspParameters { KeyContainerName = "PaymentTest" };
            var rsa = new RSACryptoServiceProvider(cs) { PersistKeyInCsp = false };
            rsa.Clear();
            rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(RSAKey);
            byte[] signMain = rsa.SignData(Encoding.UTF8.GetBytes(data), new
            SHA1CryptoServiceProvider());
            string sign = Convert.ToBase64String(signMain);
            return sign;
        }

        private string PreparePOSTForm(string url, NameValueCollection data)
        {
            string formID = "PaymentForm";
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\" >");
            strForm.Append("<input type=\"hidden\" name=\"Token\" value=\"" + data["Token"] + "\" >");
            strForm.Append("</form>");

            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript' >");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            return strForm.ToString() + strScript.ToString();
        }


        private struct PasargadResponse
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public string Token { get; set; }
        }

    }

}