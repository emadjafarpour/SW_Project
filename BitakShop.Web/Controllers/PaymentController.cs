using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Infratructure.Services;
using BitakShop.Web.Providers;
using BitakShop.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BitakShop.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly InvoicesRepository _invoicesRepository;
        private readonly CustomersRepository _customerRepo;
        private readonly EPaymentRepository _ePaymentRepo;
        private readonly EPaymentLogRepository _ePaymentLogRepo;
        private readonly ProductService _productService;
        private readonly StaticContentDetailsRepository _staticContentRepo;

        public PaymentController(InvoicesRepository invoicesRepository, CustomersRepository customersRepository,
            EPaymentRepository ePaymentRepository,
            EPaymentLogRepository ePaymentLogRepository,
            ProductService productService,
            StaticContentDetailsRepository staticContentDetailsRepository)
        {
            _invoicesRepository = invoicesRepository;
            _customerRepo = customersRepository;
            _ePaymentRepo = ePaymentRepository;
            _ePaymentLogRepo = ePaymentLogRepository;
            _productService = productService;
            _staticContentRepo = staticContentDetailsRepository;
        }

        // GET: Payment
        public ActionResult Index()
        {
            return Redirect("/Shop/Checkout");
        }

        [CustomerAuthorize]
        [HttpPost]
        public ActionResult InitPay(FormCollection collection)
        {
            var invoiceNumber = collection["InvoiceNumber"];

            if (string.IsNullOrEmpty(invoiceNumber))
            {
                return Redirect("/Customer/Dashboard/Invoices");
            }

            Invoice invoice = _invoicesRepository.GetInvoice(invoiceNumber);
            var customer = _customerRepo.GetCurrentCustomer();


            // prevent two payment for a same invoice at the same time
            var prevEPayment = _ePaymentRepo.GetInvoiceLatestUnprocessedEPayment(invoice.Id);
            if (prevEPayment != null)
            {
                if (DateTime.Now.Subtract(prevEPayment.InsertDate.Value).TotalMinutes > 10)
                {
                    // expire the payment and continue
                    prevEPayment.PaymentStatus = PaymentStatus.Failed;
                    var ePayLog = new EPaymentLog();
                    ePayLog.Message = "پایان اعتبار پرداخت پس از سپری شدن بیش از 10 دقیقه";
                    ePayLog.LogDate = DateTime.Now;
                    ePayLog.LogType = "منقضی شد";
                    ePayLog.MethodName = "/Payment/InitPay";
                    ePayLog.Amount = ePayLog.Amount;
                    ePayLog.Token = ePayLog.Token;
                    ePayLog.InsertDate = DateTime.Now;
                    ePayLog.InsertUser = customer.User.UserName;


                    prevEPayment.PaymentStatus = PaymentStatus.Failed;
                    if (prevEPayment.EPaymentLogs == null)
                        prevEPayment.EPaymentLogs = new List<EPaymentLog>();
                    prevEPayment.EPaymentLogs.Add(ePayLog);
                    _ePaymentRepo.Update(prevEPayment);

                }
                else
                {
                    return Redirect("/Payment/Error");
                }
            }


            // adding an ePayment
            EPayment ePayment = new EPayment();
            ePayment.InvoiceId = invoice.Id;
            ePayment.Amount = invoice.TotalPrice;
            ePayment.Description = "پرداخت هزینه سفارش از فروشگاه بی تک شاپ";
            ePayment.Token = "";
            ePayment.ExtraInfo = "تکمیل نشده";
            ePayment.PaymentStatus = PaymentStatus.Unprocessed;
            ePayment.InsertUser = customer.User.UserName;
            ePayment.InsertDate = DateTime.Now;
            ePayment.PaymentAccountId = _ePaymentRepo.GetPaymentAccountId();

            _ePaymentRepo.Add(ePayment);


            // logging initial bank response
            IBankGatewayRepository bankGatewayRepository = new SepehrGatewayRepository();
            IDictionary<string, string> data = new Dictionary<string, string>();
            data.Add("InvoiceNumber", ePayment.Id.ToString()); // sending epayment Id as invoice number
            data.Add("InvoiceDate", ePayment.InsertDate.ToString());
            data.Add("Amount", invoice.TotalPrice.ToString());
            data.Add("Mobile", invoice.Phone);
            data.Add("Email", invoice.Email);
            data.Add("Timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            var result = bankGatewayRepository.SendInitialRequest(data);
            var bankResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BankResult>(result.RequestResponseMessage);

            // adding an epayment log
            var ePaymentLog = new EPaymentLog();
            ePaymentLog.Message = bankResult.Message;
            ePaymentLog.LogDate = DateTime.Now;
            ePaymentLog.LogType = "تایید سفارش و دریافت توکن پرداخت";
            ePaymentLog.MethodName = "/Payment/InitPay";
            ePaymentLog.Amount = ePayment.Amount;
            ePaymentLog.Token = bankResult.AccessToken;
            ePaymentLog.InsertDate = DateTime.Now;
            ePaymentLog.InsertUser = customer.User.UserName;
            ePaymentLog.AdditionalData = "IsSuccess=" + bankResult.IsSuccess + "- Status=" + bankResult.Status + "- Message=" + bankResult.Message + "- Token=" + (bankResult.AccessToken ?? "");

            ePayment.EPaymentLogs = new List<EPaymentLog>();
            ePayment.EPaymentLogs.Add(ePaymentLog);
            _ePaymentRepo.Update(ePayment);


            if (bankResult.Status == 0)//succeed
            {
                ePayment.Token = bankResult.AccessToken;
                _ePaymentRepo.Update(ePayment);

                var url = bankGatewayRepository.GetPaymentURL(bankResult.AccessToken);
                Response.Write(url);
            }
            else
            {
                return Redirect("/Payment/Error");
            }


            return View();

        }


        public ActionResult Callback()
        {
            var parameters = Request.QueryString;

            //parameters = Request.Form; // sepehr sends data using post method

            //parameters = new System.Collections.Specialized.NameValueCollection() { { "respcode", "0" }, { "digitalreceipt", "0" }, { "invoiceid", "13" } };
            if (string.IsNullOrEmpty(parameters["respcode"]) && string.IsNullOrEmpty(parameters["digitalreceipt"]))//TransactionReferenceID
            {
                return Redirect("/Payment/Error");
            }

            //Response.Write(parameters);

            // check if transaction succeed
            var customer = _customerRepo.GetCustomerUsingPaymentId(int.Parse(parameters["invoiceid"]));
            if (customer == null) // in case admin tries to buy something
                return Redirect("/Customer/Auth/Register");

            IBankGatewayRepository bankGatewayRepository = new SepehrGatewayRepository();

            if (int.Parse(parameters["respcode"]) == 0)//succeed
            {
                int paymentId = int.Parse(parameters["invoiceid"]);// this "InvoiceNumber" is actually the payment id
                var invoice = _invoicesRepository.GetInvoiceByPaymentId(paymentId);
                // getting invoice latest payment
                var payment = _ePaymentRepo.Get(paymentId);


                // invoice number and invoice date are already present in "parameters"
                IDictionary<string, string> data = new Dictionary<string, string>();
                data.Add("digitalreceipt", parameters["digitalreceipt"]);


                var result = bankGatewayRepository.VerifyPeyment(data);

                var verifyRes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(result.RequestResponseMessage);
                Response.Write(verifyRes);

                if (verifyRes["Status"].ToLower().Trim() == "ok")
                {
                    // log result & update payment
                    var ePaymentLog = new EPaymentLog();
                    ePaymentLog.Message = verifyRes["Message"];
                    ePaymentLog.LogDate = DateTime.Now;
                    ePaymentLog.LogType = "تایید پرداخت موفق";
                    ePaymentLog.MethodName = "/Payment/Confirm";
                    ePaymentLog.Amount = payment.Amount;
                    ePaymentLog.Token = "";
                    ePaymentLog.InsertDate = DateTime.Now;
                    ePaymentLog.InsertUser = customer.User.UserName;
                    ePaymentLog.StackTraceNo = parameters["tracenumber"];
                    ePaymentLog.AdditionalData = "digitalreceipt=" + parameters["digitalreceipt"];

                    if (payment.EPaymentLogs == null)
                        payment.EPaymentLogs = new List<EPaymentLog>();
                    payment.EPaymentLogs.Add(ePaymentLog);
                    payment.SystemTraceNo = parameters["tracenumber"];
                    payment.PaymentStatus = PaymentStatus.Succeed;
                    payment.RetrievalRefNo = parameters["invoiceid"];
                    payment.ExtraInfo = parameters["cardnumber"]; // شماره کارت مسک شده کار
                    payment.Description = verifyRes["Message"]; 
                    _ePaymentRepo.Update(payment);


                    // update order payed status
                    invoice.IsPayed = true;
                    _invoicesRepository.Update(invoice);



                    // update stock status
                    foreach (var item in invoice.InvoiceItems)
                    {
                        _productService.DecreaseStockProductCount(item.ProductId, item.MainFeatureId, item.Quantity);
                    }


                    return Redirect("/Payment/Succeed/?invoiceNumber=" + invoice.InvoiceNumber);
                }
                else
                {
                    // log result & expire payment
                    var ePaymentLog = new EPaymentLog();
                    ePaymentLog.Message = verifyRes["Message"];
                    ePaymentLog.LogDate = DateTime.Now;
                    ePaymentLog.LogType = "تایید پرداخت شکست خورد";
                    ePaymentLog.MethodName = "/Payment/Confirm";
                    ePaymentLog.Amount = payment.Amount;
                    ePaymentLog.Token = payment.Token;
                    ePaymentLog.InsertDate = DateTime.Now;
                    ePaymentLog.InsertUser = customer.User.UserName;
                    ePaymentLog.AdditionalData = "IsSuccess=" + verifyRes["Status"] + "- Message" + verifyRes["Message"] + " - ReturnId" + (verifyRes["ReturnId"] ?? "");

                    payment.PaymentStatus = PaymentStatus.Failed;
                    if (payment.EPaymentLogs == null)
                        payment.EPaymentLogs = new List<EPaymentLog>();
                    payment.EPaymentLogs.Add(ePaymentLog);
                    payment.PaymentStatus = PaymentStatus.Failed;
                    _ePaymentRepo.Update(payment);

                    // redirect
                    return Redirect("/Payment/Error");
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(parameters["iN"]))
                {
                    var paymentId = int.Parse(parameters["iN"]);
                    var payment = _ePaymentRepo.Get(paymentId);
                    payment.PaymentStatus = PaymentStatus.Failed;
                    _ePaymentRepo.Update(payment);
                }

                return Redirect("/Payment/Error");
            }


        }



        public ActionResult Error()
        {
            return View();
        }



        public ActionResult Succeed()
        {
            string invoiceNumber = !string.IsNullOrEmpty(Request["invoiceNumber"]) ? Request["invoiceNumber"] : "";


            if (string.IsNullOrEmpty(invoiceNumber))
                return Redirect("/");

            var invoice = _invoicesRepository.GetInvoice(invoiceNumber);
            if (invoice == null)
                invoice = new Invoice();

            return View(invoice);
        }

    }


}