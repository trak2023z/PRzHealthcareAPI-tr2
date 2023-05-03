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
        public ActionResult GetDoctorsList()
        {
            var doctors = _userService.GetDoctorsList();
            return Ok(doctors);
        }
        [HttpGet("getpatientslist")]
        public ActionResult GetPatientsList()
        {
            var patients = _userService.GetPatientsList();
            return Ok(patients);
        }

        [AllowAnonymous]
        [HttpGet("confirm-mail")]
        public async Task<IActionResult> ConfirmMail([FromQuery] string hashCode)
        {
            var message = await _userService.ConfirmMail(hashCode);
            return Ok(message);
        }

        [HttpPut("changepassword")]
        public ActionResult ChangePassword([FromBody] ChangeUserPasswordDto dto)
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                dto.Login = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            _userService.ChangePassword(dto);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut("reset-password")]
        public ActionResult ResetPassword([FromBody] ResetUserPasswordDto dto)
        {
            _userService.ResetPassword(dto);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPut("reset-password-request")]
        public ActionResult ResetPasswordRequest([FromQuery] string email)
        {
            _userService.ResetPasswordRequest(email);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut("reset-passwordcheck")]
        public ActionResult ResetPasswordCheckHashCode([FromQuery] string hashCode)
        {
            _userService.ResetPasswordCheckHashCode(hashCode);
            return Ok();
        }
    }
}
