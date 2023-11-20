using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using webProject2.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<webProject2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("webProject2Context") ?? throw new InvalidOperationException("Connection string 'webProject2Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(5); });


var app = builder.Build();
app.UseCors("_myAllowSpecificOrigins");
app.UseHttpsRedirection();
app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");
app.UseSession();

app.UseCors("_myAllowSpecificOrigins");
app.UseHttpsRedirection();
app.UseStaticFiles();


app.Run();
