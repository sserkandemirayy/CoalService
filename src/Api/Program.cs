using Api.Extensions;
using Api.Security;
using Api.Services;
using Api.Hubs;
using Application.Common.Realtime;
using Application.Common.Behaviors;
using Application.Common.Options;
using Application.Common.Maps;
using Application.Dashboard;
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

builder.Services.AddScoped<IBleAdvertisementEventRepository, BleAdvertisementEventRepository>();
builder.Services.AddScoped<IDioValueEventRepository, DioValueEventRepository>();
builder.Services.AddScoped<ITagDioValueSnapshotRepository, TagDioValueSnapshotRepository>();
builder.Services.AddScoped<II2cDataEventRepository, I2cDataEventRepository>();

builder.Services.AddScoped<ICommandRequestRepository, CommandRequestRepository>();
builder.Services.AddScoped<ICommandStatusHistoryRepository, CommandStatusHistoryRepository>();

builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

builder.Services.AddScoped<IFloorMapRepository, FloorMapRepository>();
builder.Services.AddScoped<IMapFileStorageService, LocalMapFileStorageService>();
builder.Services.AddScoped<IMapCoordinateTransformer, MapCoordinateTransformer>();

builder.Services.AddScoped<IDashboardReadRepository, DashboardReadRepository>();
builder.Services.AddScoped<IMapZoneResolver, MapZoneResolver>();

builder.Services.AddScoped<IntegrationApiKeyFilter>();

builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();

// MediatR - Validation
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

    // Yönetim
    options.AddPolicy("ManageUsers", p => p.RequireClaim("permission", "manage_users"));
    options.AddPolicy("ManageRoles", p => p.RequireClaim("permission", "manage_roles"));
    options.AddPolicy("ManagePermissions", p => p.RequireClaim("permission", "manage_permissions"));

    // =========================
    // ===== MANAGEMENT ========
    // =========================

    options.AddPolicy("ManageUsers",
        p => p.RequireClaim("permission", "manage_users"));

    options.AddPolicy("ManageRoles",
        p => p.RequireClaim("permission", "manage_roles"));

    options.AddPolicy("ManagePermissions",
        p => p.RequireClaim("permission", "manage_permissions"));

    options.AddPolicy("ManageSettings",
        p => p.RequireClaim("permission", "manage_settings"));

    options.AddPolicy("ManageCompanies",
        p => p.RequireClaim("permission", "manage_companies"));

    options.AddPolicy("ManageBranches",
        p => p.RequireClaim("permission", "manage_branches"));

    // =========================
    // ===== DEVICE MGMT =======
    // =========================

    options.AddPolicy("ViewDevices",
        p => p.RequireClaim("permission", "view_devices"));

    options.AddPolicy("ManageTags",
        p => p.RequireClaim("permission", "manage_tags"));

    options.AddPolicy("ManageAnchors",
        p => p.RequireClaim("permission", "manage_anchors"));

    options.AddPolicy("AssignTags",
        p => p.RequireClaim("permission", "assign_tags"));

    options.AddPolicy("ManageDeviceConfigs",
        p => p.RequireClaim("permission", "manage_device_configs"));

    options.AddPolicy("ViewConfigSnapshots",
        p => p.RequireClaim("permission", "view_config_snapshots"));

    // =========================
    // ===== TRACKING ==========
    // =========================

    options.AddPolicy("ViewDashboard",
        p => p.RequireClaim("permission", "view_dashboard"));

    options.AddPolicy("ViewTracking",
        p => p.RequireClaim("permission", "view_tracking"));

    options.AddPolicy("ViewTrackingHistory",
        p => p.RequireClaim("permission", "view_tracking_history"));

    // =========================
    // ===== EVENTS ============
    // =========================

    options.AddPolicy("ViewEvents",
        p => p.RequireClaim("permission", "view_events"));

    options.AddPolicy("ViewRawEvents",
        p => p.RequireClaim("permission", "view_raw_events"));

    // =========================
    // ===== ALARMS ============
    // =========================

    options.AddPolicy("ViewAlarms",
        p => p.RequireClaim("permission", "view_alarms"));

    options.AddPolicy("AcknowledgeAlarms",
        p => p.RequireClaim("permission", "acknowledge_alarms"));

    options.AddPolicy("ManageAlarms",
        p => p.RequireClaim("permission", "manage_alarms"));

    // =========================
    // ===== COMMANDS ==========
    // =========================

    options.AddPolicy("ViewCommands",
        p => p.RequireClaim("permission", "view_commands"));

    options.AddPolicy("CreateCommands",
        p => p.RequireClaim("permission", "create_commands"));

    options.AddPolicy("QueueCommands",
        p => p.RequireClaim("permission", "queue_commands"));

    options.AddPolicy("CancelCommands",
        p => p.RequireClaim("permission", "cancel_commands"));

    options.AddPolicy("RetryCommands",
        p => p.RequireClaim("permission", "retry_commands"));

    // =========================
    // ===== REPORTING / PII ===
    // =========================

    options.AddPolicy("ViewReports",
        p => p.RequireClaim("permission", "view_reports"));

    options.AddPolicy("ViewPii",
        p => p.RequireClaim("permission", "view_pii"));

    options.AddPolicy("EditPii",
        p => p.RequireClaim("permission", "edit_pii"));

    options.AddPolicy("ExportPii",
        p => p.RequireClaim("permission", "export_pii"));
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
app.MapHub<TrackingHub>("/hubs/tracking");
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();