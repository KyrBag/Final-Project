using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerApp.DTOs;
using TaskManagerApp.Models;
using TaskManagerApp.Security;
using TaskManagerApp.Services.Exceptions;

namespace TaskManagerApp.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService>? _logger;
        private readonly IMapper? _mapper;

        public UserService(ILogger<UserService>? logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

		public string GenerateUserToken(string userId, string? username, string? email,
				 string? appSecurityKey, IList<string> roles)
		{
			try
			{
				//_logger!.LogInformation($"Secret key used for token generation: {appSecurityKey}");

				// Convert the appSecurityKey to a byte array
				var securityKey = Encoding.ASCII.GetBytes(appSecurityKey!);

				// Create a symmetric security key object
				var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);

				// Define the signing credentials using the symmetric security key and HMAC-SHA256 algorithm
				var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

				// Create a list of claims for the token
				var claims = new List<Claim>
				{
					new Claim(JwtRegisteredClaimNames.Sub, userId),
					new Claim(JwtRegisteredClaimNames.UniqueName, username ?? ""),
					new Claim(JwtRegisteredClaimNames.Email, email ?? ""),
					new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "User")

				};

				// Add role claims
				if (roles != null)
				{
					foreach (var role in roles)
					{
						claims.Add(new Claim(ClaimTypes.Role, role));
					}
				}

				// Create the JWT token descriptor
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(claims),
					Expires = DateTime.UtcNow.AddDays(30), // Set a long expiration time for testing
					SigningCredentials = signingCredentials
					//Expires = DateTime.UtcNow.AddHours(3), // Set the token expiration time
					//Issuer = issuer,
					//Audience = audience,
				};

				// Create a token handler
				var tokenHandler = new JwtSecurityTokenHandler();

				// Create the token based on the token descriptor
				var securityToken = tokenHandler.CreateToken(tokenDescriptor);

				// Serialize the token to a string
				var tokenString = tokenHandler.WriteToken(securityToken);

				// Log token generation success
				_logger!.LogInformation("JWT token generated successfully for user {UserId}.", userId);

				return tokenString;
			}
			catch (Exception ex)
			{
				// Log token generation failure
				_logger!.LogError(ex, "Failed to generate JWT token for user {UserId}.", userId);
				throw; // Rethrow the exception to handle it at a higher level
			}
		}
	}
}
