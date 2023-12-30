using FlightTicket.Business.Abstract;
using FlightTicket.Business.Concreate;
using FlightTicket.Data.Abstract;
using FlightTicket.Data.Concreate.EfCore;
using FlightTicket.Data.Identity;
using FlightTicket.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region Localizer
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddMvcLocalization().AddDataAnnotationsLocalization(options =>
options.DataAnnotationLocalizerProvider = (type, factory) =>
{
    var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
    return factory.Create(nameof(SharedResource), assemblyName.Name);
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("tr-TR"),
    };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture: "tr-TR", uiCulture: "tr-TR");
    options.SupportedCultures = supportCultures;
    options.SupportedUICultures = supportCultures;
    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});
#endregion

var sqliteConnection = builder.Configuration.GetConnectionString("SqliteCon");

builder.Services.AddDbContext<Context_Identity>(o => o.UseSqlite(sqliteConnection));

builder.Services.AddIdentity<User, IdentityRole>()
     .AddEntityFrameworkStores<Context_Identity>()
     .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = ".FlightTicket.Security.Cookie"
    };
});



builder.Services.AddDbContext<Context_FlightTicket>(opt => opt.UseSqlite(sqliteConnection));

builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ITripService, TripManager>();

builder.Services.AddScoped<ILineRepository, LineRepository>();
builder.Services.AddScoped<ILineService, LineManager>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerManager>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketManager>();

builder.Services.AddScoped<IMidlineRepository, MidlineRepository>();
builder.Services.AddScoped<IMidlineService, MidlineManager>();

builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightService, FlightManager>();

builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IDriverService, DriverManager>();

builder.Services.AddScoped<ITripDetailRepository, TripDetailRepository>();
builder.Services.AddScoped<ITripDetailService, TripDetailManager>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value);

app.UseCookiePolicy();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
