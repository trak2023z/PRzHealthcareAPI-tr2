using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Models;
using System.Net.Http;

namespace PRzHealthcareAPI.Services
{
    public interface ICaptchaService
    {
        Task<string> ValidateCaptcha(CaptchaResponseDto captchaResponse);
    }

    public class CaptchaService : ICaptchaService
    {
        private readonly HttpClient _httpClient;

        public CaptchaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ValidateCaptcha(CaptchaResponseDto captchaResponse)
        {
            try
            {
                // 1. Extract the CAPTCHA response from the request parameters.

                // 2. Send a request to the CAPTCHA validation service to verify the response.
                var validationEndpoint = "https://www.google.com/recaptcha/api/siteverify";
                var secret = "6LfJZt8lAAAAAJBdjkP_SsJE169C6ct0S-nR0Gp8";
                var requestBody = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("secret", secret),
            new KeyValuePair<string, string>("response", captchaResponse.Key)
        });
                var response = await _httpClient.PostAsync(validationEndpoint, requestBody);

                // 3. Depending on the validation result, return a response to the client indicating whether the CAPTCHA was validated successfully or not.
                var responseContent = await response.Content.ReadAsStringAsync();
                var validationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptchaValidationResponse>(responseContent);
                if (validationResponse.Success)
                {
                    return "Ok";
                }
                else
                {
                    throw new BadRequestException("CAPTCHA validation failed.");
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

        }
    }
}
