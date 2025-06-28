using Newtonsoft.Json;
using PassionStore.Infrastructure.Settings;
using System.Net.Http.Headers;
using System.Text;
using Net.payOS;
using PassionStore.Infrastructure.Externals.Payos.Models;
using Net.payOS.Types;
using Microsoft.Extensions.Logging;

namespace PassionStore.Infrastructure.Externals.Payos
{
    public class PayOSService
    {
        private readonly PayOSOption _payOSOption;
        private readonly PayOS _payOS;

        public PayOSService(PayOSOption payOSOption)
        {
            _payOSOption = payOSOption ?? throw new ArgumentNullException(nameof(payOSOption));
            _payOS = new PayOS(_payOSOption.ClientId, _payOSOption.APIKey, _payOSOption.ChecksumKey);
        }

        public async Task<PayOSDetailResponse> CreatePaymentAsync(PayOSRequest payOSRequest)
        {
            var signedRequest = payOSRequest.WithSignature(_payOSOption.ChecksumKey, _payOSOption.ReturnUrl, _payOSOption.CancelUrl);

            var requestBody = new
            {
                amount = signedRequest.Amount,
                description = signedRequest.Description,
                orderCode = signedRequest.OrderCode,
                items = signedRequest.Items,
                returnUrl = _payOSOption.ReturnUrl,
                cancelUrl = _payOSOption.CancelUrl,
                currency = "VND", // Include if required by API
                signature = signedRequest.Signature
            };

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("x-client-id", _payOSOption.ClientId);
                    client.DefaultRequestHeaders.Add("x-api-key", _payOSOption.APIKey);

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody, Formatting.None), Encoding.UTF8, "application/json");
                    Console.WriteLine($"Request body: {JsonConvert.SerializeObject(requestBody, Formatting.None)}"); // Debug log
                    var response = await client.PostAsync(_payOSOption.BaseUrl + "/v2/payment-requests", jsonContent);

                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response: {responseContent}"); // Debug log

                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<PayOSResponse<PayOSDetailResponse>>(responseContent);

                        if (result != null && result.Code.Equals("00"))
                        {
                            return result.Data;
                        }

                        throw new PayOSException(result?.Code.ToString() ?? "UNKNOWN", $"Payment failed: {responseContent}");
                    }

                    throw new PayOSException(response.StatusCode.ToString(), $"Payment request failed with status {response.StatusCode}: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task CancelOrder(long orderCode, string? reason)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-client-id", _payOSOption.ClientId);
                client.DefaultRequestHeaders.Add("x-api-key", _payOSOption.APIKey);
                var requestBody = new
                {
                    cancellationReason = reason ?? "No reason provided"
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody, Formatting.None), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_payOSOption.BaseUrl}/v2/payment-requests/{orderCode}/cancel", jsonContent);

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<PayOSResponse<object>>(responseContent);
                    if (result != null && result.Code.Equals("00"))
                    {
                        return; // Successfully cancelled
                    }
                    throw new PayOSException(result?.Code.ToString() ?? "UNKNOWN", $"Cancellation failed: {responseContent}");
                }
                else
                {
                    throw new PayOSException(response.StatusCode.ToString(), $"Cancellation request failed with status {response.StatusCode}: {responseContent}");

                }
            }

        }

        public async Task<PayOSInformationResponse?> PaymentCallBack(string code, string id, bool cancel, string status, long orderCode)
        {
           
            if (code.Equals("00"))
            {
                var paymentInfo = await GetPaymentInfomation(orderCode);
                if (paymentInfo != null && paymentInfo.Status.Equals("PAID"))
                {
                    return paymentInfo; 
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<PayOSInformationResponse?> GetPaymentInfomation(long orderCode)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-client-id", _payOSOption.ClientId);
                client.DefaultRequestHeaders.Add("x-api-key", _payOSOption.APIKey);
                var response = await client.GetAsync($"{_payOSOption.BaseUrl}/v2/payment-requests/{orderCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<PayOSResponse<PayOSInformationResponse>>(responseContent);
                    if (result != null && result.Code.Equals("00"))
                    {
                        return result.Data;
                    }
                    throw new PayOSException(result?.Code.ToString() ?? "UNKNOWN", $"Get payment information failed: {responseContent}");
                }
                else
                {
                    throw new PayOSException(response.StatusCode.ToString(), $"Get payment information request failed with status {response.StatusCode}: {responseContent}");
                }
            }
        }

        public class PayOSException : Exception
        {
            public string Code { get; }
            public PayOSException(string code, string message) : base(message) => Code = code;
        }
    }
}