using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Services;

namespace PRzHealthcareAPI.Controllers
{
    [Route("captcha")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly ICaptchaService _captchaService;

        public CaptchaController(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        [HttpPost]
        public async Task<string> ValidateCaptcha([FromBody] CaptchaResponseDto captchaResponse)
        {
            await _captchaService.ValidateCaptcha(captchaResponse);
            return "Ok";
        }

        
    }
}
