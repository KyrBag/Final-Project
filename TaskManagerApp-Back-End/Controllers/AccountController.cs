using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.DTOs;
using TaskManagerApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using TaskManagerApp.Data;
using TaskManagerApp.Helpers;
using AutoMapper;
using TaskManagerApp.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagerApp.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly ILogger<AccountController> _logger;
		private readonly TaskManagerDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IMapper _mapper;
		private readonly IUserService _userService;
		private readonly RoleManager<IdentityRole> _roleManager;
		private const string UserRoleName = "User";

		public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
			IConfiguration configuration, ILogger<AccountController> logger,
			TaskManagerDbContext context, IMapper mapper, IUserService userService,
			RoleManager<IdentityRole> roleManager)

		{
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
			_logger = logger;
			_context = context;
			_mapper = mapper;
			_userService = userService;
			_roleManager = roleManager;

			// Create the "User" role if it doesn't exist
			var userRoleExists = _roleManager.RoleExistsAsync(UserRoleName).Result;
			if (!userRoleExists)
			{
				var result = _roleManager.CreateAsync(new IdentityRole(UserRoleName)).Result;
				if (!result.Succeeded)
				{
					// Handle role creation failure
					throw new ApplicationException($"Error creating role '{UserRoleName}'.");
				}
			}
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
		{
			_logger.LogInformation("Registration attempt initiated for user {Username}.", dto.Username);

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for registration.");
				return BadRequest(ModelState);
			}

			var user = new User
			{
				UserName = dto.Username,
				Email = dto.Email,
				FirstName = dto.FirstName,
				LastName = dto.LastName
			};

			var result = await _userManager.CreateAsync(user, dto.Password!);

			if (result.Succeeded)
			{
				_logger.LogInformation("User {Username} created successfully.", user.UserName);

				// Assign the "User" role to the newly created user
				var roleResult = await _userManager.AddToRoleAsync(user, "User");

				if (!roleResult.Succeeded)
				{
					// Handle the case where role assignment fails
					foreach (var error in roleResult.Errors)
					{
						ModelState.AddModelError(error.Code, error.Description);
					}

					_logger.LogError("Role assignment failed for user {Username}.", user.UserName);
					return BadRequest(ModelState);
				}

			// Automatically sign in the user after registration (optional)
			await _signInManager.SignInAsync(user, isPersistent: false);

			_logger.LogInformation("User {Username} registered successfully.", user.UserName);
			return Ok(new { message = "Registration successful. Redirecting to login..." });
		}
	

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(error.Code, error.Description);
			}

			_logger.LogError("User registration failed for {Username}.", dto.Username);
			return BadRequest(ModelState);
		}


		
		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<ActionResult<JwtTokenDTO>> LoginUserAsync(LoginDTO dto)
		{
			var user = await _userManager.FindByNameAsync(dto.Username!);
			if (user == null)
			{
				//throw new UnauthorizedAccessException("BadCredentials");
				return Unauthorized(new JwtTokenDTO
				{
					IsSuccess = false,
					Message = "User not found with this username",
				});
			}
			_logger!.LogInformation("Login Success");

			// Retrieve roles associated with the user
			var roles = await _userManager.GetRolesAsync(user);

			var userToken = _userService.GenerateUserToken(user.Id, user.UserName, user.Email, _configuration["Authentication:SecretKey"]!, roles);
			// Log the login event
			await LogLoginEvent(user);


			JwtTokenDTO token = new()
			{
				Token = userToken,
				IsSuccess = true,
				Message = "Login Success."
			};
			_logger!.LogInformation("Token has been given");
			return Ok(token);
		}


		private async Task LogLoginEvent(User user)
		{
			var loginEvent = new LoginEvent
			{
				UserId = user.Id,
				LoginTime = DateTime.UtcNow
			};

			_context.LoginEvents.Add(loginEvent);
			await _context.SaveChangesAsync();
		}



        [HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return Ok(new { message = "Logout successful." });
		}

		[AllowAnonymous]
		[HttpGet("me")]
		public async Task<IActionResult> GetCurrentUserWithTasks()
		{
			_logger.LogInformation("Get user Tasks details called");

			
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var user = await _userManager.Users
				.Include(u => u.Tasks)
				.Where(u => u.Id == userId)
				.Select(u => new UserTasksDTO
				{
					Username = u.UserName,
					UserId = u.Id,
					Email = u.Email,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Tasks = u.Tasks!.Select(t => new TaskItemDTO
					{
						Id = t.Id,
						Title = t.Title,
						Description = t.Description,
						DueDate = t.DueDate,
						IsCompleted = t.IsCompleted
					}).ToList()
				}).FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		


		[HttpGet("all")]
		public async Task<IActionResult> GetAllUsersWithTasks()
		{
			var users = await _userManager.Users
				.Include(u => u.Tasks)
				.Select(u => new UserTasksDTO
				{
					UserId = u.Id,
					Email = u.Email,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Tasks = u.Tasks!.Select(t => new TaskItemDTO
					{
						Id = t.Id,
						Title = t.Title,
						Description = t.Description,
						DueDate = t.DueDate,  
						IsCompleted = t.IsCompleted
					}).ToList()
				}).ToListAsync();

			return Ok(users);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO model)
		{
			_logger.LogInformation("UpdateMethod called");

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound(new { message = "User not found" });
			}

			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Email = model.Email;
			user.UserName = model.Username;

			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				return Ok(new { message = "User updated successfully" });
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return BadRequest(ModelState);
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound(new { message = "User not found" });
			}

			var result = await _userManager.DeleteAsync(user);

			if (result.Succeeded)
			{
				return Ok(new { message = "User deleted successfully" });
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return BadRequest(ModelState);
		}
		

	}
}