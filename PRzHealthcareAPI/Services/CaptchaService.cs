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

        /// <summary>
        /// Walidacja Captchy
        /// </summary>
        /// <param name="captchaResponse">Klucz witryny</param>
        /// <returns>Wynik walidacji captcha</returns>
        /// <exception cref="BadRequestException">Błąd podczas wysyłania żądania</exception>
        public async Task<string> ValidateCaptcha(CaptchaResponseDto captchaResponse)
        {
            try
            {
                var validationEndpoint = "https://www.google.com/recaptcha/api/siteverify";
                var secret = "6LftZ-wlAAAAAA6DnXWdjk8LbaLb5wGqCUrIRgII";
                var requestBody = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("secret", secret),
            new KeyValuePair<string, string>("response", captchaResponse.Key)
        });
                var response = await _httpClient.PostAsync(validationEndpoint, requestBody);

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
