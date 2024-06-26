﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskManagerApp.Helpers
{
	public class AuthorizeOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var authAtrributes = context.MethodInfo
				.GetCustomAttributes(true)
				.OfType<AuthorizeAttribute>()
				.Distinct();

			if (authAtrributes.Any())
			{
				operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
				operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

				operation.Security = new List<OpenApiSecurityRequirement>();
				{
					new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Description = "JWT Authorization header using the Bearer scheme.",
								Name = "Authorization",
								In = ParameterLocation.Header,
								Type = SecuritySchemeType.Http,
								Scheme = JwtBearerDefaults.AuthenticationScheme,
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = JwtBearerDefaults.AuthenticationScheme
								}
							},
							new List<string>()
						}
					};
				};
			}
		}
	}
}
