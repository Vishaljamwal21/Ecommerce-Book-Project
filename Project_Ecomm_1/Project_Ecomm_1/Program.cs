using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Project_Ecomm_1.DataAccess.Data;
using Project_Ecomm_1.DataAccess.Repository;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("constr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//  .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<ICoverTypeRepository, CoverTypeRepository>();
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "2526724704144466";
    options.AppSecret = "8aea4ed7653782ef6b50ad7a0ba99b5c";
});
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "225724888716-17jdu0l7flv0t015v46o4agb6jf6n2jn.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
builder.Services.AddAuthentication().AddTwitter(options =>
{
    options.ConsumerKey = "1QYYs16hnRctmWh1iz1Mu0TeW";
    options.ConsumerSecret = "OhEOVfcDuv44MC2YyegjPYPvZKPtz8NNMSJufACgNNz9zgIXgN";
});

builder.Services.AddAuthentication().AddInstagram(options =>
{
        options.ClientId = "2526724704144466";
        options.ClientSecret = "8aea4ed7653782ef6b50ad7a0ba99b5c";
});

builder.Services.AddSession(options =>
{ 
    options.IdleTimeout= TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly= true;
    options.Cookie.IsEssential= true;
});

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));


//builder.Services.AddAuthentication().AddLinkedIn(options =>
//{
//    options.ClientId = "77cpp7m93y7hn1";
//    options.ClientSecret = "2p2OzAym3EqF844z";

//});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["Secretkey"];


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
