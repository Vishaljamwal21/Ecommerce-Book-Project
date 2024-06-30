# Ecommerce-Book-Project

This is an E-commerce project built using ASP.NET Core and C# with a Code-First approach. It utilizes Identity for authentication, supports Google and Facebook OAuth logins, integrates Stripe for payment processing, and provides email confirmation and SMS verification using Twilio.

## Features
- ASP.NET Core with C#
- Code-First Approach using Entity Framework Core
- ASP.NET Core Identity for authentication
- Google and Facebook OAuth login
- Stripe Payment Gateway Integration
- Email confirmation
- SMS confirmation using Twilio
- Role-based access control (Admin, Individual User, Company, and Employee)

## Getting Started

### Prerequisites
- Visual Studio 2022
- .NET 6.0 SDK
- SQL Server or any preferred database

### Setup Instructions

1. **Clone the Repository**:
   git clone https://github.com/Vishaljamwal21/Ecommerce-Book-Project.git
   cd Ecommerce-Book-Project

2.**Open the Solution**:
  Open the solution in Visual Studio 2022.

3. **Set Up the Database**:
. Open the Package Manager Console from Tools > NuGet Package Manager > Package Manager Console.
. Select the Project_Ecomm_1.DataAccess project in the console.
  Add-Migration InitialMigration
  Update-Database

4. **Configure Environment Variables**:
. Ensure you have the following environment variables set in your development environment for sensitive information:

- TWILIO_ACCOUNT_SID
- TWILIO_AUTH_TOKEN
- GOOGLE_CLIENT_ID
- GOOGLE_CLIENT_SECRET
- FACEBOOK_APP_ID
- FACEBOOK_APP_SECRET
- STRIPE_SECRET_KEY
- EMAIL_SENDER

5. **Run the Project**:
. Set Project_Ecomm_1 as the startup project.
.Run the project using Ctrl + F5.

6. **Usage**
1. Register the First User:
. The first registered user will be assigned the Admin role.
. Subsequent users will be assigned the Individual role by default.

2. Admin Functionality:
. Admin can add companies.
. Admin can register company users and employees.

3. Adding Products:
. Company users can add products.
. Employees can assist in managing the company's products.

4. Shopping:
. Individual users can browse and shop for products.

**Project Structure**
. Project_Ecomm_1: Main project with controllers, views, and other frontend components.
. Project_Ecomm_1.DataAccess: Contains the Entity Framework Core context, models, and migrations.
. Project_Ecomm_1.Models: Contains the domain models.
. Project_Ecomm_1.Utility: Contains utility classes and constants.

**Technologies Used**
. ASP.NET Core: Web framework
. Entity Framework Core: ORM for database interactions
. ASP.NET Core Identity: Authentication and authorization
. Google and Facebook OAuth: Third-party authentication
. Stripe: Payment gateway integration
. Twilio: SMS service for phone number verification

**Configuration**
 appsettings.json:
 {
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string here"
  },
  "Twilio": {
    "AccountSid": "Your Twilio Account SID here",
    "AuthToken": "Your Twilio Auth Token here"
  },
  "Stripe": {
    "SecretKey": "Your Stripe Secret Key here"
  },
  "EmailSettings": {
    "Sender": "Your email sender address here",
    "SMTPServer": "Your SMTP server here",
    "SMTPPort": "Your SMTP port here",
    "SMTPUsername": "Your SMTP username here",
    "SMTPPassword": "Your SMTP password here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*"
}
**Update Program.cs for OAuth Configuration**:
. In Program.cs, add the following configurations:
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
    })
    .AddFacebook(options =>
    {
        options.AppId = Environment.GetEnvironmentVariable("FACEBOOK_APP_ID");
        options.AppSecret = Environment.GetEnvironmentVariable("FACEBOOK_APP_SECRET");
    });

**Contact**
For any queries or issues, please contact Vishal Jamwal at [vishaljamwal402@gmail.com].