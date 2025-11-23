using LibrarySystemWebAPI.Data;
using LibrarySystemWebAPI.Interfaces;
using LibrarySystemWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace LibrarySystemWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var homeDir = Environment.GetEnvironmentVariable("HOME") ?? Directory.GetCurrentDirectory();
            var dbFolder = Path.Combine(homeDir, "data");
            Directory.CreateDirectory(dbFolder);

            var dbPath = Path.Combine(dbFolder, "app.db");

            Console.WriteLine($"SQLite DB path: {dbPath}");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            builder.Services.AddScoped<IBookRepository, EFBookRepository>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBookLoanRepository, EFBookLoanRepository>();
            builder.Services.AddScoped<IUserRepository, EFUserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var jwt = builder.Configuration.GetSection("Jwt");
            var key = jwt["Key"];
            if(string.IsNullOrEmpty(key))
            {
                throw new Exception("JWT config is missing or empty");
            }
            var keyBytes = Encoding.UTF8.GetBytes(key);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["jwt"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });



            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<AppDbContext>();
                var config = services.GetRequiredService<IConfiguration>();

                db.Database.Migrate();

                var adminSection = config.GetSection("AdminSeed");
                var username = adminSection["Username"];
                var password = adminSection["Password"];
                var role = adminSection["Role"];
                if(!db.Users.Any(u => u.UserName == username))
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    var adminUser = new Models.User
                    {
                        UserName = username,
                        PasswordHash = hashedPassword,
                        Role = role
                    };
                    db.Users.Add(adminUser);
                    db.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate(); // requires you to have at least one migration
            }
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Page}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
