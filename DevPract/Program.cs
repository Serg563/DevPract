

using DevPract;
using DevPract.AllData;
using DevPract.AllData.Cart;
using DevPract.AllData.Services;
using DevPract.Models.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.UI;
using NLog.Extensions.Logging;
using System.Security.Principal;


//using MainContext = DevPract.Models.Domain.MainContext;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddDbContext<MainContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

//Services configuration
builder.Services.AddScoped<IActorsService, ActorService>();
builder.Services.AddScoped<IProducersService, ProducersService>();
builder.Services.AddScoped<ICinemasService, CinemasService>();
builder.Services.AddScoped<IMoviesService, MoviesService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(ShoppingCart.GetShoppingCart);



builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<MainContext>()
            .AddDefaultTokenProviders();
builder.Services.AddMemoryCache();
builder.Services.AddSession();


builder.Services.AddRouting(options =>
{
    // Replace the type and the name used to refer to it with your own
    // IOutboundParameterTransformer implementation
    options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
});
builder.Services.AddMvc(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(
             new SlugifyParameterTransformer()));

});
builder.Services.AddControllersWithViews();
/*builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});*/


/*builder.Services.AddAuthentication()
    .AddIdentityServerJwt()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = "413861196441-js8elb0k6hgscg1mjv5kemjvki23rg99.apps.googleusercontent.com";
        googleOptions.ClientSecret = "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";
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = "853519985762874";
        facebookOptions.AppSecret = "12b96b93243cfd343aee0cae65ff37ac";
    });*/

builder.Services.AddResponseCompression();




var app = builder.Build();

app.UseResponseCompression();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

/*app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller:slugify=Home}/{action:slugify=Index}/{id?}");
    
});*/
app.MapControllerRoute(
    name: "default",
    pattern: "{controller:slugify=Movies}/{action:slugify=Index}/{id?}");

AppDbInitializer.Seed(app);
AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.Run();
