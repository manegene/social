using Kmums.Areas.Identity.Data;
using Kmums.CustomServices;
using Kmums.Models;
using Kmums.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.UI;
using AspNetCore.ReCaptcha;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQL-Conn"),
        provideroptions=>provideroptions.EnableRetryOnFailure()));


builder.Services.AddDefaultIdentity<UserModel>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<RolesModel>()
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<DataContext>();

// Add microsoft auth services to the container.
builder.Services.AddAuthentication()
    .AddMicrosoftAccount(microsoftOptions =>
 {
     microsoftOptions.ClientId = config["AzureAd:ClientId"];
     microsoftOptions.ClientSecret = config["AzureAd:ClientSecret"];
 })

//Add google auth service
    .AddGoogle(googleOptions =>
{
    googleOptions.ClientId = config["GoogleCreds:client_id"];
    googleOptions.ClientSecret = config["GoogleCreds:client_secret"];
});



builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build());

builder.Services.AddRazorPages()
.AddMicrosoftIdentityUI()
.AddRazorPagesOptions(options => {
    options.Conventions
    .AddAreaPageRoute("Identity", "/Account/Login", "/login")
    .AddAreaPageRoute("Identity", "/Account/Register", "/signup")
    .AddAreaPageRoute("Identity", "/Account/ForgotPassword", "/ForgotPassword")
    .AddAreaPageRoute("Identity", "/Account/ResendEmailConfirmation", "/ResendEmailConfirmation")
    .AddAreaPageRoute("Identity", "/Account/Manage/Email", "/manage-email")
    .AddAreaPageRoute("Identity", "/Account/Manage/SetPassword", "/manage-password")
    .AddAreaPageRoute("Identity", "/Account/Manage/Chat", "/chats")
    .AddAreaPageRoute("Identity", "/Account/Manage/ExternalLogins", "/external-logins")
    .AddAreaPageRoute("Identity", "/Account/Manage/Images", "/photos")
    .AddAreaPageRoute("Identity", "/Account/Manage/Public", "/public-profile")
    .AddAreaPageRoute("Identity", "/Account/Manage/Index", "/User-home")
    .AddAreaPageRoute("Identity", "/Account/Manage/Public", "/profile")
    .AddAreaPageRoute("Identity", "/Account/Manage/TwoFactorAuthentication", "/MFA-setup")
    .AddAreaPageRoute("Identity", "/Account/Manage/SentChat", "/sent-mails");
});
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
builder.Services.AddTransient<IEmailSender, EmailSendService>();
builder.Services.Configure<EmailAuth>(config);
builder.Services.AddReCaptcha(opts =>
{
    opts.SiteKey = config["ReSiteKey"];
    opts.SecretKey = config["ReSecretKey"];
});
builder.Services.AddSeoTags(seoOptions =>
{
    seoOptions.SetSiteInfo(
            siteTitle: "KilimanimumsKe",
            siteFacebookId: "https://facebook.com/MySite",  //optional
            openSearchUrl: "https://kilimanimums.ke/sitemap.xml",  //optional
            robots: "index, follow"  //optional
        );

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
