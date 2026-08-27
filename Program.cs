using IglesiaBackend.Data;
using IglesiaBackend.Shared.Extensions;
using IglesiaBackend.Shared.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Agregado para OpenApi models explícitos
using System.Text;

//Si te mando una fecha sin zona horaria, no te quejes y guárdala como si fuera UTC
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// =======================================
// DbContext
// =======================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention()
);

// =======================================
// Feature services (repos + services)
// =======================================
// Aquí se cargan todos tus nuevos repositorios y servicios (Organization, etc.)
builder.Services.AddFeatureServices();

// =======================================
// Scrutor (opcional, no estorba)
// =======================================
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
        .AddClasses(classes => classes.InNamespaces("IglesiaBackend.Features"))
        .AsSelf()
        .WithScopedLifetime()
);

// =======================================
// JWT Authentication (12h, minimalista)
// =======================================
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

// =======================================
// Authorization
// =======================================
builder.Services.AddAuthorization();

// =======================================
// Controllers + Swagger
// =======================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto permite que "organizationTypeId" se mapee a "OrganizationTypeId" sin dolor
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddHttpContextAccessor();

// HABILITAR CORS (Vital para que Flutter conecte)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// =======================================
// Pipeline
// =======================================
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//// =======================================
//// Seed inicial (admin)
//// =======================================
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    // NOTA: Recuerda actualizar el DatabaseSeeder con las nuevas entidades (OrganizationStructure)
//    // antes de descomentar esto, o te dará error.
//    // await DatabaseSeeder.SeedAsync(db);
//}

app.UseHttpsRedirection();

// ACTIVAR CORS AQUÍ
app.UseCors("AllowAll");

// ORDEN CRÍTICO
app.UseAuthentication();
app.UseMiddleware<UserContextMiddleware>(); // ¡Perfecto! Justo después de Auth para capturar el ID
app.UseAuthorization();

app.MapControllers();

app.Run();