using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyWishList.Web.Data;
using MyWishList.Web.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebJobs(webJobsBuilder =>
    {
        webJobsBuilder.AddAzureStorageQueues();
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection is required for MyWishList.WebJobs.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IItemService, ItemService>();
    })
    .Build();

await host.RunAsync();
