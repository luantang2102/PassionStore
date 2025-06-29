using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Extensions;
using PassionStore.Api.Filters;
using PassionStore.Api.SeedData;
using PassionStore.Application.Validators.Requests;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

/// Add services to the container.

// Add Swagger
builder.Services.AddCustomSwagger();

// Add Infrastructure
builder.Services.AddInfrastructure(configuration);

// Add Services
builder.Services.AddApplication(configuration);

// Disable automatic model state error response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add Controller and Validation filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddValidatorsFromAssembly(typeof(ChangePasswordRequestValidator).Assembly);

builder.Services.AddHttpContextAccessor();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:3000", "https://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Exception handling and authentication
builder.Services.AddGlobalExceptionHandling();
builder.Services.AddCustomJwtAuthentication(configuration);




/// App configuration
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Passion Store API v1");
        c.DocumentTitle = "Passion Store API";
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await DbInitializer.InitDb(app);

app.Run();
