using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
  
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();

    public DbSet<UserType> UserTypes => Set<UserType>();
    public DbSet<UserSpecialization> UserSpecializations => Set<UserSpecialization>();


    //COAL

    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Anchor> Anchors => Set<Anchor>();
    public DbSet<TagAssignment> TagAssignments => Set<TagAssignment>();

    public DbSet<RawEvent> RawEvents => Set<RawEvent>();
    public DbSet<LocationEvent> LocationEvents => Set<LocationEvent>();
    public DbSet<CurrentLocation> CurrentLocations => Set<CurrentLocation>();

    public DbSet<BatteryEvent> BatteryEvents => Set<BatteryEvent>();
    public DbSet<ImuEvent> ImuEvents => Set<ImuEvent>();
    public DbSet<ProximityEvent> ProximityEvents => Set<ProximityEvent>();
    public DbSet<EmergencyEvent> EmergencyEvents => Set<EmergencyEvent>();

    public DbSet<AnchorHeartbeatEvent> AnchorHeartbeatEvents => Set<AnchorHeartbeatEvent>();
    public DbSet<AnchorStatusEvent> AnchorStatusEvents => Set<AnchorStatusEvent>();

    public DbSet<Alarm> Alarms => Set<Alarm>();

    public DbSet<AnchorHealthEvent> AnchorHealthEvents => Set<AnchorHealthEvent>();
    public DbSet<TagDataEvent> TagDataEvents => Set<TagDataEvent>();
    public DbSet<UwbRangingEvent> UwbRangingEvents => Set<UwbRangingEvent>();
    public DbSet<UwbTagToTagRangingEvent> UwbTagToTagRangingEvents => Set<UwbTagToTagRangingEvent>();

    public DbSet<AnchorConfigEvent> AnchorConfigEvents => Set<AnchorConfigEvent>();
    public DbSet<AnchorConfigSnapshot> AnchorConfigSnapshots => Set<AnchorConfigSnapshot>();

    public DbSet<BleConfigEvent> BleConfigEvents => Set<BleConfigEvent>();
    public DbSet<TagBleConfigSnapshot> TagBleConfigSnapshots => Set<TagBleConfigSnapshot>();

    public DbSet<UwbConfigEvent> UwbConfigEvents => Set<UwbConfigEvent>();
    public DbSet<TagUwbConfigSnapshot> TagUwbConfigSnapshots => Set<TagUwbConfigSnapshot>();

    public DbSet<DioConfigEvent> DioConfigEvents => Set<DioConfigEvent>();
    public DbSet<TagDioConfigSnapshot> TagDioConfigSnapshots => Set<TagDioConfigSnapshot>();

    public DbSet<I2cConfigEvent> I2cConfigEvents => Set<I2cConfigEvent>();
    public DbSet<TagI2cConfigSnapshot> TagI2cConfigSnapshots => Set<TagI2cConfigSnapshot>();

    public DbSet<BleAdvertisementEvent> BleAdvertisementEvents => Set<BleAdvertisementEvent>();
    public DbSet<DioValueEvent> DioValueEvents => Set<DioValueEvent>();
    public DbSet<TagDioValueSnapshot> TagDioValueSnapshots => Set<TagDioValueSnapshot>();
    public DbSet<I2cDataEvent> I2cDataEvents => Set<I2cDataEvent>();

    public DbSet<CommandRequest> CommandRequests => Set<CommandRequest>();
    public DbSet<CommandStatusHistory> CommandStatusHistories => Set<CommandStatusHistory>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
        base.OnModelCreating(modelBuilder);

        // ==== Basit Base64 converter (prod için AES/HSM önerilir) ====
        var encryptConverter = new ValueConverter<string?, string?>(
            v => v == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(v)),
            v => v == null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(v))
        );

        // ==== User ====       
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.NationalIdEncrypted).HasConversion(encryptConverter);
            entity.Property(u => u.PhoneEncrypted).HasConversion(encryptConverter);
            entity.Property(u => u.AddressEncrypted).HasConversion(encryptConverter);
            entity.Property(u => u.EmergencyContactNameEncrypted).HasConversion(encryptConverter);
            entity.Property(u => u.EmergencyContactPhoneEncrypted).HasConversion(encryptConverter);
            entity.Property(u => u.IsActive).HasDefaultValue(true);

            entity.HasMany(u => u.UserRoles)
                  .WithOne(ur => ur.User)
                  .HasForeignKey(ur => ur.UserId);

            entity.HasMany(u => u.RefreshTokens)
                  .WithOne(rt => rt.User)
                  .HasForeignKey(rt => rt.UserId);

            // === NEW ===
            entity.HasOne(u => u.UserType)
                  .WithMany(t => t.Users)
                  .HasForeignKey(u => u.UserTypeId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();

            entity.HasOne(u => u.Specialization)
                  .WithMany(s => s.Users)
                  .HasForeignKey(u => u.SpecializationId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ==== UserType ====
        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.IsSystem).HasDefaultValue(false);
        });
 

        // ==== Role ====
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Name).IsUnique();

            entity.HasMany(r => r.UserRoles)
                  .WithOne(ur => ur.Role)
                  .HasForeignKey(ur => ur.RoleId);

            entity.HasMany(r => r.RolePermissions)
                  .WithOne(rp => rp.Role)
                  .HasForeignKey(rp => rp.RoleId);
        });

        // ==== Permission ====
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.Name).IsUnique();

            entity.HasMany(p => p.RolePermissions)
                  .WithOne(rp => rp.Permission)
                  .HasForeignKey(rp => rp.PermissionId);
        });

        // ==== UserRole ====
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => ur.Id);
            entity.HasIndex(x => new { x.UserId, x.RoleId })
                  .IsUnique()
                  .HasFilter("\"DeletedAt\" IS NULL");

            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId);
        });

        // ==== RolePermission ====
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => rp.Id);
            entity.HasIndex(x => new { x.RoleId, x.PermissionId })
                  .IsUnique()
                  .HasFilter("\"DeletedAt\" IS NULL");

            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.RoleId);

            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(rp => rp.PermissionId);
        });

        // ==== RefreshToken ====
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
        });

        // ==== AuditLog ====
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Action).IsRequired().HasMaxLength(200);
            entity.Property(a => a.ResourceType).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Detail).HasColumnType("jsonb");
        });

        // ==== Company ====
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.TaxNumber).IsUnique();

            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.TaxNumber).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Address).HasMaxLength(500);
            entity.Property(c => c.Phone).HasMaxLength(50);
            entity.Property(c => c.Email).HasMaxLength(150);

            entity.Property<string?>("LogoUrl").HasMaxLength(300).IsRequired(false);
        });

        // ==== UserCompany ==== (N-N iliþki)
        modelBuilder.Entity<UserCompany>(entity =>
        {
            entity.HasKey(uc => uc.Id);

            entity.HasIndex(uc => new { uc.UserId, uc.CompanyId })
               .IsUnique();      
            entity.HasOne(x => x.User)
                  .WithMany(u => u.UserCompanies)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Company)
                  .WithMany(c => c.UserCompanies)
                  .HasForeignKey(x => x.CompanyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => x.DeletedAt == null);
        });

              
        
        // ==== Branch ====
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Address).HasMaxLength(500);
            entity.Property(b => b.Phone).HasMaxLength(50);
            entity.Property(b => b.Email).HasMaxLength(150);

            entity.HasOne(b => b.Company)
                  .WithMany(c => c.Branches)
                  .HasForeignKey(b => b.CompanyId);
        });

        // ==== UserBranch ==== (N-N iliþki)
        modelBuilder.Entity<UserBranch>(entity =>
        {
            entity.HasKey(ub => ub.Id); 

            entity.HasIndex(ub => new { ub.UserId, ub.BranchId })
                .IsUnique();

            entity.HasOne(ub => ub.User)
                .WithMany(u => u.UserBranches)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ub => ub.Branch)
                .WithMany(b => b.UserBranches)
                .HasForeignKey(ub => ub.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => x.DeletedAt == null);
        });

        modelBuilder.Entity<UserSpecialization>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Code).IsUnique();

            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.IsSystem).HasDefaultValue(false);

            entity.HasOne(x => x.UserType)
                  .WithMany(t => t.Specializations)
                  .HasForeignKey(x => x.UserTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

       

        modelBuilder.Entity<Setting>().HasKey(x => x.Id);

        // ==== Tag ====
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ExternalId).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Code).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.SerialNumber).HasMaxLength(200);
            entity.Property(x => x.MetadataJson).HasColumnType("jsonb");
            entity.Property(x => x.TagType).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity.HasIndex(x => x.ExternalId)
                  .IsUnique()
                  .HasFilter("\"DeletedAt\" IS NULL");

            entity.HasIndex(x => x.Code)
                  .IsUnique()
                  .HasFilter("\"DeletedAt\" IS NULL");

            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.LastSeenAt);
            entity.HasIndex(x => x.LastEventAt);
        });

        // ==== Anchor ====
        modelBuilder.Entity<Anchor>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ExternalId).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Code).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.IpAddress).HasMaxLength(100);
            entity.Property(x => x.MetadataJson).HasColumnType("jsonb");
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity.HasIndex(x => x.ExternalId)
                  .IsUnique()
                  .HasFilter("\"DeletedAt\" IS NULL");

            entity.HasIndex(x => x.Code)
                  .IsUnique()
                  .HasFilter("\"DeletedAt\" IS NULL");

            entity.HasIndex(x => x.IpAddress);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.LastHeartbeatAt);
            entity.HasIndex(x => x.LastStatusChangedAt);

            entity.HasOne(x => x.Company)
                  .WithMany()
                  .HasForeignKey(x => x.CompanyId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Branch)
                  .WithMany()
                  .HasForeignKey(x => x.BranchId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ==== TagAssignment ====
        modelBuilder.Entity<TagAssignment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssignedAt).IsRequired();
            entity.Property(x => x.IsPrimary).HasDefaultValue(true);

            entity.HasIndex(x => new { x.TagId, x.UserId, x.AssignedAt });

            entity.HasIndex(x => x.TagId)
                  .IsUnique()
                  .HasFilter("\"UnassignedAt\" IS NULL AND \"DeletedAt\" IS NULL");

            entity.HasIndex(x => x.UserId)
                  .HasFilter("\"UnassignedAt\" IS NULL AND \"DeletedAt\" IS NULL");

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.Assignments)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== RawEvent ====
        modelBuilder.Entity<RawEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ExternalEventId).IsRequired();
            entity.Property(x => x.EventType).IsRequired().HasMaxLength(150);
            entity.Property(x => x.TagExternalId).HasMaxLength(200);
            entity.Property(x => x.AnchorExternalId).HasMaxLength(200);
            entity.Property(x => x.PayloadJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.ErrorMessage).HasMaxLength(2000);
            entity.Property(x => x.ProcessingStatus).HasConversion<string>().HasMaxLength(50);

            entity.HasIndex(x => x.ExternalEventId).IsUnique();
            entity.HasIndex(x => x.EventType);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => x.TagExternalId);
            entity.HasIndex(x => x.AnchorExternalId);
            entity.HasIndex(x => x.ProcessingStatus);
            entity.HasIndex(x => x.ReceivedAt);
        });

        // ==== LocationEvent ====
        modelBuilder.Entity<LocationEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.X).HasPrecision(18, 6);
            entity.Property(x => x.Y).HasPrecision(18, 6);
            entity.Property(x => x.Z).HasPrecision(18, 6);
            entity.Property(x => x.Accuracy).HasPrecision(18, 6);
            entity.Property(x => x.Confidence).HasPrecision(5, 2);
            entity.Property(x => x.UsedAnchorsJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.LocationEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== CurrentLocation ====
        modelBuilder.Entity<CurrentLocation>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.X).HasPrecision(18, 6);
            entity.Property(x => x.Y).HasPrecision(18, 6);
            entity.Property(x => x.Z).HasPrecision(18, 6);
            entity.Property(x => x.Accuracy).HasPrecision(18, 6);
            entity.Property(x => x.Confidence).HasPrecision(5, 2);

            entity.HasIndex(x => x.TagId).IsUnique();
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.LastEventAt);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.CurrentLocations)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ==== BatteryEvent ====
        modelBuilder.Entity<BatteryEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.BatteryEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                  .WithMany(x => x.BatteryEvents)
                  .HasForeignKey(x => x.AnchorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== ImuEvent ====
        modelBuilder.Entity<ImuEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.EventType).HasConversion<string>().HasMaxLength(50);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => x.EventType);

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.ImuEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== ProximityEvent ====
        modelBuilder.Entity<ProximityEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Distance).HasPrecision(18, 6);
            entity.Property(x => x.Threshold).HasPrecision(18, 6);
            entity.Property(x => x.Severity).HasConversion<string>().HasMaxLength(50);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.PeerTagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => x.Severity);

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.PrimaryProximityEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.PeerTag)
                  .WithMany(x => x.PeerProximityEvents)
                  .HasForeignKey(x => x.PeerTagId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== EmergencyEvent ====
        modelBuilder.Entity<EmergencyEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.EmergencyEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== AnchorHeartbeatEvent ====
        modelBuilder.Entity<AnchorHeartbeatEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.IpAddress).IsRequired().HasMaxLength(100);

            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.AnchorId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                  .WithMany(x => x.HeartbeatEvents)
                  .HasForeignKey(x => x.AnchorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== AnchorStatusEvent ====
        modelBuilder.Entity<AnchorStatusEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.PreviousStatus).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Reason).HasMaxLength(1000);

            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => x.Status);

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                  .WithMany(x => x.StatusEvents)
                  .HasForeignKey(x => x.AnchorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==== Alarm ====
        modelBuilder.Entity<Alarm>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AlarmType).HasConversion<string>().HasMaxLength(100);
            entity.Property(x => x.Severity).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(300);
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.DataJson).HasColumnType("jsonb");

            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.AlarmType);
            entity.HasIndex(x => x.Severity);
            entity.HasIndex(x => x.StartedAt);
            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.PeerTagId);
            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => new { x.Status, x.StartedAt });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.PrimaryAlarms)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.PeerTag)
                  .WithMany(x => x.PeerAlarms)
                  .HasForeignKey(x => x.PeerTagId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Anchor)
                  .WithMany(x => x.Alarms)
                  .HasForeignKey(x => x.AnchorId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AnchorHealthEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Temperature).HasPrecision(10, 2);
            entity.Property(x => x.CpuUsage).HasPrecision(5, 2);
            entity.Property(x => x.MemoryUsage).HasPrecision(5, 2);
            entity.Property(x => x.PacketLossRate).HasPrecision(5, 2);

            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.AnchorId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                  .WithMany(x => x.HealthEvents)
                  .HasForeignKey(x => x.AnchorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TagDataEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ReportedTagType).HasMaxLength(200);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                  .WithMany(x => x.TagDataEvents)
                  .HasForeignKey(x => x.AnchorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.TagDataEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UwbRangingEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Distance).HasPrecision(18, 6);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                  .WithMany(x => x.UwbRangingEvents)
                  .HasForeignKey(x => x.AnchorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.UwbRangingEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UwbTagToTagRangingEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Distance).HasPrecision(18, 6);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.PeerTagId);
            entity.HasIndex(x => x.EventTimestamp);

            entity.HasOne(x => x.RawEvent)
                  .WithMany()
                  .HasForeignKey(x => x.RawEventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                  .WithMany(x => x.PrimaryUwbTagToTagRangingEvents)
                  .HasForeignKey(x => x.TagId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.PeerTag)
                  .WithMany(x => x.PeerUwbTagToTagRangingEvents)
                  .HasForeignKey(x => x.PeerTagId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AnchorConfigEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirmwareVersion).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PositionJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.NetworkJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.UwbJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.BleJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.AnchorId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                .WithMany(x => x.ConfigEvents)
                .HasForeignKey(x => x.AnchorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AnchorConfigSnapshot>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirmwareVersion).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PositionJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.NetworkJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.UwbJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.BleJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => x.AnchorId).IsUnique();
            entity.HasIndex(x => x.LastReportedAt);

            entity.HasOne(x => x.Anchor)
                .WithMany(x => x.ConfigSnapshots)
                .HasForeignKey(x => x.AnchorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BleConfigEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TxPower).HasPrecision(10, 2);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.BleConfigEvents)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TagBleConfigSnapshot>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TxPower).HasPrecision(10, 2);

            entity.HasIndex(x => x.TagId).IsUnique();
            entity.HasIndex(x => x.LastReportedAt);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.BleConfigSnapshots)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UwbConfigEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TxPower).HasPrecision(10, 2);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.UwbConfigEvents)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TagUwbConfigSnapshot>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TxPower).HasPrecision(10, 2);

            entity.HasIndex(x => x.TagId).IsUnique();
            entity.HasIndex(x => x.LastReportedAt);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.UwbConfigSnapshots)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DioConfigEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Direction).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.LowPassFilterJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.Pin);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.Pin, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.DioConfigEvents)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TagDioConfigSnapshot>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Direction).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.LowPassFilterJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => new { x.TagId, x.Pin }).IsUnique();
            entity.HasIndex(x => x.LastReportedAt);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.DioConfigSnapshots)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<I2cConfigEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.DevicesJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.I2cConfigEvents)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TagI2cConfigSnapshot>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.DevicesJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => x.TagId).IsUnique();
            entity.HasIndex(x => x.LastReportedAt);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.I2cConfigSnapshots)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BleAdvertisementEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Anchor)
                .WithMany(x => x.BleAdvertisementEvents)
                .HasForeignKey(x => x.AnchorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.BleAdvertisementEvents)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DioValueEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.Pin);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.Pin, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.DioValueEvents)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TagDioValueSnapshot>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.TagId, x.Pin }).IsUnique();
            entity.HasIndex(x => x.LastReportedAt);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.DioValueSnapshots)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<I2cDataEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Direction).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.DataJson).IsRequired().HasColumnType("jsonb");

            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.EventTimestamp);
            entity.HasIndex(x => new { x.TagId, x.EventTimestamp });

            entity.HasOne(x => x.RawEvent)
                .WithMany()
                .HasForeignKey(x => x.RawEventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.I2cDataEvents)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CommandRequest>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CommandType).HasConversion<string>().HasMaxLength(100);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.TargetType).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.PayloadJson).IsRequired().HasColumnType("jsonb");

            entity.Property(x => x.ExternalCorrelationId).HasMaxLength(200);
            entity.Property(x => x.CancelReason).HasMaxLength(1000);
            entity.Property(x => x.FailureReason).HasMaxLength(2000);
            entity.Property(x => x.ResponseJson).HasColumnType("jsonb");

            entity.HasIndex(x => x.CommandType);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.TargetType);
            entity.HasIndex(x => x.TagId);
            entity.HasIndex(x => x.AnchorId);
            entity.HasIndex(x => x.RequestedByUserId);
            entity.HasIndex(x => x.RequestedAt);
            entity.HasIndex(x => new { x.Status, x.RequestedAt });

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.CommandRequests)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Anchor)
                .WithMany(x => x.CommandRequests)
                .HasForeignKey(x => x.AnchorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.RequestedByUser)
                .WithMany(x => x.RequestedCommands)
                .HasForeignKey(x => x.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CommandStatusHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OldStatus).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.NewStatus).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Note).HasMaxLength(1000);
            entity.Property(x => x.DataJson).HasColumnType("jsonb");

            entity.HasIndex(x => x.CommandRequestId);
            entity.HasIndex(x => x.ChangedAt);
            entity.HasIndex(x => new { x.CommandRequestId, x.ChangedAt });

            entity.HasOne(x => x.CommandRequest)
                .WithMany(x => x.StatusHistory)
                .HasForeignKey(x => x.CommandRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ChangedByUser)
                .WithMany(x => x.CommandStatusChanges)
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AggregateType).IsRequired().HasMaxLength(200);
            entity.Property(x => x.MessageType).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PayloadJson).IsRequired().HasColumnType("jsonb");
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.FailureReason).HasMaxLength(2000);

            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.AggregateType);
            entity.HasIndex(x => x.MessageType);
            entity.HasIndex(x => x.AggregateId);
            entity.HasIndex(x => x.OccurredAt);
            entity.HasIndex(x => new { x.Status, x.OccurredAt });
        });

        // ==== Global Soft-Delete Filter ====
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc
                            ? v
                            : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    ));
                }

                if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                        v => v.HasValue
                            ? (v.Value.Kind == DateTimeKind.Utc
                                ? v
                                : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc))
                            : v,
                        v => v.HasValue
                            ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                            : v
                    ));
                }
            }


            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propMethod = typeof(EF).GetMethod("Property", new[] { typeof(object), typeof(string) })!
                                            .MakeGenericMethod(typeof(DateTime?));
                var deletedAt = Expression.Call(propMethod, parameter, Expression.Constant(nameof(BaseEntity.DeletedAt)));
                var body = Expression.Equal(deletedAt, Expression.Constant(null, typeof(DateTime?)));
                var lambda = Expression.Lambda(body, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }


        }
    }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<UserType>())
        {
            if (entry.State == EntityState.Deleted && entry.Entity.IsSystem)
                throw new InvalidOperationException($"UserType '{entry.Entity.Code}' is system-defined and cannot be deleted.");
        }

        SetAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        var entries = ChangeTracker.Entries<BaseEntity>().ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = currentUserId;

                AddAuditLog("Insert", entry.Entity.GetType().Name, entry.Entity.Id,
                    new { Changes = GetChanges(entry) });
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = currentUserId;

                AddAuditLog("Update", entry.Entity.GetType().Name, entry.Entity.Id,
                    new { Changes = GetChanges(entry) });
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Soft delete
                entry.State = EntityState.Modified;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = currentUserId;

                AddAuditLog("Delete", entry.Entity.GetType().Name, entry.Entity.Id,
                    new { Changes = GetChanges(entry) });
            }
        }
    }

    private string? GetChanges(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var changes = new List<object>();

        foreach (var prop in entry.OriginalValues.Properties)
        {
            object? originalObj = null;
            object? currentObj = null;

            try
            {
                originalObj = entry.OriginalValues[prop];
                currentObj = entry.CurrentValues[prop];
            }
            catch
            {
                continue; // bazý navigation’larda eriþilemez olabilir
            }

            string? original = originalObj switch
            {
                byte[] b => Convert.ToBase64String(b),
                uint u => u.ToString(),
                _ => originalObj?.ToString()
            };

            string? current = currentObj switch
            {
                byte[] b => Convert.ToBase64String(b),
                uint u => u.ToString(),
                _ => currentObj?.ToString()
            };

            if (original != current)
            {
                changes.Add(new
                {
                    Property = prop.Name,
                    OldValue = original,
                    NewValue = current
                });
            }
        }

        return changes.Any() ? JsonSerializer.Serialize(changes) : null;
    }

    public void AddAuditLog(string action, string resourceType, Guid? resourceId = null, object? detail = null)
    {
        string? detailJson = null;

        if (detail != null)
        {
            detailJson = JsonSerializer.Serialize(detail, new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }

        var auditLog = AuditLog.Create(
            _currentUserService.GetCurrentUserId(),
            action,
            resourceType,
            resourceId,
            _currentUserService.GetIpAddress(),
            detailJson
        );

        AuditLogs.Add(auditLog);
    }
}