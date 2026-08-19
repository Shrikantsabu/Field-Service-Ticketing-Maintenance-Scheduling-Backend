using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Data;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Middleware;
using Field_Service_Ticketing___Maintenance_Scheduling_Backend.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddHostedService<EscalationBackgroundService>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Add DbContext - Connection String of Database 
builder.Services.AddDbContext<FieldOpsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FieldOps API V1");
        c.RoutePrefix = "docs"; 
    });
}


builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

