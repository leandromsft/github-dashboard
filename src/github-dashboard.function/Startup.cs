using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using githubdashboard.function.Models.EF;

[assembly: FunctionsStartup(typeof(githubdashboard.function.StartUp))]

namespace githubdashboard.function
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // https://markheath.net/post/ef-core-di-azure-functions
            // https://medium.com/globant/entity-framework-on-azure-functions-with-dependency-injection-77208c94a16
            // leandro - #P@assw0rd# - githubdashboard.database.windows.net
            FunctionsHostBuilderContext context = builder.GetContext();  
            string connectionString = context.Configuration["MyDbContext"];
            
            if(!String.IsNullOrEmpty(connectionString))
                builder.Services.AddDbContext<MyDbContext>(options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));
        }
    }
}