using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using SlientMoon.AdminPanel.Models;
using SlientMoon.AdminPanel.Models.Courses;
using SlientMoon.Application;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;
using SlientMoon.Infrastructure.Messaging.Services;
using SlientMoon.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SlientMoon.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IValidator<LoginViewModel>, LoginViewModelValidator>();
builder.Services.AddScoped<IValidator<CreateCourseViewModel>, CreateCourseViewModelValidator>();
builder.Services.AddScoped<IValidator<EditCourseViewModel>, EditCourseViewModelValidator>();

builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceRegistration(builder.Configuration);
builder.Services.AddPersistenceApiServices(builder.Configuration);

builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();