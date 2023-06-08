using AdminService.HashService;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataConnection.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Service.AccountService;
using Service.AdviseService;
using Service.AuthService;
using Service.CategoryMappingService;
using Service.CategoryService;
using Service.ChatService;
using Service.EmployeeService;
using Service.ExpertService;
using Service.MapperConfig;
using Service.RatingService;
using Service.ThrowException;
using Service.UserService;
using Swashbuckle.AspNetCore.Filters;

namespace ExpertConnection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<HashService>().As<IHashService>();
                builder.RegisterType<EmployeeService>().As<IEmployeeService>();
                builder.RegisterType<AccountService>().As<IAccountService>();
                builder.RegisterType<ExpertConnectionContext>().AsSelf();
                builder.RegisterType<AuthService>().As<IAuthService>();
                builder.RegisterType<UserService>().As<IUserService>();
                builder.RegisterType<ExpertService>().As<IExpertService>();
                builder.RegisterType<CategoryService>().As<ICategoryService>();
                builder.RegisterType<CategoryMappingService>().As<ICategoryMappingService>();
                builder.RegisterType<AdviseService>().As<IAdviseService>();
                builder.RegisterType<ChatService>().As<IChatService>();
                builder.RegisterType<RatingService>().As<IRatingService>();
            });
            builder.Services.AddAutoMapper(typeof(ConfigMapper).Assembly);
            builder.Services.AddControllers(option => option.Filters.Add(typeof(CustomerExceptionFilter)));
            builder.Services.AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();
                options.AddSecurityDefinition("Token-Expert-Connection", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "Past token here",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                      new OpenApiSecurityScheme{
                      Reference = new OpenApiReference {
                      Type=ReferenceType.SecurityScheme,
                      Id="Token-Expert-Connection"
                      }
                      },
                      new List<string>()
                    }
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                        options.RoutePrefix = string.Empty;
                    });
                }
            }
            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}