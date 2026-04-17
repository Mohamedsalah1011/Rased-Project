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
using Microsoft.Extensions.FileProviders;

namespace Rased_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. تسجيل الخدمات الأساسية (Services) ---
            builder.Services.AddControllers();

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IComplaintService, ComplaintService>();
            builder.Services.AddTransient<IJWTService, JwtService>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddMemoryCache();

            // قاعدة البيانات
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // --- 2. إعدادات الـ Identity ---
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
            .AddDefaultTokenProviders();

            // --- 3. إعدادات الـ JWT Authentication ---
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

            // --- 4. إعدادات الـ CORS والـ Swagger ---
            builder.Services.AddCors(options => {
                options.AddDefaultPolicy(policy => {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // --- 5. إعدادات الـ Middleware (الترتيب مهم جداً) ---

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHsts();
            app.UseHttpsRedirection();

            // --- تفعيل ملفات الصور (Static Files) ---
            // أ. الملفات العادية في wwwroot
            app.UseStaticFiles();

            // ب. إعدادات خاصة لفولدر الـ uploads لضمان وصول الموبايل للصور
            var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/uploads"
            });

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}