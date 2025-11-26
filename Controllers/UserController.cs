using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;
using ShairiStore.Services;

namespace ShairiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Manager")] // Restrict to Admin & Manager
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepo;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotificationRepository _notificationRepository;

        public UserController(IUserRepository userRepository,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            INotificationRepository notificationRepository
        )
        {
            _userRepo = userRepository;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        // ✅ Get all users
        [HttpGet("listusers")]
        public async Task<IActionResult> ListAllUsers()
        {
            var result = await _userRepo.ListAllUsersAsync();
            return Ok(result);
        }

        // ✅ Get user by Id
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // ✅ Update user
        [HttpPut("UpdateUserById/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser user)
        {
            if (id != user.Id) return BadRequest(new { message = "Mismatched user ID" });

            var existingRole = await _userManager.GetRolesAsync(user);
            var updatedUser = await _userRepo.UpdateUserAsync(user);
            var updatedRole = await _userManager.GetRolesAsync(user);
            string content = string.Empty;
            if (existingRole[0].Equals(updatedRole[0],StringComparison.InvariantCultureIgnoreCase))
            {
                content = $"Account has been updated succesfully for user: {user?.FullName} with Role: {existingRole[0]} at: {DateTime.Now}.";
            }
            else
            {
                content = $"Account has been updated succesfully for user: {user?.FullName} OLD Role: {existingRole[0]} New Role: {updatedRole[0]} at: {DateTime.Now}.";
            }
            var notificationRequest = MapNotificationPayload(title: "User Account Updated", content: content, redirectURL: $"/user", notificationBy: user.Id, notificationFor: user.Id, notificationType: 2, DateTime.Now, "User");
            var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
            if (updatedUser == null) return BadRequest(new { message = "Failed to update user" });

            return Ok(updatedUser);
        }

        // ✅ Delete user
        [HttpDelete("DeleteUserById/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deletedUser = await _userRepo.DeleteUserAsync(id);
            if (deletedUser == null) return NotFound(new { message = "User not found or delete failed" });

            var notificationRequest = MapNotificationPayload(title: "User Account Deleted", content: $"Account has been deleted succesfully for user: {deletedUser?.FullName} at: {DateTime.Now}.", redirectURL: $"/user", notificationBy: id, notificationFor: id, notificationType: 9, DateTime.Now, "User");
            var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);

            return Ok(deletedUser);
        }

        // ✅ Assign role to user
        [HttpPost("{id}/assign-role")]
        public async Task<IActionResult> AssignRole(string id, [FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest(new { message = "Role name is required" });

            var updatedUser = await _userRepo.AssignRoleAsync(id, roleName);
            if (updatedUser == null) return BadRequest(new { message = "Failed to assign role" });

            return Ok(updatedUser);
        }

        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest model)
        {
            int[] roleArray = new int[]
            {
                1,
                2,
                3
            };

            if (!roleArray.Contains(model.RoleId))
                return BadRequest(new { error = $"Invalid Role Id Provided : {model.RoleId}" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
                return BadRequest(new { error = "Email already in use" });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                DisplayPicture = model.DisplayPicture,
                //DisplayPic =  model.DisplayPicture,
                EmailConfirmed = string.IsNullOrEmpty(model.Email) ? false : true,
                PhoneNumberConfirmed = string.IsNullOrEmpty(model.PhoneNumber) ? false : true,
                RoleName = model.RoleName,
                RawPassword = model.Password
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

                var notificationRequest = MapNotificationPayload(title: "User Account Created", content: $"Account has been created succesfully for user: {user?.FullName} with Role: {roles[0]} at: {DateTime.Now}.", redirectURL: $"/user", notificationBy: user.Id, notificationFor: user.Id, notificationType: 1, DateTime.Now, "User");
                var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);

                return Ok( new {success = "User created successfully"});
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private static NotificationDetails MapNotificationPayload(string title, string content, string redirectURL, string notificationBy, string notificationFor, int notificationType, DateTime notificationDate, string moduleName)
        {
            var notificationRequest = new NotificationDetails();
            notificationRequest.NotificationTitle = title;
            notificationRequest.RedirectURL = redirectURL;
            notificationRequest.NotificationRecepient = notificationFor;
            notificationRequest.NotificationGeneratedBy = notificationBy;
            notificationRequest.IsRead = false;
            notificationRequest.IsActive = true;
            notificationRequest.NotificationContent = content;
            notificationRequest.NotificationTypeId = notificationType;
            notificationRequest.NotificationCreatedAt = notificationDate;
            notificationRequest.NotificationModule = moduleName;
            return notificationRequest;
        }
    }
}
