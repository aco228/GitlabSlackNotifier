using GitlabSlackNotifier.Core;
using GitlabSlackNotifier.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.RegisterDeserializers();
builder.Services.RegisterConfigurations();
builder.Services.RegisterGitlabServices();
builder.Services.RegisterSlackServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.RegisterSlackCommands();
app.UseEndpoints(endpoints =>
{
    //endpoints.MapControllerRoute("default", "{controller=Account}/{action=login}/{id?}");
                
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllers();
});
app.Run();