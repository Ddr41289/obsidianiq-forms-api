using FluentValidation;
using ObsidianIQ.FormsAPI.Services;
using ObsidianIQ.FormsAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs for production deployment
if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls("http://127.0.0.1:5000");
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
            .WithHeaders("Content-Type", "Accept", "Authorization", "X-Requested-With")
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10)); // Cache preflight for 10 minutes
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
    // Configure for production with reverse proxy
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                          Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });
    
    // Only redirect to HTTPS if the request isn't already HTTPS
    // This prevents 301 redirects on preflight requests from NGINX
    app.Use(async (context, next) =>
    {
        if (!context.Request.IsHttps && 
            context.Request.Method != "OPTIONS" && 
            !context.Request.Headers.ContainsKey("X-Forwarded-Proto"))
        {
            var httpsUrl = $"https://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
            context.Response.Redirect(httpsUrl, permanent: true);
            return;
        }
        await next();
    });
}

// Handle OPTIONS requests globally BEFORE CORS middleware
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        // Let CORS middleware handle the response headers
        await next();
        
        // Ensure we return 200 OK for preflight requests
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 200;
            await context.Response.CompleteAsync();
        }
        return;
    }
    await next();
});

// Use configured CORS policy
app.UseCors(corsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
