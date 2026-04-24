using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyWishList.Web.BackgroundJobs;
using MyWishList.Web.Data;
using MyWishList.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase")
    || builder.Environment.IsEnvironment("Testing")
    || string.IsNullOrWhiteSpace(defaultConnectionString);
var inMemoryDatabaseName = builder.Configuration["InMemoryDatabaseName"] ?? "MyWishListTestDb";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useInMemoryDatabase)
    {
        options.UseInMemoryDatabase(inMemoryDatabaseName);
        return;
    }

    options.UseSqlServer(defaultConnectionString);
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "MyWishList API",
        Version = "v1"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddHostedService<WishlistMetricsBackgroundService>();
var storageQueueConnectionString = builder.Configuration["StorageQueue:ConnectionString"];
builder.Services.AddSingleton<IItemQueueService>(serviceProvider =>
{
    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    var queueName = builder.Configuration["StorageQueue:AddItemQueueName"] ?? "add-item-requests";

    if (!string.IsNullOrWhiteSpace(storageQueueConnectionString))
    {
        return new AzureStorageItemQueueService(
            storageQueueConnectionString,
            queueName,
            loggerFactory.CreateLogger<AzureStorageItemQueueService>());
    }

    loggerFactory.CreateLogger("QueueConfig")
        .LogWarning("Storage queue connection string missing. Using in-memory queue fallback.");
    return new InMemoryItemQueueService();
});

builder.Services.AddHostedService<AddItemQueueWebJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWishList API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("StartupDatabase");

    if (dbContext.Database.IsRelational())
    {
        try
        {
            dbContext.Database.Migrate();
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Database migration failed during startup. Continuing app startup.");
        }
    }
    else
    {
        dbContext.Database.EnsureCreated();
    }
}


app.Run();

public partial class Program;
