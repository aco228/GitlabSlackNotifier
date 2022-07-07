using GitlabSlackNotifier.Api.Middleware;
using GitlabSlackNotifier.Core;
using GitlabSlackNotifier.Core.Services.Jira;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging(b =>
{
    b.ClearProviders();
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Trace);
});
builder.Services.RegisterDeserializers();
builder.Services.RegisterConfigurations();
builder.Services.RegisterPersistency();
builder.Services.RegisterGitlabServices();
builder.Services.RegisterSlackServices();
builder.Services.RegisterJiraServices();
builder.Services.RegisterZenQuote();
builder.Services.RegisterUtilities();

var app = builder.Build();

// app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();
app.RegisterSlackCommands();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllers();
});
app.Run();