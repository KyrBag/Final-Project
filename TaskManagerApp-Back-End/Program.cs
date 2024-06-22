using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using TaskManagerApp.Repositories;
using TaskManagerApp.Services;
using AutoMapper;
using TaskManagerApp.Configuration;
using TaskManagerApp.Helpers;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TaskManagerApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Database configuration
			builder.Services.AddDbContext<TaskManagerDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			// Add services to the container.
			builder.Services.AddEndpointsApiExplorer();

			// Configure logging
			builder.Logging.ClearProviders();
			builder.Logging.AddConsole();
			builder.Logging.AddDebug();
			builder.Logging.AddEventSourceLogger();

			// Identity configuration
			builder.Services.AddControllers();
			builder.Services.AddLogging();
			builder.Services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<TaskManagerDbContext>()
				.AddDefaultTokenProviders();


			builder.Services.Configure<IdentityOptions>(options =>
			{
			// Password settings
			options.Password.RequireDigit = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = true;
			options.Password.RequiredLength = 6;
			options.Password.RequiredUniqueChars = 1;

			// Lockout settings
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
			options.Lockout.MaxFailedAccessAttempts = 5;
			options.Lockout.AllowedForNewUsers = true;

			// User settings
				options.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = true;
			});

			builder.Services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/Account/AccessDenied";
				options.SlidingExpiration = true;
			});

			// Scoped services
			builder.Services.AddScoped<ITaskItemService, TaskItemService>();
            builder.Services.AddScoped<SignInManager<User>>();
            builder.Services.AddScoped<UserManager<User>>();
			builder.Services.AddScoped<IUserService, UserService>();

			// Per request scope
			builder.Services.AddScoped(provider =>
				new MapperConfiguration(cfg =>
				{
					cfg.AddProfile(new MapperConfig());
				})
			.CreateMapper());


			builder.Services.AddControllers()
			   .AddNewtonsoftJson(options =>
			   {
				   options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				   options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
				   options.SerializerSettings.Converters.Add(new StringEnumConverter());
			   });

			var secretKey = builder.Configuration["Authentication:SecretKey"];
			var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
			var logger = loggerFactory.CreateLogger<Program>();
			//logger.LogInformation($"Secret key used in JWT Bearer options: {secretKey}");

			builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]!)),
					SignatureValidator = (token, validator) => { return new JsonWebToken(token); },
					//ValidIssuer = builder.Configuration["Authentication:Issuer"],
					//ValidAudience = builder.Configuration["Authentication:Audience"]
					/// return new JsonWebToken(token); in .NET 8
					/// Override the default token signature validation an do NOT validtae the signature
					/// Just return the token 

				};

				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						// Log the exception message
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						logger.LogError(context.Exception, "Authentication failed.");
						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						// Log that the token was successfully validated
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						logger.LogInformation("Token validated successfully.");
						return Task.CompletedTask;
					},
					OnMessageReceived = context =>
					{
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						//logger.LogInformation($"Secret key used in JWT Bearer options: {secretKey}");
						return Task.CompletedTask;
					},
					OnChallenge = context =>
					{
						// Log details about the challenge
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						logger.LogWarning("Token challenge.");
						return Task.CompletedTask;
					}
				};
			});

			builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Manager App", Version = "v1" });
				
				options.OperationFilter<AuthorizeOperationFilter>();
				options.SupportNonNullableReferenceTypes();
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey, // Http ,//instead of ApiKey
						Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "Bearer"
                    });

					options.AddSecurityRequirement(new OpenApiSecurityRequirement{
					{
						new OpenApiSecurityScheme{
							Reference = new OpenApiReference{
								Id = "Bearer",
								Type = ReferenceType.SecurityScheme
							}
						},
						new List<string>()
					}
				});
			});

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin());
            });

           /* builder.Services.AddCors(options => {
                options.AddPolicy("AngularClient",
                    b => b.WithOrigins("http://localhost:4200") // Assuming Angular runs on localhost:4200
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });*/

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1"));
            }

            app.UseHttpsRedirection();

			//app.UseRouting(); //included in MapControllers

			//app.UseCors("AllowAll");
			app.UseCors(options =>
			{
				options.AllowAnyHeader();
				options.AllowAnyMethod();
				options.AllowAnyOrigin();
			});



			/// Global Error Handler
			//
			//app.UseMiddleware<ErrorHandlerMiddleware>();

			app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
	}
}
