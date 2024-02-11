using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NZWALKS.API.CustomActionFilters;
using NZWALKS.API.Models.Domain;
using NZWALKS.API.Models.DTO;
using NZWALKS.API.Repositories;
using NZWALKS.API.Utility;


namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailSender emailSender;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.roleManager = roleManager;
            this.emailSender = emailSender;
        }


        // POST: /api/Auth/Register
        [HttpPost]
        [ValidateModel]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var existingUser = await userManager.FindByNameAsync(registerRequestDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists. Please choose a different username.");
            }

            var identityUser = new ApplicationUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username,
                Name = registerRequestDto.Name,
                StreetAddress = registerRequestDto.StreetAddress,
                State = registerRequestDto.State,
                City = registerRequestDto.City,
                PostalCode = registerRequestDto.PostalCode,

            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                await userManager.SetPhoneNumberAsync(identityUser, $"{registerRequestDto.CountryCode}{registerRequestDto.PhoneNumber}");
                // Add roles to this User
                if (registerRequestDto.Role != null && registerRequestDto.Role.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, new[] { registerRequestDto.Role });

                    if (identityResult.Succeeded)
                    {
                        return Ok("User was registered! Please login.");
                    }
                }
            }

            return BadRequest("Something went wrong");
        }


        // POST: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    // Get Roles for this user
                    var roles = await userManager.GetRolesAsync(user);

                    if (roles != null)
                    {
                        // Create Token

                        var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());

                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(response);
                    }
                }
            }

            return BadRequest("Username or password incorrect");
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok("Password reset link sent if the email exists.");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Auth", new { userId = user.Id, token }, HttpContext.Request.Scheme);

            // Compose email message
            var subject = "Reset your password";
            var htmlMessage = $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.";

            // Send email
            await emailSender.SendEmailAsync(user.Email, subject, htmlMessage);

            return Ok("Password reset link sent.");
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByIdAsync(resetPasswordDto.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var result = await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password reset successfully.");
            }

            return BadRequest("Failed to reset password");
        }

        // PATCH: /api/Auth/ChangeRole/{userId}
        [HttpPatch]
        [Route("ChangeRole/{userId}")]
        public async Task<IActionResult> ChangeRole(string userId, [FromBody] ChangeRoleRequestDto changeRoleRequestDto)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Remove existing roles
            var currentRoles = await userManager.GetRolesAsync(user);
            var removeRolesResult = await userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeRolesResult.Succeeded)
            {
                return BadRequest("Failed to remove existing roles");
            }

            // Add new roles
            var addRolesResult = await userManager.AddToRolesAsync(user, changeRoleRequestDto.Roles);

            if (addRolesResult.Succeeded)
            {
                return Ok("User roles changed successfully");
            }

            return BadRequest("Failed to change user roles");
        }

        // GET: /api/Auth/GetAllUsers
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = userManager.Users.ToList(); // Retrieve all users

            if (users != null && users.Any())
            {
                var userDtos = users.Select(async user =>
                {
                    var roles = await userManager.GetRolesAsync(user);
                    var firstRole = roles.FirstOrDefault();
                    return new UserDto
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        Email = user.Email,
                        Role = firstRole
                    };
                }).Select(task => task.Result);

                return Ok(userDtos);
            }

            return NotFound("No users found");
        }


    }
}