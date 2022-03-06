using SmartHealthCard.JwksApi.JwksSupport;
using SmartHealthCard.JwksApi.CertificateSupport;
using SmartHealthCard.JwksApi;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

string CORSAllowOriginPolicy = "allow-origin-policy";
builder.Services.AddCors(options =>
{
  options.AddPolicy(CORSAllowOriginPolicy, PolicyBuilder => PolicyBuilder.WithOrigins().SetIsOriginAllowed(origin => true));
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<List<CertificateThumbprint>>(builder.Configuration.GetSection("CertificateThumbprintList"));
builder.Services.AddSingleton<IJwksJsonProvider, JwksJsonProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CORSAllowOriginPolicy);

app.MapGet("/well-known/jwks.json", ([FromServices] IJwksJsonProvider CertificateProvider) =>
{
  try
  {    
    return Results.Content(CertificateProvider.GetJwksJson(), "application/json");    
  }
  catch (CertificateLoadException CertificateLoadException)
  {
    return Results.BadRequest(new ErrorOutcome("CertificateLoadError", CertificateLoadException.Message));    
  }
})
.WithName(".well-known/jwks.json")
.RequireCors(CORSAllowOriginPolicy);

app.Run();
