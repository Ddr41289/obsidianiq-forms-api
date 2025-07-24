# ObsidianIQ Forms API

A .NET 8 Web API for handling form submissions from the ObsidianIQ front-end application.

## Features

- **Contact Us Form**: Handles general contact inquiries
- **Work With Us Form**: Handles project collaboration requests
- **Form Validation**: Uses FluentValidation for robust input validation
- **CORS Support**: Configured for ObsidianIQ front-end domains
- **Swagger Documentation**: API documentation available in development
- **Structured Logging**: Comprehensive logging for monitoring and debugging

## API Endpoints

### Contact Form
- `POST /api/contact` - Submit a contact form
- `GET /api/contact/health` - Health check for contact service

### Work With Us Form
- `POST /api/workwithus` - Submit a work with us form
- `GET /api/workwithus/health` - Health check for work with us service

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. Restore dependencies:
   ```bash
   dotnet restore
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Open your browser to `https://localhost:7000/swagger` to view the API documentation

### Configuration

Update the `appsettings.json` file with your email service configuration:

```json
{
  "EmailSettings": {
    "SmtpServer": "your-smtp-server.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-username",
    "SmtpPassword": "your-password",
    "FromEmail": "noreply@obsidianiq.com",
    "ToEmail": "contact@obsidianiq.com"
  }
}
```

## Project Structure

- `Controllers/` - API controllers for handling HTTP requests
- `Models/` - Data models for form submissions and responses
- `Services/` - Business logic services for form processing and email sending
- `Validators/` - FluentValidation validators for form validation

## Next Steps

1. **Email Service**: Implement actual email sending using SendGrid, SMTP, or another email service
2. **Database**: Add Entity Framework Core for persisting form submissions
3. **Rate Limiting**: Implement rate limiting to prevent spam
4. **Authentication**: Add API key authentication if needed
5. **Monitoring**: Add Application Insights or similar monitoring
6. **Deployment**: Configure for deployment to Azure, AWS, or other cloud providers
