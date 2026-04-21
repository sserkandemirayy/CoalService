using Api.Extensions;
using Api.Services;
using Application.Common.Behaviors;
using Application.Common.Options;
using Application.Security.Requirements;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Config
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));


builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));

// Db
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();


builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IEncryptionService, DummyEncryptionService>();
builder.Services.AddScoped<IDataRetentionService, DataRetentionService>();



builder.Services.AddScoped<ISettingRepository, SettingRepository>();

builder.Services.AddScoped<IUserCompanyRepository, UserCompanyRepository>();
builder.Services.AddScoped<IUserBranchRepository, UserBranchRepository>();

builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();

builder.Services.AddScoped<IUserSpecializationRepository, UserSpecializationRepository>();

builder.Services.AddScoped<IBarcodeService, BarcodeService>();

builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IAnchorRepository, AnchorRepository>();
builder.Services.AddScoped<ITagAssignmentRepository, TagAssignmentRepository>();

builder.Services.AddScoped<IRawEventRepository, RawEventRepository>();
builder.Services.AddScoped<ILocationEventRepository, LocationEventRepository>();
builder.Services.AddScoped<ICurrentLocationRepository, CurrentLocationRepository>();

builder.Services.AddScoped<IBatteryEventRepository, BatteryEventRepository>();
builder.Services.AddScoped<IImuEventRepository, ImuEventRepository>();
builder.Services.AddScoped<IProximityEventRepository, ProximityEventRepository>();
builder.Services.AddScoped<IEmergencyEventRepository, EmergencyEventRepository>();

builder.Services.AddScoped<IAnchorHeartbeatEventRepository, AnchorHeartbeatEventRepository>();
builder.Services.AddScoped<IAnchorStatusEventRepository, AnchorStatusEventRepository>();

builder.Services.AddScoped<IAlarmRepository, AlarmRepository>();

builder.Services.AddScoped<IAnchorHealthEventRepository, AnchorHealthEventRepository>();
builder.Services.AddScoped<ITagDataEventRepository, TagDataEventRepository>();
builder.Services.AddScoped<IUwbRangingEventRepository, UwbRangingEventRepository>();
builder.Services.AddScoped<IUwbTagToTagRangingEventRepository, UwbTagToTagRangingEventRepository>();

builder.Services.AddScoped<IAnchorConfigEventRepository, AnchorConfigEventRepository>();
builder.Services.AddScoped<IAnchorConfigSnapshotRepository, AnchorConfigSnapshotRepository>();

builder.Services.AddScoped<IBleConfigEventRepository, BleConfigEventRepository>();
builder.Services.AddScoped<ITagBleConfigSnapshotRepository, TagBleConfigSnapshotRepository>();

builder.Services.AddScoped<IUwbConfigEventRepository, UwbConfigEventRepository>();
builder.Services.AddScoped<ITagUwbConfigSnapshotRepository, TagUwbConfigSnapshotRepository>();

builder.Services.AddScoped<IDioConfigEventRepository, DioConfigEventRepository>();
builder.Services.AddScoped<ITagDioConfigSnapshotRepository, TagDioConfigSnapshotRepository>();

builder.Services.AddScoped<II2cConfigEventRepository, I2cConfigEventRepository>();
builder.Services.AddScoped<ITagI2cConfigSnapshotRepository, TagI2cConfigSnapshotRepository>();

// MediatR + Validation
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Application.Auth.Commands.RegisterUserCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Infrastructure.Repositories.UnitOfWork).Assembly);
});

builder.Services.AddValidatorsFromAssembly(
    typeof(Application.ApplicationAssemblyMarker).Assembly
);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Auth
var jwtSection = builder.Configuration.GetSection("JWT");
var secret = jwtSection.GetValue<string>("Secret") ?? "super_long_development_secret_change_me";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

Console.WriteLine("JWT Issuer: " + jwtSection.GetValue<string>("Issuer"));
Console.WriteLine("JWT Audience: " + jwtSection.GetValue<string>("Audience"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.GetValue<string>("Issuer"),
            ValidAudience = jwtSection.GetValue<string>("Audience"),
            IssuerSigningKey = key
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Özel requirement örneği
    options.AddPolicy("AccessOwnPatientData", p => p.Requirements.Add(new SameUserRequirement()));

    // Yönetim
    options.AddPolicy("ManageUsers", p => p.RequireClaim("permission", "manage_users"));
    options.AddPolicy("ManageRoles", p => p.RequireClaim("permission", "manage_roles"));
    options.AddPolicy("ManagePermissions", p => p.RequireClaim("permission", "manage_permissions"));

    // Raporlama
    options.AddPolicy("ViewReports", p => p.RequireClaim("permission", "view_reports"));

    // Hasta verileri
    options.AddPolicy("AccessPatientData", p => p.RequireClaim("permission", "access_patient_data"));
    options.AddPolicy("EditPatientData", p => p.RequireClaim("permission", "edit_patient_data"));
    options.AddPolicy("DeletePatientData", p => p.RequireClaim("permission", "delete_patient_data"));
    options.AddPolicy("ManagePatients", p => p.RequireClaim("permission", "manage_patients"));

    // Randevu
    options.AddPolicy("CreateAppointment", p => p.RequireClaim("permission", "create_appointment"));

    // Şirket / Şube
    options.AddPolicy("ManageCompanies", p => p.RequireClaim("permission", "manage_companies"));
    options.AddPolicy("ManageBranches", p => p.RequireClaim("permission", "manage_branches"));

    // Servisler
    options.AddPolicy("ManageServices", p => p.RequireClaim("permission", "manage_services"));

    // Ödemeler
    options.AddPolicy("ManagePayments", p => p.RequireClaim("permission", "manage_payments"));
    options.AddPolicy("ViewPayments", p => p.RequireClaim("permission", "view_payments"));

    // Giderler
    options.AddPolicy("ManageExpenses", p => p.RequireClaim("permission", "manage_expenses"));
    options.AddPolicy("ViewExpenses", p => p.RequireClaim("permission", "view_expenses"));

    // Ürün / Depo
    options.AddPolicy("ManageProducts", p => p.RequireClaim("permission", "manage_products"));
    options.AddPolicy("ManageWarehouses", p => p.RequireClaim("permission", "manage_warehouses"));

    // Performans
    options.AddPolicy("ManagePerformance", p => p.RequireClaim("permission", "manage_performance"));

    // =========================
    // ===== ROOM POLICIES =====
    // =========================
    options.AddPolicy("room:view", p => p.RequireClaim("permission", "room:view"));
    options.AddPolicy("room:manage", p => p.RequireClaim("permission", "room:manage"));
    options.AddPolicy("room:assignEquipment", p => p.RequireClaim("permission", "room:assignEquipment"));
    options.AddPolicy("room:assignStaff", p => p.RequireClaim("permission", "room:assignStaff"));
    options.AddPolicy("room:maintain", p => p.RequireClaim("permission", "room:maintain"));

    options.AddPolicy("ManageCampaigns", p => p.RequireClaim("permission", "manage_campaigns"));
});

// --- CORS POLICY ---
const string FrontendCors = "FrontendCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:4173",
                "https://localhost:4173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
        // .AllowCredentials(); 
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Coal Core API", Version = "v1" });

    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

    // JWT Bearer için security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Örnek: \"Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddControllers();

var app = builder.Build();

// Apply migrations and seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedData.Initialize(db);
}

// HTTPS yönlendirme
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global hata yönetimi
app.UseGlobalExceptionHandling();

// CORS — auth'tan ÖNCE
app.UseCors(FrontendCors);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();