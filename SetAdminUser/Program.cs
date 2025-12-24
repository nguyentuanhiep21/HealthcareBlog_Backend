using HealthCareBlog_Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

var app = builder.Build();

// Set admin user
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    Console.WriteLine("=== Current Users ===");
    var users = await context.Users.ToListAsync();
    foreach (var u in users)
    {
        Console.WriteLine($"ID: {u.Id}, Name: {u.FullName}, Email: {u.Email}, IsAdmin: {u.IsAdmin}");
    }
    
    Console.WriteLine("\n=== Setting Admin ===");
    Console.Write("Enter email to set as admin: ");
    var email = Console.ReadLine();
    
    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user != null)
    {
        user.IsAdmin = true;
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ {user.FullName} is now an admin!");
    }
    else
    {
        Console.WriteLine($"✗ User with email {email} not found!");
    }
    
    Console.WriteLine("\n=== Admin Users ===");
    var admins = await context.Users.Where(u => u.IsAdmin).ToListAsync();
    foreach (var a in admins)
    {
        Console.WriteLine($"- {a.FullName} ({a.Email})");
    }
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
