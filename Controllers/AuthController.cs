using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Common;
using ShairiStore.Models;
using ShairiStore.Repositories;
using ShairiStore.Services;

namespace ShairiStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotificationStrategy _notificationStrategy;
        private readonly IUserRepository _userRepository;
        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager,
            INotificationStrategy emailNotificationStrategy,
            IUserRepository userRepository
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _notificationStrategy = emailNotificationStrategy;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
                return BadRequest(new { error = "Email already in use" });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);


            if (!string.IsNullOrEmpty(model.RoleName))
            {
                if (!await _roleManager.RoleExistsAsync(model.RoleName))
                    await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

                await _userManager.AddToRoleAsync(user, model.RoleName);

                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenService.CreateTokenAsync(user, roles);

                return Ok(new { token });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return Unauthorized(new { error = "Invalid credentials" });

                var res = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (!res.Succeeded)
                    return Unauthorized(new { error = "Invalid credentials" });

                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenService.CreateTokenAsync(user, roles);

                var userRecord = await _userRepository.GetUserByIdAsync(user.Id);

                var loginResponse = new LoginResponseModel
                {
                    Id = user.Id,
                    Email = userRecord.Email,
                    FullName = userRecord.FullName,
                    PhoneNumber = userRecord.PhoneNumber,
                    token = token,
                    roles = roles[0],
                    RoleId = roles[0] == "Admin" ? 4 : roles[0] == "Sub Admin" ? 2 : roles[0] == "Manager" ? 1 : roles[0] == "Employee" ? 3 : 0,
                    DisplayPicture = ""
                };

                return Ok(loginResponse);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { error = "User not found" });

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // You would normally send this via email using your INotificationStrategy
            await _notificationStrategy.SendNotificationAsync(user.Email,
                "Password Reset",
                $"Use this token to reset your password: {token}");

            return Ok(new { bypasstoken = token ,message = "Password reset token generated and sent to email." });
        }


        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { error = "User not found" });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password reset successful." });
        }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Email { get; set; }
            public string Token { get; set; }   // The reset token generated earlier
            public string NewPassword { get; set; }
        }
    }
}
