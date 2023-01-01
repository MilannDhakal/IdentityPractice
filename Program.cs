using Domain.Models;
//using IdentityPractice.InternalDependency;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Migration Assembly le kah rakhne migration tyo decide garxa 

builder.Services.AddDbContext<ApplicationDbContext>(e =>
 e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), e => e.MigrationsAssembly("IdentityPractice")));
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// For Dependency Injection 
// dont understand why behaive like this
//builder.Services.AddInternalDependency();
// Adding Identity
//builder.Services.AddScoped<typeof(IUnitOfWork<T>) , typeof(UnitOfWork<>) >;
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
var app = builder.Build();
 
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");
app.MapRazorPages();
app.Run();
