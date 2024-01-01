using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson.Serialization.Conventions;
using MongoDbOrdersAPI.Model;
using MongoDbOrdersAPI.Data;
using MongoDbOrdersAPI.Services;
using System.Text;

namespace Major_BeanScene_ReservationSystem
{
    public partial class Program
    {
        //this is zacs comment
        //Foie Gras
        public static void Main(string[] args)
        {
           
            var builder = WebApplication.CreateBuilder(args);
           

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //CamelConvention
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            //CORS
            builder.Services.AddCors(option =>
            {
                option.AddDefaultPolicy(
                    policy =>
                    {
                        policy
                        //.WithOrigins("http://localhost:5240","https://localhost:7200")
                        .WithOrigins("*")
                        .WithMethods("PUT", "GET", "DELETE", "PATCH", "POST", "OPTIONS")
                        .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization);
                        

                    });
            });

            // Adds both cookie and JWT Bearer token based authentication, so that you can still sign in using the website.
            // The policy scheme is used to determine which authentication scheme should be used so that both will work.
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = "JWT_OR_COOKIE";
                o.DefaultChallengeScheme = "JWT_OR_COOKIE";
            })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,

                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

                            // Prevents tokens without an expiry from ever working, as that would be a security vulnerability.
                            RequireExpirationTime = true,

                            // ClockSkew generally exists to account for potential clock difference between issuer and consumer
                            // But we are both, so we don't need to account for it.
                            // For all intents and purposes, this is optional
                            //ClockSkew = TimeSpan.Zero
                            //The zero clock skew is causing 500 error i think. Nope it was due to development variables
                            ClockSkew =  TimeSpan.FromSeconds(300)
                        };
                    })
                    .AddPolicyScheme("JWT_OR_COOKIE", null, o =>
                    {
                        o.ForwardDefaultSelector = c =>
                        {
                            string auth = c.Request.Headers[HeaderNames.Authorization];
                            if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer "))
                            {
                                return JwtBearerDefaults.AuthenticationScheme;
                            }

                            return IdentityConstants.ApplicationScheme;
                        };
                    });

            builder.Services.Configure<OrderSystemDatabaseSettings>(
                builder.Configuration.GetSection("OrderDatabase"));
            builder.Services.AddTransient<MenuItemService>()
                .AddTransient<OrderService>()
                .AddTransient<ApplicationContextAPIService>();


            builder.Services.AddControllers()
                .AddJsonOptions(o => { o.JsonSerializerOptions.PropertyNameCaseInsensitive = false; });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                //
                app.UseSwagger();
                app.UseSwaggerUI();

                //app.UseExceptionHandler("/Error");
                app.UseWhen(
                    context => !context.Request.Path.StartsWithSegments("/api"),
                    appBuilder =>
                    {
                        appBuilder.UseStatusCodePagesWithReExecute("/Error/Status", "?statusCode={0}");
                    }
                );

                //app.UseExceptionHandler("/Home/Error");
                //This preserve status code for the api
                //app.UseStatusCodePagesWithReExecute("/Error/Status", "?statusCode={0}");
            }
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseExceptionHandler("/Home/Error");
                app.UseWhen(
                    context => !context.Request.Path.StartsWithSegments("/api"),
                    appBuilder =>
                    {
                        appBuilder.UseStatusCodePagesWithReExecute("/Error/Status", "?statusCode={0}");
                    }
                );

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //Cors
            app.UseCors();
            //
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllers();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}