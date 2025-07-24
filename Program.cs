using FluentValidation;
using ObsidianIQ.FormsAPI.Services;
using ObsidianIQ.FormsAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowObsidianIQFrontend", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:3000", 
                "http://localhost:5000",
                "https://localhost:7000", 
                "https://obsidianiq.com", 
                "https://www.obsidianiq.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    
    // Add a more permissive policy for development
    options.AddPolicy("DevelopmentCORS", builder =>
    {
        builder
            .SetIsOriginAllowed(origin => 
                origin.StartsWith("http://localhost") || 
                origin.StartsWith("https://localhost"))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    
    // Add wildcard policy for maximum flexibility (use with caution)
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
            // Note: Cannot use .AllowCredentials() with .AllowAnyOrigin()
    });
});

// Add services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFormService, FormService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ContactFormValidator>();

var app = builder.Build();

// Get CORS policy from configuration
var corsPolicy = builder.Configuration.GetValue<string>("CorsPolicy") ?? "DevelopmentCORS";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Only use HTTPS redirection in production
    // In development, allow both HTTP and HTTPS
}
else
{
    app.UseHttpsRedirection();
}

// Use configured CORS policy
app.UseCors(corsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
