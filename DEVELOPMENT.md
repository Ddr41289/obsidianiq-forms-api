# Development SSL Certificate Issues - Solutions

## Problem
When your front-end tries to call `https://localhost:5000/api/contact`, you get `net::ERR_SSL_PROTOCOL_ERROR`.

## Solutions

### Option 1: Use HTTP for Local Development (Recommended)
Your API is now configured to run on HTTP during development. Update your front-end to call:
```
http://localhost:5000/api/contact
```

### Option 2: Generate Development SSL Certificate
If you need HTTPS, run these commands to trust the .NET development certificate:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Then run the API with HTTPS:
```powershell
dotnet run --launch-profile https
```

### Option 3: Browser-Specific Solutions

#### For Chrome/Edge:
1. Navigate to `chrome://flags/#allow-insecure-localhost`
2. Enable "Allow invalid certificates for resources loaded from localhost"
3. Restart browser

#### For Firefox:
1. Navigate to `about:config`
2. Set `security.tls.insecure_fallback_hosts` to `localhost`

### Option 4: Programmatic Bypass (Front-end)
If using fetch in your front-end, you can't programmatically bypass SSL errors due to browser security. However, you can:

1. Use HTTP endpoint during development
2. Use environment variables to switch between HTTP/HTTPS based on environment

## Current API Configuration
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:7000` (when using https profile)
- Swagger UI: Available at `/swagger` endpoint

### CORS Configuration
The API is configured with proper CORS headers:

**Development Environment:**
- Allows any localhost origin (http://localhost:* and https://localhost:*)
- Includes `Access-Control-Allow-Origin` header
- Supports credentials and any methods/headers

**Production Environment:**
- Specific allowed origins: obsidianiq.com, www.obsidianiq.com
- Secure CORS policy

**Test CORS Endpoint:**
- GET `http://localhost:5000/api/test` - Test CORS configuration
- POST `http://localhost:5000/api/test` - Test CORS for form submissions

## Running the API
```powershell
# HTTP only (no SSL issues)
dotnet run --launch-profile http

# HTTPS (requires trusted certificate)
dotnet run --launch-profile https
```
