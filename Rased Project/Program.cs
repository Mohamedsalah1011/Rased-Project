using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Rased.Core.Identity;
using Rased.Core.Servies;
using Rased.Core.ServiseContracts;
using Rased.Infrustracture.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Rased_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddScoped<RideMatchingService>();

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IComplaintService, ComplaintService>();
            builder.Services.AddTransient<IJWTService, JwtService>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IAdsService, AdsService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity Configuration
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders()
              .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
              .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            // JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // إضافة سياسة CORS مسمّية وصريحة لفتح الصلاحيات بالكامل
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll", policy => {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

            // Configure the HTTP request pipeline.
            app.UseHsts();
            app.UseHttpsRedirection();

            // تفعيل الـ Static Files الافتراضية (هتقرأ تلقائياً كل ما هو داخل wwwroot)
            app.UseStaticFiles();

            app.UseRouting();

            // تفعيل السياسة المسمّية بالترتيب الصحيح (بعد الـ Routing وقبل الـ Auth)
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // تشغيل السيرفر بشكل Async صحيح متوافق مع الـ Task Main
            await app.RunAsync();
        }
    }
}