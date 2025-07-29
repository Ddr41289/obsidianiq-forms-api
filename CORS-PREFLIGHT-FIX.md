# CORS Preflight Fix Deployment Guide

## Problem
Preflight OPTIONS requests returning 301 redirects instead of 200 OK, causing CORS failures.

## Root Causes
1. **HTTPS Redirection**: ASP.NET Core redirecting OPTIONS requests before CORS middleware handles them
2. **NGINX Redirects**: Reverse proxy redirecting requests before they reach the application
3. **Trailing Slash**: URL mismatches causing redirects

## Solution Implemented

### 1. ASP.NET Core Changes (Program.cs)
- **Forwarded Headers**: Properly handle X-Forwarded-Proto from NGINX
- **Conditional HTTPS Redirect**: Skip redirects for OPTIONS requests and when behind proxy
- **Global OPTIONS Handler**: Ensure all OPTIONS requests return 200 OK
- **Preflight Caching**: Added 10-minute cache for preflight responses

### 2. NGINX Configuration (nginx-cors.conf)
- **OPTIONS at NGINX Level**: Handle preflight requests before proxying to ASP.NET Core
- **Proper CORS Headers**: Set all required CORS headers at NGINX level
- **No Redirects for OPTIONS**: Prevent any redirects for preflight requests
- **Forward Headers**: Properly forward protocol and origin information

## Deployment Steps

### Step 1: Update ASP.NET Core Application
1. Deploy the updated `Program.cs` with the new CORS handling
2. Ensure `appsettings.Production.json` has the correct configuration:
   ```json
   {
     "CorsPolicy": "AllowObsidianIQFrontend",
     "Urls": "http://0.0.0.0:5000"
   }
   ```

### Step 2: Update NGINX Configuration
1. Backup your current NGINX config:
   ```bash
   sudo cp /etc/nginx/sites-available/api.obsidian-iq.com /etc/nginx/sites-available/api.obsidian-iq.com.backup
   ```

2. Update your NGINX server block with the configuration from `nginx-cors.conf`

3. Test NGINX configuration:
   ```bash
   sudo nginx -t
   ```

4. Reload NGINX:
   ```bash
   sudo systemctl reload nginx
   ```

### Step 3: Test the Fix

#### Test Preflight Request:
```bash
curl -X OPTIONS \
  -H "Origin: https://www.obsidian-iq.com" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: Content-Type" \
  -v https://api.obsidian-iq.com/api/contact
```

**Expected Response:**
- Status: `204 No Content` (from NGINX) or `200 OK` (from ASP.NET Core)
- Headers: Proper CORS headers set
- No redirects (no 301/302)

#### Test Actual POST Request:
```bash
curl -X POST \
  -H "Origin: https://www.obsidian-iq.com" \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Test","email":"test@example.com","message":"Test message"}' \
  -v https://api.obsidian-iq.com/api/contact
```

### Step 4: Monitor and Verify

1. **Check Application Logs:**
   ```bash
   sudo journalctl -u your-api-service -f
   ```
   Look for: `Using CORS policy: AllowObsidianIQFrontend`

2. **Check NGINX Logs:**
   ```bash
   sudo tail -f /var/log/nginx/access.log
   sudo tail -f /var/log/nginx/error.log
   ```

3. **Browser Developer Tools:**
   - Check Network tab for OPTIONS requests
   - Verify no CORS errors in console
   - Confirm 200/204 responses for preflight

## Key Features of This Fix

### ✅ Prevents 301 Redirects
- OPTIONS requests handled before HTTPS redirection
- NGINX handles preflight at proxy level
- Conditional redirection logic

### ✅ Proper CORS Headers
- All required headers set correctly
- Origin validation maintained
- Credentials support preserved

### ✅ Production Ready
- Works with reverse proxy setup
- Handles forwarded headers properly
- Caches preflight responses for performance

### ✅ Fallback Handling
- Multiple layers of OPTIONS handling
- ASP.NET Core backup if NGINX doesn't catch it
- Explicit controller OPTIONS endpoints

## Troubleshooting

### If Still Getting 301s:
1. Check NGINX is handling OPTIONS: `curl -X OPTIONS -v your-api-url`
2. Verify ASP.NET Core isn't redirecting: Check application logs
3. Ensure no trailing slash issues: Test with/without trailing slashes

### If CORS Headers Missing:
1. Verify NGINX config syntax: `sudo nginx -t`
2. Check origin is in allowed list
3. Ensure NGINX is adding headers: Check response headers

### If App Not Accessible:
1. Verify ASP.NET Core is running on 0.0.0.0:5000
2. Check firewall allows port 5000
3. Ensure NGINX can reach backend: `curl http://127.0.0.1:5000/api/contact/health`

This comprehensive fix should resolve all CORS preflight 301 redirect issues in your VPS + NGINX + Kestrel setup.
