using Microsoft.AspNetCore.Mvc;
using ScpmaBe.Services.Common;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.WebApi.Helpers;

namespace ScpmaBe.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            AppSettings appSettings,
            IContractService contractService,
            ILogger<PaymentController> logger)
        {
            _appSettings = appSettings;
            _contractService = contractService;
            _logger = logger;
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreatePayment(int paymentContractId, string platform = "mobile")
        {
            _logger.LogInformation("Creating payment for contract ID: {PaymentContractId}, Platform: {Platform}",
                paymentContractId, platform);

            try
            {
                var paymentContract = await _contractService.GetPaymentContract(paymentContractId);

                if (paymentContract == null || paymentContract.Status != (int)PaymentContractStatus.Approved)
                {
                    _logger.LogWarning("Payment contract not found or not approved. ID: {PaymentContractId}, Status: {Status}",
                        paymentContractId, paymentContract?.Status);
                    return NotFound("Không tìm thấy hợp đồng thanh toán");
                }

                // Calculate total amount in VND (multiply by 100 for VNPay format)
                var amount = (long)(paymentContract.PaymentAmount * 100);

                var vnp_Returnurl = $"{_appSettings.ApplicationUrl}/api/payment/ipn?platform={platform}";
                var vnp_Url = _appSettings.PaymentSettings.PaymentBaseUrl;

                var vnp_TmnCode = _appSettings.PaymentSettings.TmnCode;
                var vnp_HashSecret = _appSettings.PaymentSettings.HashSecretKey;

                VnPayLibrary vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", amount.ToString());
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToVNTime().ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán hợp đồng: #{paymentContractId}");
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", paymentContractId.ToString());
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");

                string paymentUrl = $"{vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret)}";

                _logger.LogInformation("Payment URL created successfully for contract ID: {PaymentContractId}, Amount: {Amount}, PaymentUrl: {URL}",
                    paymentContractId, 
                    paymentContract.PaymentAmount,
                    paymentUrl
                    );

                return Redirect(paymentUrl); // mobile WebView sẽ được mở link này
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for contract ID: {PaymentContractId}", paymentContractId);
                throw;
            }
        }

        [HttpGet("ipn")]
        public async Task<IActionResult> PaymentIPN(string platform)
        {
            _logger.LogInformation("Payment IPN callback received. Platform: {Platform}", platform);

            try
            {
                var vnp_HashSecret = _appSettings.PaymentSettings.HashSecretKey;
                VnPayLibrary vnpay = new VnPayLibrary();
                var requestData = Request.Query;

                // Log request data for debugging
                _logger.LogDebug("VNPay IPN request data: {@RequestData}",
                    requestData.ToDictionary(x => x.Key, x => x.Value.ToString()));

                foreach (var key in requestData.Keys)
                {
                    vnpay.AddResponseData(key, requestData[key]);
                }

                string vnp_SecureHash = requestData["vnp_SecureHash"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                string txnRef = requestData["vnp_TxnRef"];

                //if (!checkSignature)
                //{
                //    _logger.LogWarning("Invalid VNPay signature for transaction: {TxnRef}", txnRef);
                //}

                var htmlContent = @"
                                        <html>
                                        <head><meta charset='UTF-8'><title>Redirect</title>
                                        <script>
                                            setTimeout(function() {
                                                window.location.href = '[URL]';
                                            }, 1000);
                                        </script>
                                        </head>
                                        <body>Đang xử lý giao dịch... vui lòng đợi.</body>
                                        </html>";

                string responseCode = requestData["vnp_ResponseCode"];
                string amount = requestData["vnp_Amount"];

                _logger.LogInformation("VNPay transaction: {TxnRef}, Response Code: {ResponseCode}",
                    txnRef, responseCode);

                if (responseCode == "00")
                {
                    _logger.LogInformation("Payment successful for transaction: {TxnRef}, Amount: {Amount}",
                        txnRef, amount);

                    await _contractService.PayPaymentContract(int.Parse(txnRef), "VNPay");

                    htmlContent = htmlContent.Replace("[URL]", platform == "mobile"
                        ? $"myapp://payment-success?paymentContractId={txnRef}"
                        : $"{_appSettings.ApplicationUrl}/api/payment/success?id={txnRef}");

                    return Content(htmlContent, "text/html");
                }

                _logger.LogWarning("Payment failed for transaction: {TxnRef}, Response Code: {ResponseCode}",
                    txnRef, responseCode);

                htmlContent = htmlContent.Replace("[URL]", platform == "mobile"
                    ? $"myapp://payment-error?paymentContractId={txnRef}&errorCode=${responseCode}"
                    : $"{_appSettings.ApplicationUrl}/api/payment/error?id={txnRef}&errorCode={responseCode}");

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment IPN callback");
                throw;
            }
        }

        [HttpGet("error")]
        public IActionResult PaymentErorr(string errorCode)
        {
            _logger.LogInformation("Payment error page requested. Error code: {ErrorCode}", errorCode ?? "Unknown");

            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static", "payment-error.html");
                string htmlContent = System.IO.File.ReadAllText(filePath);

                // Replace error code placeholder if it exists
                htmlContent = htmlContent.Replace("{{ERROR_CODE}}", errorCode ?? "Unknown");

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving payment error page. Error code: {ErrorCode}", errorCode);

                // Fallback if file not found or other error
                return Content($"<html><body><h1>Payment Error</h1><p>Error code: {errorCode}</p></body></html>", "text/html");
            }
        }

        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess(string id)
        {
            _logger.LogInformation("Payment success page requested. Payment ID: {PaymentId}", id);

            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static", "payment-success.html");
                string htmlContent = System.IO.File.ReadAllText(filePath);

                // Get payment details if needed
                if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int paymentContractId))
                {
                    var paymentContract = await _contractService.GetPaymentContract(paymentContractId);
                    if (paymentContract != null)
                    {
                        _logger.LogInformation("Payment contract found for ID: {PaymentId}, Amount: {Amount}",
                            paymentContractId, paymentContract.PaymentAmount);

                        htmlContent = htmlContent.Replace("{{PAYMENT_ID}}", paymentContractId.ToString());
                        htmlContent = htmlContent.Replace("{{PAYMENT_AMOUNT}}", paymentContract.PaymentAmount.ToString("N0"));
                        htmlContent = htmlContent.Replace("{{PAYMENT_DATE}}", paymentContract.PaymentDate?.ToString("dd/MM/yyyy HH:mm:ss") ?? "");
                    }
                    else
                    {
                        _logger.LogWarning("Payment contract not found for ID: {PaymentId}", paymentContractId);
                    }
                }

                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving payment success page. Payment ID: {PaymentId}", id);

                // Fallback if file not found or other error
                return Content("<html><body><h1>Payment Successful</h1><p>Your payment has been processed successfully.</p></body></html>", "text/html");
            }
        }

        [HttpGet("Histories")]
        public async Task<IActionResult> GetPaymentHistories(int customerId)
        {
            var result = await _contractService.GetPaymentHistories(customerId);

            return Ok(result);
        }
    }
}