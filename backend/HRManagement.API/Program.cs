using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using HRManagement.Infrastructure;
using HRManagement.Application;
using Microsoft.EntityFrameworkCore;
using HRManagement.API.Services;
using HRManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HR Management API", Version = "v1" });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:3000", "https://localhost:3000",
                  "http://localhost:3001", "https://localhost:3001")
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add Infrastructure and Application services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Create admin user on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<HRManagement.Infrastructure.Data.HRManagementDbContext>();
        // Ensure database and all migrations are applied on startup
        await context.Database.MigrateAsync();
        
        // Check if admin exists
        var existingAdmin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        
        if (existingAdmin == null)
        {
            var adminUser = new HRManagement.Core.Entities.User
            {
                Id = "admin-001",
                Username = "admin",
                Email = "admin@congty.com",
                FullName = "Nguyễn Văn Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Phone = "0123456789",
                Address = "123 Đường Lê Lợi, Quận 1, TP.HCM",
                Gender = "Nam",
                DateOfBirth = new DateTime(1985, 1, 15),
                Department = "Công nghệ thông tin",
                Position = "Quản trị hệ thống",
                HireDate = new DateTime(2020, 1, 1),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
            
            Console.WriteLine("✅ Admin user created successfully!");
            Console.WriteLine("Username: admin");
            Console.WriteLine("Password: admin123");
        }
        else
        {
            Console.WriteLine("ℹ️  Admin user already exists!");
        }
        
        // Do not seed sample employees; we want a clean DB with only Admin
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error creating admin user: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Disable extra seeders to keep database clean (only admin created above)

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
