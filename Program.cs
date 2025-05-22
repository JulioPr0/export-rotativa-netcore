using Rotativa.AspNetCore;
using RotativaDemo.Data;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IScoreRepository, ScoreRepository>();

var app = builder.Build();

var baseConn = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing DefaultConnection");

var csb = new SqlConnectionStringBuilder(baseConn) {
    InitialCatalog = "master",
    TrustServerCertificate = true,
    Encrypt = false
};
RunDatabaseMigrations(csb.ConnectionString,
    Path.Combine(app.Environment.ContentRootPath, "init-rotativa.sql"));


RotativaConfiguration.Setup(
    app.Environment.WebRootPath,   
    "Rotativa"
);

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

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var masterCsb = new SqlConnectionStringBuilder(baseConn)
{
    InitialCatalog = "master",
    TrustServerCertificate = true,
    Encrypt = false
};

RunDatabaseMigrations(
    masterCsb.ConnectionString,                 
    Path.Combine(app.Environment.ContentRootPath, "init-rotativa.sql")
);

void RunDatabaseMigrations(string defaultConn, string scriptFile)
{
    string sql = File.ReadAllText(scriptFile);

    var batches = System.Text.RegularExpressions.Regex
        .Split(sql, @"^\s*GO\s*$",
            System.Text.RegularExpressions.RegexOptions.Multiline | 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    using var conn = new SqlConnection(defaultConn);

    conn.Open();

    foreach (var batch in batches)
    {
        var trimmed = batch.Trim();
        if (string.IsNullOrEmpty(trimmed))
            continue;

        using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = trimmed;
        cmd.ExecuteNonQuery();
    }

}

app.Run();
