using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BitakShop.Web.Providers
{
    public class SepehrGatewayRepository : IBankGatewayRepository
    {
        #region gateway info
        private readonly int TerminalCode = 69007688;
        private readonly long StoreCode = 693408594702204;
        private readonly string callbackUrl = ConfigurationManager.AppSettings["PaymentCallbackUrl"];
        #endregion

        public SepehrGatewayRepository()
        {
        }

        public RequestResponse SendInitialRequest(IDictionary<string, string> data)
        {
            var amount = decimal.Parse(data["Amount"]) * 10; // convert to Rial
            RequestResponse requestResponse = new RequestResponse();

            string url =
                string.Format(
                    "https://mabna.shaparak.ir:8081/V1/PeymentApi/GetToken?Amount={0}&CallbackUrl={1}&InvoiceId={2}&TerminalId={3}",
                    amount, callbackUrl, long.Parse(data["InvoiceNumber"]), TerminalCode);


            string result;
            HttpWebRequest request;
            WebResponse webResponse = null;
            try
            {
                request = WebRequest.Create(url.Remove(url.IndexOf("?", StringComparison.Ordinal))) as HttpWebRequest;

                if (request != null)
                {
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";


                    string parameters = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);

                    byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();

                    dataStream.Write(byteArray, 0, byteArray.Length);

                    dataStream.Close();


                    webResponse = request.GetResponse();
                }


                StreamReader reader = new StreamReader(webResponse?.GetResponseStream() ?? throw new InvalidOperationException());
                result = reader.ReadToEnd();

                reader.Close();
                requestResponse.RequestResponseMessage = result;
                requestResponse.RequestResponseType = RequestResponseType.OK;


            }
            catch (Exception exp)
            {

                requestResponse.RequestResponseType = RequestResponseType.Error;
                result = "Error";
            }
            finally
            {
                webResponse?.Close(); //Close webresponse connection
            }


            return requestResponse;
        }
        public string GetPaymentURL(string Token)
        {
            if (string.IsNullOrEmpty(Token))
                return "false";


            NameValueCollection collection = new NameValueCollection();
            collection.Add("Token", Token);
            string url = PreparePOSTForm("https://mabna.shaparak.ir:8080/Pay", collection);
            return url;
        }
        public RequestResponse CheckPeyment(System.Collections.Specialized.NameValueCollection parameters)
        {
            RequestResponse requestResponse = new RequestResponse();



            return requestResponse;
        }

        public RequestResponse VerifyPeyment(IDictionary<string, string> data)
        {
            RequestResponse requestResponse = new RequestResponse();

            string url =
                string.Format(
                    "https://mabna.shaparak.ir:8081/V1/PeymentApi/Advice?digitalreceipt={0}&Tid={1}",
                    data["digitalreceipt"], TerminalCode);


            string result;
            HttpWebRequest request;
            WebResponse webResponse = null;
            try
            {
                request = WebRequest.Create(url.Remove(url.IndexOf("?", StringComparison.Ordinal))) as HttpWebRequest;

                if (request != null)
                {
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";


                    string parameters = url.Substring(url.IndexOf("?", StringComparison.Ordinal) + 1);

                    byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();

                    dataStream.Write(byteArray, 0, byteArray.Length);

                    dataStream.Close();


                    webResponse = request.GetResponse();
                }


                StreamReader reader = new StreamReader(webResponse?.GetResponseStream() ?? throw new InvalidOperationException());
                result = reader.ReadToEnd();

                reader.Close();
                requestResponse.RequestResponseMessage = result;
                requestResponse.RequestResponseType = RequestResponseType.OK;


            }
            catch (Exception exp)
            {

                requestResponse.RequestResponseType = RequestResponseType.Error;
                result = "Error";
            }
            finally
            {
                webResponse?.Close(); //Close webresponse connection
            }


            return requestResponse;
        }

        public string GetGatewayMessage(int code)
        {
            string result = "";

            return result;
        }

        private string PreparePOSTForm(string url, NameValueCollection data)
        {
            string formID = "PaymentForm";
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\" >");
            strForm.Append("<input type=\"hidden\" name=\"TerminalID\" value=\"" + TerminalCode + "\" >");
            strForm.Append("<input type=\"hidden\" name=\"token\" value=\"" + data["Token"] + "\" >");
            strForm.Append("<input type=\"hidden\" name=\"getMethod\" value=\"" + "1" + "\" >");
            strForm.Append("</form>");

            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript' >");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            return strForm.ToString() + strScript.ToString();
        }

    }
}