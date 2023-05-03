using Application.Interface.Data;
using Application.Interface.Service;
using Application.Services;
using Infrastructure.Data;

namespace IdentityPractice.Dependency
{
    public  static class Services
    {

        public static  IServiceCollection InternalDependency(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IActivityLogService, AcitvityLogService>();
            services.AddTransient<ITasksService, TasksService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();

            return services;
        }

    }
}
