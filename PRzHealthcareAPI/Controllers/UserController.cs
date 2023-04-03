using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using System.Security.Claims;

namespace PRzHealthcareAPI.Controllers
{
    [Route("account")]
    [ApiController]
    [Authorize]

    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            await _userService.Register(dto);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            var loggedUser = _userService.GenerateToken(dto);
            return Ok(loggedUser);
        }

        [HttpGet("getdoctorslist")]
        //todo: [Authorize]
        public ActionResult GetDoctorsList()
        {
            var doctors = _userService.GetDoctorsList();
            return Ok(doctors);
        }

        [AllowAnonymous]
        [HttpGet("confirm-mail")]
        public async Task<IActionResult> ConfirmMail([FromQuery] string hashCode)
        {
            var message = await _userService.ConfirmMail(hashCode);
            return Ok(message);
        }

        [HttpPut("changepassword")]
        [Authorize]
        public ActionResult ChangePassword([FromBody] ChangeUserPasswordDto dto)
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                dto.Login = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            _userService.ChangePassword(dto);
            return Ok();
        }

    }
}
