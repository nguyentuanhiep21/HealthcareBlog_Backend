using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using HealthCareBlog_Backend.Interfaces;
using HealthCareBlog_Backend.Repositories;
using HealthCareBlog_Backend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataProtection();

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    // Password requirements
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HealthCareBlog API",
        Description = "API cho hệ thống blog chăm sóc sức khỏe",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@healthcareblog.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".HealthCareBlog.Session";
});

// Register Services
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ISavedPostService, SavedPostService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserLockService, UserLockService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<ISupabaseStorageService, SupabaseStorageService>();
builder.Services.AddSingleton<IHealthAssessmentService, HealthAssessmentService>();
builder.Services.AddScoped<IMealSuggestionService, MealSuggestionService>();

// Register Repositories
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<ISavedPostRepository, SavedPostRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<INutritionRepository, NutritionRepository>();
builder.Services.AddScoped<IMealSuggestionRepository, MealSuggestionRepository>();

// Register Middleware
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

// Configure Supabase
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? Environment.GetEnvironmentVariable("Supabase__Url");
var supabaseKey = builder.Configuration["Supabase:Key"] ?? Environment.GetEnvironmentVariable("Supabase__Key");

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
{
    throw new InvalidOperationException("Supabase URL or Key is missing. Please ensure 'Supabase__Url' and 'Supabase__Key' are set in Environment Variables (Render) or in appsettings.json.");
}

var options = new Supabase.SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
};
var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, options);
builder.Services.AddSingleton(supabaseClient);

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnectionString"),
        npgsqlOptions =>
        {
            // Enable automatic retry on transient failures (network timeouts, connection pool exhaustion)
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null
            );
            
            // Set command execution timeout
            npgsqlOptions.CommandTimeout(30);
            
            // Only disable certificate validation in development. Use proper certs in production
            if (!builder.Environment.IsProduction())
            {
                npgsqlOptions.RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);
            }
        }
    );
    
    // Enable query tracking for debugging (can be disabled in production for performance)
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
                origin.StartsWith("http://localhost:") || 
                origin.StartsWith("https://localhost:") || 
                origin.EndsWith(".vercel.app") ||
                origin == "http://10.0.2.2:5216" ||
                origin == "https://10.0.2.2:7223" ||
                origin == "https://healthcareblog-backend.onrender.com"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Render injects PORT env variable; fallback to 5216 for local dev
var port = Environment.GetEnvironmentVariable("PORT") ?? "5216";
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Any, int.Parse(port));
});

var app = builder.Build();

// Configure CORS - Must be before other middleware
app.UseCors("AllowSpecific");

// Global Exception Handler — phải đứng đầu pipeline để bắt mọi exception
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthCareBlog API v1");
        c.RoutePrefix = "swagger";

        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
    });
}

app.UseStaticFiles();
app.UseSession();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
