using FluentValidation;
using ObsidianIQ.FormsAPI.Services;
using ObsidianIQ.FormsAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs for production deployment
if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    // Production policy - specific origins for security
    options.AddPolicy("AllowObsidianIQFrontend", policy =>
    {
        policy.WithOrigins(
                "https://www.obsidian-iq.com",
                "https://obsidian-iq.com",
                "http://localhost:3000",  // For local development
                "http://localhost:8000",  // For local development
                "http://127.0.0.1:8000"   // For local development
            )
            .WithMethods("GET", "POST", "OPTIONS")
            .WithHeaders("Content-Type", "Accept", "Authorization")
            .AllowCredentials();
    });
    
    // Development policy - more permissive for local testing
    options.AddPolicy("DevelopmentCORS", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
                origin.StartsWith("http://localhost") || 
                origin.StartsWith("https://localhost") ||
                origin.StartsWith("http://127.0.0.1") ||
                origin.StartsWith("http://0.0.0.0"))
            .WithMethods("GET", "POST", "OPTIONS")
            .WithHeaders("Content-Type", "Accept", "Authorization", "X-Requested-With")
            .AllowCredentials();
    });
    
    // Fallback policy for maximum compatibility (development only)
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
            .WithMethods("GET", "POST", "OPTIONS")
            .WithHeaders("Content-Type", "Accept", "Authorization");
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
Console.WriteLine($"Using CORS policy: {corsPolicy}");

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
