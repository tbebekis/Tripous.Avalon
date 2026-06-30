var builder = WebApplication.CreateBuilder(args);
App.Initialize(builder);

// Add services to the container.
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

var WebDemosPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../../WebDemos"));
if (Directory.Exists(WebDemosPath))
{
    var WebDemosProvider = new PhysicalFileProvider(WebDemosPath);
    app.UseDefaultFiles(new DefaultFilesOptions()
    {
        FileProvider = WebDemosProvider,
        RequestPath = "/web-demos"
    });
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = WebDemosProvider,
        RequestPath = "/web-demos"
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
