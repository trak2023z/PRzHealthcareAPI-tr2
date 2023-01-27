using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;

namespace PRzHealthcareAPI.Controllers
{
    [Route("account")]
    [ApiController]

    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            await _userService.Register(dto);
            return Ok();
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            var loggedUser = _userService.GenerateToken(dto);
            return Ok(loggedUser);
        }

        [AllowAnonymous]
        [HttpGet("confirm-mail")]
        public async Task<IActionResult> ConfirmMail([FromQuery]  string hashCode)
        {
            var message = await _userService.ConfirmMail(hashCode);
            return Ok(message);
        }

        [HttpPost("changepassword")]
        [Authorize]
        public ActionResult ChangePassword([FromBody] LoginUserDto dto)
        {
            _userService.ChangePassword(dto);
            return Ok();
        }

    }
}
