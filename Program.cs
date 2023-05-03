using Application.Interface.Data;
using Application.Interface.Service;
using Application.Services;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//--< Migration Assembly le kah rakhne migration tyo decide garxa>-- 
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddDbContext<ApplicationDbContext>
    (e => e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IActivityLogService, AcitvityLogService>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<ITaskRepository, TaskRepository>();
builder.Services.AddTransient<ITasksUserService, TasksUserService>();
builder.Services.AddTransient<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddTransient<ITasksUserRepository, TasksUserRepository>();
// --< Dependency Injection From Here>-- 
// --< This is Repository Pattern >--
//builder.Services.InternalDependency();
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
//--<Adding Identity>--
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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
