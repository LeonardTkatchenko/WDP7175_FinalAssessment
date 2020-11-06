using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WDP20207175A3.Data;

namespace WDP20207175A3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI();

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddControllersWithViews();
            services.AddRazorPages();

            // Update database
            services.BuildServiceProvider()
                .GetRequiredService<ApplicationDbContext>()
                .Database.Migrate();

            // Add user with Manager role
            string[] roles = new string[] { "Admin" };
            string email = "admin@asp.net";
            string password = "Wdp-2020";
            AddRoles(roles, services);
            AddUser(email, password, services);
            AssignRolesToUser(roles, email, services);

            // Add user with no role
            email = "user@asp.net";
            password = "Wdp-2020";
            AddUser(email, password, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy(); // not in Core 3.1

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization(); //Core 3.1

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }


        private async void AddRoles(string[] roleNames, IServiceCollection services)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var roleName in roleNames)
            {
                bool roleExists = await roleManager.RoleExistsAsync(roleName);
                IdentityRole userRole = new IdentityRole(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(userRole);
                }
            }
        }
        private async void AddUser(string email, string password, IServiceCollection services)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IdentityUser identityUser = await userManager.FindByEmailAsync(email);
            if (identityUser == null)
            {
                identityUser = new IdentityUser
                {
                    UserName = email,
                    Email = email
                };
                var createUser = await userManager.CreateAsync(identityUser, password);
            }
        }
        private async void AssignRolesToUser(string[] roleNames, string email, IServiceCollection services)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IdentityUser identityUser = await userManager.FindByEmailAsync(email);
            if (identityUser != null)
            {
                await userManager.AddToRolesAsync(identityUser, roleNames);
            }
        }

    }
}
