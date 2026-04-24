using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        db.Database.Migrate();

        var hasher = new BcryptPasswordHasher();

        // =========================================================
        // USER TYPES
        // =========================================================
        if (!db.UserTypes.Any())
        {
            db.UserTypes.AddRange(
                UserType.Create("SYSTEM", "Sistem", "Sistem kullanıcıları", true),
                UserType.Create("EMPLOYEE", "Çalışan", "Şirket çalışanı", true),
                UserType.Create("CONTRACTOR", "Yüklenici", "Taşeron / yüklenici personel", true),
                UserType.Create("VISITOR", "Ziyaretçi", "Geçici saha ziyaretçisi", true)
            );
            db.SaveChanges();
            Console.WriteLine("✓ UserTypes oluşturuldu.");
        }

        var userTypes = db.UserTypes.ToDictionary(x => x.Code, x => x);

        // =========================================================
        // SYSTEM USER
        // =========================================================
        var systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        if (!db.Users.Any(u => u.Id == systemUserId))
        {
            var systemUser = User.Create(
                "system@local",
                "!!SYSTEM!!",
                "Sistem",
                "Kullanıcısı",
                userTypes["SYSTEM"].Id
            );

            typeof(User).GetProperty(nameof(User.Id))!.SetValue(systemUser, systemUserId);

            db.Users.Add(systemUser);
            db.SaveChanges();
            Console.WriteLine("✓ Sistem kullanıcısı oluşturuldu.");
        }

        // =========================================================
        // ROLES
        // =========================================================
        var roleNames = new[]
        {
            "super_admin",
            "company_admin",
            "branch_admin",
            "dispatch_operator",
            "safety_operator",
            "security_operator",
            "maintenance_operator",
            "field_supervisor",
            "viewer"
        };

        foreach (var roleName in roleNames)
        {
            if (!db.Roles.Any(r => r.Name == roleName))
            {
                db.Roles.Add(Role.Create(roleName));
            }
        }

        db.SaveChanges();
        Console.WriteLine("✓ Roller oluşturuldu.");

        // =========================================================
        // PERMISSIONS
        // =========================================================
        var permissions = new[]
        {
            // auth / yönetim
            "manage_users",
            "manage_roles",
            "manage_permissions",
            "manage_settings",

            // organization
            "manage_companies",
            "manage_branches",

            // devices
            "view_devices",
            "manage_tags",
            "manage_anchors",
            "assign_tags",
            "view_config_snapshots",
            "manage_device_configs",

            // tracking
            "view_dashboard",
            "view_tracking",
            "view_tracking_history",

            // events
            "view_events",
            "view_raw_events",

            // alarms
            "view_alarms",
            "acknowledge_alarms",
            "manage_alarms",

            // commands
            "view_commands",
            "create_commands",
            "queue_commands",
            "cancel_commands",
            "retry_commands",

            // reporting
            "view_reports",

            // pii
            "view_pii",
            "edit_pii",
            "export_pii"
        };

        foreach (var permissionName in permissions)
        {
            if (!db.Permissions.Any(p => p.Name == permissionName))
            {
                db.Permissions.Add(Permission.Create(permissionName, $"{permissionName} yetkisi"));
            }
        }

        db.SaveChanges();
        Console.WriteLine("✓ Permission'lar oluşturuldu.");

        // =========================================================
        // ROLE - PERMISSION MAPPING
        // =========================================================
        var roles = db.Roles.ToDictionary(r => r.Name, r => r.Id);
        var perms = db.Permissions.ToDictionary(p => p.Name, p => p.Id);

        var mappings = new List<(string Role, string Permission)>
        {
            // super admin -> her şey
            ("super_admin","manage_users"),
            ("super_admin","manage_roles"),
            ("super_admin","manage_permissions"),
            ("super_admin","manage_settings"),
            ("super_admin","manage_companies"),
            ("super_admin","manage_branches"),
            ("super_admin","view_devices"),
            ("super_admin","manage_tags"),
            ("super_admin","manage_anchors"),
            ("super_admin","assign_tags"),
            ("super_admin","view_config_snapshots"),
            ("super_admin","manage_device_configs"),
            ("super_admin","view_dashboard"),
            ("super_admin","view_tracking"),
            ("super_admin","view_tracking_history"),
            ("super_admin","view_events"),
            ("super_admin","view_raw_events"),
            ("super_admin","view_alarms"),
            ("super_admin","acknowledge_alarms"),
            ("super_admin","manage_alarms"),
            ("super_admin","view_commands"),
            ("super_admin","create_commands"),
            ("super_admin","queue_commands"),
            ("super_admin","cancel_commands"),
            ("super_admin","retry_commands"),
            ("super_admin","view_reports"),
            ("super_admin","view_pii"),
            ("super_admin","edit_pii"),
            ("super_admin","export_pii"),

            // company admin
            ("company_admin","manage_users"),
            ("company_admin","manage_companies"),
            ("company_admin","manage_branches"),
            ("company_admin","view_devices"),
            ("company_admin","manage_tags"),
            ("company_admin","manage_anchors"),
            ("company_admin","assign_tags"),
            ("company_admin","view_config_snapshots"),
            ("company_admin","manage_device_configs"),
            ("company_admin","view_dashboard"),
            ("company_admin","view_tracking"),
            ("company_admin","view_tracking_history"),
            ("company_admin","view_events"),
            ("company_admin","view_raw_events"),
            ("company_admin","view_alarms"),
            ("company_admin","acknowledge_alarms"),
            ("company_admin","manage_alarms"),
            ("company_admin","view_commands"),
            ("company_admin","create_commands"),
            ("company_admin","queue_commands"),
            ("company_admin","cancel_commands"),
            ("company_admin","retry_commands"),
            ("company_admin","view_reports"),
            ("company_admin","view_pii"),
            ("company_admin","edit_pii"),
            ("company_admin","export_pii"),

            // branch admin
            ("branch_admin","manage_users"),
            ("branch_admin","view_devices"),
            ("branch_admin","manage_tags"),
            ("branch_admin","manage_anchors"),
            ("branch_admin","assign_tags"),
            ("branch_admin","view_config_snapshots"),
            ("branch_admin","manage_device_configs"),
            ("branch_admin","view_dashboard"),
            ("branch_admin","view_tracking"),
            ("branch_admin","view_tracking_history"),
            ("branch_admin","view_events"),
            ("branch_admin","view_alarms"),
            ("branch_admin","acknowledge_alarms"),
            ("branch_admin","manage_alarms"),
            ("branch_admin","view_commands"),
            ("branch_admin","create_commands"),
            ("branch_admin","queue_commands"),
            ("branch_admin","cancel_commands"),
            ("branch_admin","retry_commands"),
            ("branch_admin","view_reports"),
            ("branch_admin","view_pii"),

            // dispatch
            ("dispatch_operator","view_devices"),
            ("dispatch_operator","view_dashboard"),
            ("dispatch_operator","view_tracking"),
            ("dispatch_operator","view_tracking_history"),
            ("dispatch_operator","view_events"),
            ("dispatch_operator","view_alarms"),
            ("dispatch_operator","acknowledge_alarms"),
            ("dispatch_operator","view_commands"),
            ("dispatch_operator","create_commands"),
            ("dispatch_operator","queue_commands"),
            ("dispatch_operator","cancel_commands"),

            // safety
            ("safety_operator","view_dashboard"),
            ("safety_operator","view_tracking"),
            ("safety_operator","view_tracking_history"),
            ("safety_operator","view_events"),
            ("safety_operator","view_alarms"),
            ("safety_operator","acknowledge_alarms"),
            ("safety_operator","manage_alarms"),
            ("safety_operator","view_reports"),
            ("safety_operator","view_pii"),

            // security
            ("security_operator","view_dashboard"),
            ("security_operator","view_tracking"),
            ("security_operator","view_events"),
            ("security_operator","view_alarms"),
            ("security_operator","acknowledge_alarms"),
            ("security_operator","view_pii"),

            // maintenance
            ("maintenance_operator","view_devices"),
            ("maintenance_operator","manage_tags"),
            ("maintenance_operator","manage_anchors"),
            ("maintenance_operator","view_config_snapshots"),
            ("maintenance_operator","manage_device_configs"),
            ("maintenance_operator","view_events"),
            ("maintenance_operator","view_commands"),
            ("maintenance_operator","create_commands"),
            ("maintenance_operator","queue_commands"),
            ("maintenance_operator","cancel_commands"),
            ("maintenance_operator","retry_commands"),

            // field supervisor
            ("field_supervisor","view_dashboard"),
            ("field_supervisor","view_tracking"),
            ("field_supervisor","view_tracking_history"),
            ("field_supervisor","view_events"),
            ("field_supervisor","view_alarms"),
            ("field_supervisor","acknowledge_alarms"),
            ("field_supervisor","view_reports"),
            ("field_supervisor","view_pii"),

            // viewer
            ("viewer","view_dashboard"),
            ("viewer","view_tracking"),
            ("viewer","view_tracking_history"),
            ("viewer","view_events"),
            ("viewer","view_alarms"),
            ("viewer","view_commands"),
            ("viewer","view_reports")
        };

        foreach (var (role, permission) in mappings)
        {
            if (roles.TryGetValue(role, out var roleId) && perms.TryGetValue(permission, out var permId))
            {
                if (!db.RolePermissions.Any(rp => rp.RoleId == roleId && rp.PermissionId == permId))
                {
                    db.RolePermissions.Add(RolePermission.Create(roleId, permId));
                }
            }
        }

        db.SaveChanges();
        Console.WriteLine("✓ Rol-permission eşleşmeleri oluşturuldu.");

        // =========================================================
        // USER SPECIALIZATIONS
        // =========================================================
        if (!db.UserSpecializations.Any())
        {
            db.UserSpecializations.AddRange(
                // EMPLOYEE
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "UNDERGROUND_WORKER", "Yeraltı Çalışanı", "Yeraltı saha personeli", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "SURFACE_WORKER", "Yerüstü Çalışanı", "Yerüstü saha personeli", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "SUPERVISOR", "Süpervizör", "Vardiya / saha süpervizörü", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "SAFETY_SPECIALIST", "İSG Uzmanı", "İş sağlığı ve güvenliği uzmanı", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "SECURITY_STAFF", "Güvenlik", "Güvenlik personeli", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "DISPATCHER", "Dispatch Operatörü", "Takip / sevk operatörü", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "MAINTENANCE_TECHNICIAN", "Bakım Teknisyeni", "Bakım ve teknik servis", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "ELECTRICIAN", "Elektrik Teknisyeni", "Elektrik bakım personeli", true),
                UserSpecialization.Create(userTypes["EMPLOYEE"].Id, "MECHANIC", "Mekanik Teknisyeni", "Mekanik bakım personeli", true),

                // CONTRACTOR
                UserSpecialization.Create(userTypes["CONTRACTOR"].Id, "CONTRACTOR_TECHNICIAN", "Yüklenici Teknisyen", "Taşeron teknik personel", true),
                UserSpecialization.Create(userTypes["CONTRACTOR"].Id, "CONTRACTOR_OPERATOR", "Yüklenici Operatör", "Taşeron saha operatörü", true),

                // VISITOR
                UserSpecialization.Create(userTypes["VISITOR"].Id, "VISITOR_STANDARD", "Ziyaretçi", "Standart ziyaretçi", true),
                UserSpecialization.Create(userTypes["VISITOR"].Id, "AUDITOR", "Denetçi", "Denetim / inceleme amaçlı ziyaretçi", true)
            );

            db.SaveChanges();
            Console.WriteLine("✓ UserSpecializations oluşturuldu.");
        }

        var specializations = db.UserSpecializations.ToDictionary(x => x.Code, x => x);

        // =========================================================
        // USERS
        // =========================================================
        var userSeeds = new List<UserSeedItem>
        {
            new("admin@rtls.local", "Admin123!", "Sistem", "Yöneticisi", "super_admin", "EMPLOYEE", "SUPERVISOR"),
            new("serkan.demiray@live.com", "634496", "Sistem", "Yöneticisi", "super_admin", "EMPLOYEE", "SUPERVISOR"),
            new("companyadmin@maden.local", "Admin123!", "Şirket", "Yöneticisi", "company_admin", "EMPLOYEE", "SUPERVISOR"),
            new("branchadmin@maden.local", "Admin123!", "Şube", "Yöneticisi", "branch_admin", "EMPLOYEE", "SUPERVISOR"),

            new("dispatch1@maden.local", "123456", "Ahmet", "Koç", "dispatch_operator", "EMPLOYEE", "DISPATCHER"),
            new("dispatch2@maden.local", "123456", "Merve", "Aydın", "dispatch_operator", "EMPLOYEE", "DISPATCHER"),

            new("safety1@maden.local", "123456", "Selin", "Yılmaz", "safety_operator", "EMPLOYEE", "SAFETY_SPECIALIST"),
            new("security1@maden.local", "123456", "Burak", "Demir", "security_operator", "EMPLOYEE", "SECURITY_STAFF"),

            new("maintenance1@maden.local", "123456", "Emre", "Kaya", "maintenance_operator", "EMPLOYEE", "MAINTENANCE_TECHNICIAN"),
            new("mechanic1@maden.local", "123456", "Onur", "Çelik", "maintenance_operator", "EMPLOYEE", "MECHANIC"),
            new("electric1@maden.local", "123456", "Deniz", "Arslan", "maintenance_operator", "EMPLOYEE", "ELECTRICIAN"),

            new("supervisor1@maden.local", "123456", "Hasan", "Şahin", "field_supervisor", "EMPLOYEE", "SUPERVISOR"),

            new("worker1@maden.local", "123456", "İsmail", "Öztürk", "viewer", "EMPLOYEE", "UNDERGROUND_WORKER"),
            new("worker2@maden.local", "123456", "Kemal", "Erdoğan", "viewer", "EMPLOYEE", "UNDERGROUND_WORKER"),
            new("worker3@maden.local", "123456", "Yusuf", "Kurt", "viewer", "EMPLOYEE", "SURFACE_WORKER"),

            new("contractor1@maden.local", "123456", "Murat", "Taş", "viewer", "CONTRACTOR", "CONTRACTOR_TECHNICIAN"),
            new("contractor2@maden.local", "123456", "Cem", "Can", "viewer", "CONTRACTOR", "CONTRACTOR_OPERATOR"),

            new("visitor1@maden.local", "123456", "Ece", "Aksoy", "viewer", "VISITOR", "VISITOR_STANDARD"),
            new("auditor1@maden.local", "123456", "Levent", "Ünal", "viewer", "VISITOR", "AUDITOR")
        };

        foreach (var item in userSeeds)
        {
            if (db.Users.Any(u => u.Email == item.Email))
                continue;

            var user = User.Create(
                item.Email,
                hasher.Hash(item.Password),
                item.FirstName,
                item.LastName,
                userTypes[item.UserTypeCode].Id
            );

            user.SetSpecialization(specializations[item.SpecializationCode].Id);

            db.Users.Add(user);
            db.SaveChanges();

            var role = db.Roles.First(x => x.Name == item.RoleName);
            if (!db.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == role.Id))
            {
                db.UserRoles.Add(UserRole.Create(user.Id, role.Id));
                db.SaveChanges();
            }

            Console.WriteLine($"✓ Kullanıcı oluşturuldu: {item.Email}");
        }

        // =========================================================
        // SYSTEM USER ROLE
        // =========================================================
        var superAdminRoleId = db.Roles.First(r => r.Name == "super_admin").Id;
        if (!db.UserRoles.Any(ur => ur.UserId == systemUserId && ur.RoleId == superAdminRoleId))
        {
            db.UserRoles.Add(UserRole.Create(systemUserId, superAdminRoleId));
            db.SaveChanges();
            Console.WriteLine("✓ Sistem kullanıcısına super_admin rolü atandı.");
        }

        // =========================================================
        // COMPANY & BRANCHES
        // =========================================================
        if (!db.Companies.Any())
        {
            var companyM = Company.Create(
                "Demo Maden İşletmeleri A.Ş.",
                "1234567890",
                "Organize Sanayi Bölgesi 1. Cadde No:10",
                "+90 312 555 10 10",
                "info@demomaden.com"
            );

            db.Companies.Add(companyM);
            db.SaveChanges();

            db.Branches.AddRange(
                Branch.Create(companyM.Id, "Merkez Saha", "Merkez Saha Kampüsü", "+90 312 555 10 11", "merkez@demomaden.com"),
                Branch.Create(companyM.Id, "Yeraltı Ocak 1", "Yeraltı Ocak 1 Bölgesi", "+90 312 555 10 12", "ocak1@demomaden.com"),
                Branch.Create(companyM.Id, "Zenginleştirme Tesisi", "Tesis Bölgesi", "+90 312 555 10 13", "tesis@demomaden.com")
            );

            db.SaveChanges();
            Console.WriteLine("✓ Company ve Branch kayıtları oluşturuldu.");
        }

        // =========================================================
        // USER-COMPANY / USER-BRANCH ASSIGNMENTS
        // =========================================================
        var companyEntity = db.Companies.First();
        var firstBranch = db.Branches.OrderBy(x => x.Name).First();

        var assignableUsers = db.Users
            .Where(u => u.Id != systemUserId && u.UserTypeId != userTypes["VISITOR"].Id)
            .ToList();

        foreach (var user in assignableUsers)
        {
            if (!db.UserCompanies.Any(uc => uc.UserId == user.Id && uc.CompanyId == companyEntity.Id))
            {
                db.UserCompanies.Add(UserCompany.Create(user.Id, companyEntity.Id));
            }

            if (!db.UserBranches.Any(ub => ub.UserId == user.Id && ub.BranchId == firstBranch.Id))
            {
                db.UserBranches.Add(UserBranch.Create(user.Id, firstBranch.Id));
            }
        }

        db.SaveChanges();
        Console.WriteLine("✓ UserCompany / UserBranch atamaları yapıldı.");

        // =========================================================
        // SETTINGS
        // =========================================================
        if (!db.Settings.Any())
        {
            db.Settings.AddRange(
                Setting.Create("DefaultLanguage", "tr", SettingScope.System),
                Setting.Create("Theme", "light", SettingScope.User),
                Setting.Create("TrackingRefreshSeconds", "5", SettingScope.System),
                Setting.Create("AlarmAutoRefreshSeconds", "5", SettingScope.System),
                Setting.Create("CommandDefaultQueueMode", "manual", SettingScope.System),
                Setting.Create("DefaultMapLayer", "mine", SettingScope.System)
            );

            db.SaveChanges();
            Console.WriteLine("✓ Settings oluşturuldu.");
        }

        // =========================================================
        // REFRESH TOKENS
        // =========================================================
        if (!db.RefreshTokens.Any())
        {
            var seedUsers = db.Users
                .Where(u => u.Email == "admin@rtls.local" || u.Email == "companyadmin@maden.local")
                .ToList();

            foreach (var user in seedUsers)
            {
                db.RefreshTokens.Add(
                    RefreshToken.Create(
                        user.Id,
                        "refresh_token_" + Guid.NewGuid().ToString("N")[..24],
                        DateTime.UtcNow.AddDays(30),
                        null
                    )
                );
            }

            db.SaveChanges();
            Console.WriteLine("✓ RefreshToken kayıtları oluşturuldu.");
        }

        // =========================================================
        // HARDWARE / RTLS DEMO DATA
        // =========================================================

        Console.WriteLine("✓ RTLS demo verileri hazırlanıyor...");

        var company = db.Companies.First();
        var branches = db.Branches.OrderBy(x => x.Name).ToList();
        var branchMain = branches[0];
        var branchUnderground = branches.Count > 1 ? branches[1] : branches[0];
        var branchPlant = branches.Count > 2 ? branches[2] : branches[0];

        var usersByEmail = db.Users.ToDictionary(x => x.Email, x => x);

        var worker1 = usersByEmail["worker1@maden.local"];
        var worker2 = usersByEmail["worker2@maden.local"];
        var worker3 = usersByEmail["worker3@maden.local"];
        var contractor1 = usersByEmail["contractor1@maden.local"];
        var contractor2 = usersByEmail["contractor2@maden.local"];
        var supervisor1 = usersByEmail["supervisor1@maden.local"];
        var dispatch1 = usersByEmail["dispatch1@maden.local"];
        var safety1 = usersByEmail["safety1@maden.local"];
        var maintenance1 = usersByEmail["maintenance1@maden.local"];
        var security1 = usersByEmail["security1@maden.local"];
        var adminUser = usersByEmail["admin@rtls.local"];

        // =========================================================
        // TAGS
        // =========================================================
        var tagSeeds = new[]
        {
            new
            {
                ExternalId = "tag-person-001",
                Code = "TAG-P-001",
                Name = "Yeraltı Çalışanı - İsmail",
                Serial = "SN-TAG-P-001",
                TagType = TagType.Personnel,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Personnel",
                    HelmetNo = "B-101",
                    Zone = "Underground",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-person-002",
                Code = "TAG-P-002",
                Name = "Yeraltı Çalışanı - Kemal",
                Serial = "SN-TAG-P-002",
                TagType = TagType.Personnel,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Personnel",
                    HelmetNo = "B-102",
                    Zone = "Underground",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-person-003",
                Code = "TAG-P-003",
                Name = "Yerüstü Çalışanı - Yusuf",
                Serial = "SN-TAG-P-003",
                TagType = TagType.Personnel,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Personnel",
                    HelmetNo = "S-201",
                    Zone = "Surface",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-person-004",
                Code = "TAG-P-004",
                Name = "Yüklenici Teknisyen - Murat",
                Serial = "SN-TAG-P-004",
                TagType = TagType.Personnel,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Contractor",
                    Team = "Electrical",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-person-005",
                Code = "TAG-P-005",
                Name = "Yüklenici Operatör - Cem",
                Serial = "SN-TAG-P-005",
                TagType = TagType.Personnel,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Contractor",
                    Team = "Operator",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-vehicle-001",
                Code = "TAG-V-001",
                Name = "Yeraltı Araç 1",
                Serial = "SN-TAG-V-001",
                TagType = TagType.Vehicle,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Vehicle",
                    Plate = "34 DEMO 01",
                    VehicleType = "Loader",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-vehicle-002",
                Code = "TAG-V-002",
                Name = "Servis Aracı 2",
                Serial = "SN-TAG-V-002",
                TagType = TagType.Vehicle,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Vehicle",
                    Plate = "34 DEMO 02",
                    VehicleType = "Pickup",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-asset-001",
                Code = "TAG-A-001",
                Name = "Jeneratör 1",
                Serial = "SN-TAG-A-001",
                TagType = TagType.Asset,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Asset",
                    AssetType = "Generator",
                    Demo = true
                })
            },
            new
            {
                ExternalId = "tag-asset-002",
                Code = "TAG-A-002",
                Name = "Pompa 2",
                Serial = "SN-TAG-A-002",
                TagType = TagType.Asset,
                Metadata = JsonSerializer.Serialize(new
                {
                    Category = "Asset",
                    AssetType = "Pump",
                    Demo = true
                })
            }
        };

        foreach (var seed in tagSeeds)
        {
            if (db.Tags.Any(x => x.ExternalId == seed.ExternalId))
                continue;

            var tag = Tag.Create(
                seed.ExternalId,
                seed.Code,
                seed.Name,
                seed.Serial,
                seed.TagType,
                seed.Metadata);

            db.Tags.Add(tag);
        }

        db.SaveChanges();
        Console.WriteLine("✓ RTLS tag verileri oluşturuldu.");

        var tagsByExternalId = db.Tags.ToDictionary(x => x.ExternalId, x => x);

        // tag başlangıç durumları
        SetTagRuntime(tagsByExternalId["tag-person-001"], TagStatus.Online, 78, DateTime.UtcNow.AddMinutes(-2));
        SetTagRuntime(tagsByExternalId["tag-person-002"], TagStatus.Online, 64, DateTime.UtcNow.AddMinutes(-3));
        SetTagRuntime(tagsByExternalId["tag-person-003"], TagStatus.Online, 92, DateTime.UtcNow.AddMinutes(-5));
        SetTagRuntime(tagsByExternalId["tag-person-004"], TagStatus.Online, 55, DateTime.UtcNow.AddMinutes(-4));
        SetTagRuntime(tagsByExternalId["tag-person-005"], TagStatus.Online, 48, DateTime.UtcNow.AddMinutes(-6));
        SetTagRuntime(tagsByExternalId["tag-vehicle-001"], TagStatus.Online, 81, DateTime.UtcNow.AddMinutes(-1));
        SetTagRuntime(tagsByExternalId["tag-vehicle-002"], TagStatus.Offline, 73, DateTime.UtcNow.AddHours(-2));
        SetTagRuntime(tagsByExternalId["tag-asset-001"], TagStatus.Online, 15, DateTime.UtcNow.AddMinutes(-10));
        SetTagRuntime(tagsByExternalId["tag-asset-002"], TagStatus.Inactive, null, null);

        db.SaveChanges();

        // =========================================================
        // ANCHORS
        // =========================================================
        var anchorSeeds = new[]
        {
            new
            {
                ExternalId = "anchor-001",
                Code = "ANCH-001",
                Name = "Yeraltı Anchor 1",
                Ip = "10.10.1.11",
                BranchId = branchUnderground.Id,
                CompanyId = company.Id,
                Status = AnchorStatus.Online,
                Metadata = JsonSerializer.Serialize(new { Zone = "Underground-A", X = 0, Y = 0, Z = 0, Demo = true })
            },
            new
            {
                ExternalId = "anchor-002",
                Code = "ANCH-002",
                Name = "Yeraltı Anchor 2",
                Ip = "10.10.1.12",
                BranchId = branchUnderground.Id,
                CompanyId = company.Id,
                Status = AnchorStatus.Online,
                Metadata = JsonSerializer.Serialize(new { Zone = "Underground-B", X = 20, Y = 0, Z = 0, Demo = true })
            },
            new
            {
                ExternalId = "anchor-003",
                Code = "ANCH-003",
                Name = "Yeraltı Anchor 3",
                Ip = "10.10.1.13",
                BranchId = branchUnderground.Id,
                CompanyId = company.Id,
                Status = AnchorStatus.Online,
                Metadata = JsonSerializer.Serialize(new { Zone = "Underground-C", X = 10, Y = 12, Z = 0, Demo = true })
            },
            new
            {
                ExternalId = "anchor-004",
                Code = "ANCH-004",
                Name = "Merkez Anchor 1",
                Ip = "10.10.2.21",
                BranchId = branchMain.Id,
                CompanyId = company.Id,
                Status = AnchorStatus.Online,
                Metadata = JsonSerializer.Serialize(new { Zone = "MainGate", X = 5, Y = 5, Z = 0, Demo = true })
            },
            new
            {
                ExternalId = "anchor-005",
                Code = "ANCH-005",
                Name = "Tesis Anchor 1",
                Ip = "10.10.3.31",
                BranchId = branchPlant.Id,
                CompanyId = company.Id,
                Status = AnchorStatus.Error,
                Metadata = JsonSerializer.Serialize(new { Zone = "Plant", X = 100, Y = 20, Z = 0, Demo = true })
            },
            new
            {
                ExternalId = "anchor-006",
                Code = "ANCH-006",
                Name = "Tesis Anchor 2",
                Ip = "10.10.3.32",
                BranchId = branchPlant.Id,
                CompanyId = company.Id,
                Status = AnchorStatus.Offline,
                Metadata = JsonSerializer.Serialize(new { Zone = "Plant-West", X = 120, Y = 30, Z = 0, Demo = true })
            }
        };

        foreach (var seed in anchorSeeds)
        {
            if (db.Anchors.Any(x => x.ExternalId == seed.ExternalId))
                continue;

            var anchor = Anchor.Create(
                seed.ExternalId,
                seed.Code,
                seed.Name,
                seed.Ip,
                seed.CompanyId,
                seed.BranchId,
                seed.Metadata);

            db.Anchors.Add(anchor);
        }

        db.SaveChanges();
        Console.WriteLine("✓ RTLS anchor verileri oluşturuldu.");

        var anchorsByExternalId = db.Anchors.ToDictionary(x => x.ExternalId, x => x);

        SetAnchorRuntime(anchorsByExternalId["anchor-001"], AnchorStatus.Online, "10.10.1.11", DateTime.UtcNow.AddMinutes(-1));
        SetAnchorRuntime(anchorsByExternalId["anchor-002"], AnchorStatus.Online, "10.10.1.12", DateTime.UtcNow.AddMinutes(-2));
        SetAnchorRuntime(anchorsByExternalId["anchor-003"], AnchorStatus.Online, "10.10.1.13", DateTime.UtcNow.AddMinutes(-3));
        SetAnchorRuntime(anchorsByExternalId["anchor-004"], AnchorStatus.Online, "10.10.2.21", DateTime.UtcNow.AddMinutes(-2));
        SetAnchorRuntime(anchorsByExternalId["anchor-005"], AnchorStatus.Error, "10.10.3.31", DateTime.UtcNow.AddMinutes(-7));
        SetAnchorRuntime(anchorsByExternalId["anchor-006"], AnchorStatus.Offline, "10.10.3.32", DateTime.UtcNow.AddHours(-4));

        db.SaveChanges();

        // =========================================================
        // TAG ASSIGNMENTS
        // =========================================================
        EnsureTagAssignment(db, tagsByExternalId["tag-person-001"].Id, worker1.Id, true);
        EnsureTagAssignment(db, tagsByExternalId["tag-person-002"].Id, worker2.Id, true);
        EnsureTagAssignment(db, tagsByExternalId["tag-person-003"].Id, worker3.Id, true);
        EnsureTagAssignment(db, tagsByExternalId["tag-person-004"].Id, contractor1.Id, true);
        EnsureTagAssignment(db, tagsByExternalId["tag-person-005"].Id, contractor2.Id, true);

        db.SaveChanges();
        Console.WriteLine("✓ Tag assignment verileri oluşturuldu.");

        // =========================================================
        // RAW EVENTS + EVENTS + CURRENT LOCATIONS
        // =========================================================
        var rawLocation1 = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            "LocationCalculated",
            DateTime.UtcNow.AddMinutes(-2),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000001",
                type = "LocationCalculated",
                tagId = "tag-person-001",
                x = 11.5m,
                y = 6.3m,
                z = 0.0m,
                accuracy = 0.75m,
                confidence = 92,
                usedAnchors = new[] { "anchor-001", "anchor-002", "anchor-003" }
            }),
            RawEventProcessingStatus.Processed);

        var rawLocation2 = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000002"),
            "LocationCalculated",
            DateTime.UtcNow.AddMinutes(-3),
            "tag-person-002",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000002",
                type = "LocationCalculated",
                tagId = "tag-person-002",
                x = 14.2m,
                y = 8.9m,
                z = 0.0m,
                accuracy = 0.90m,
                confidence = 88,
                usedAnchors = new[] { "anchor-001", "anchor-002", "anchor-003" }
            }),
            RawEventProcessingStatus.Processed);

        var rawLocation3 = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000003"),
            "LocationCalculated",
            DateTime.UtcNow.AddMinutes(-1),
            "tag-vehicle-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000003",
                type = "LocationCalculated",
                tagId = "tag-vehicle-001",
                x = 15.1m,
                y = 9.4m,
                z = 0.0m,
                accuracy = 0.60m,
                confidence = 95,
                usedAnchors = new[] { "anchor-001", "anchor-002", "anchor-003" }
            }),
            RawEventProcessingStatus.Processed);

        db.SaveChanges();

        EnsureLocationEvent(
            db,
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            rawLocation1.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-2),
            11.5m, 6.3m, 0.0m, 0.75m, 92m,
            JsonSerializer.Serialize(new[] { new { anchorId = "anchor-001" }, new { anchorId = "anchor-002" }, new { anchorId = "anchor-003" } }));

        EnsureLocationEvent(
            db,
            Guid.Parse("20000000-0000-0000-0000-000000000002"),
            rawLocation2.Id,
            tagsByExternalId["tag-person-002"].Id,
            DateTime.UtcNow.AddMinutes(-3),
            14.2m, 8.9m, 0.0m, 0.90m, 88m,
            JsonSerializer.Serialize(new[] { new { anchorId = "anchor-001" }, new { anchorId = "anchor-002" }, new { anchorId = "anchor-003" } }));

        EnsureLocationEvent(
            db,
            Guid.Parse("20000000-0000-0000-0000-000000000003"),
            rawLocation3.Id,
            tagsByExternalId["tag-vehicle-001"].Id,
            DateTime.UtcNow.AddMinutes(-1),
            15.1m, 9.4m, 0.0m, 0.60m, 95m,
            JsonSerializer.Serialize(new[] { new { anchorId = "anchor-001" }, new { anchorId = "anchor-002" }, new { anchorId = "anchor-003" } }));

        db.SaveChanges();

        EnsureCurrentLocation(
            db,
            Guid.Parse("30000000-0000-0000-0000-000000000001"),
            tagsByExternalId["tag-person-001"].Id,
            worker1.Id,
            11.5m, 6.3m, 0.0m, 0.75m, 92m,
            DateTime.UtcNow.AddMinutes(-2),
            rawLocation1.Id,
            3);

        EnsureCurrentLocation(
            db,
            Guid.Parse("30000000-0000-0000-0000-000000000002"),
            tagsByExternalId["tag-person-002"].Id,
            worker2.Id,
            14.2m, 8.9m, 0.0m, 0.90m, 88m,
            DateTime.UtcNow.AddMinutes(-3),
            rawLocation2.Id,
            3);

        EnsureCurrentLocation(
            db,
            Guid.Parse("30000000-0000-0000-0000-000000000003"),
            tagsByExternalId["tag-vehicle-001"].Id,
            null,
            15.1m, 9.4m, 0.0m, 0.60m, 95m,
            DateTime.UtcNow.AddMinutes(-1),
            rawLocation3.Id,
            3);

        db.SaveChanges();

        // =========================================================
        // BATTERY / EMERGENCY / PROXIMITY / UWB / BLE / TAGDATA
        // =========================================================
        var rawBattery = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000004"),
            "BatteryLevelReported",
            DateTime.UtcNow.AddMinutes(-10),
            "tag-asset-001",
            "anchor-004",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000004",
                type = "BatteryLevelReported",
                tagId = "tag-asset-001",
                anchorId = "anchor-004",
                batteryLevel = 15
            }),
            RawEventProcessingStatus.Processed);

        var rawEmergency = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000005"),
            "EmergencyButtonPressed",
            DateTime.UtcNow.AddMinutes(-8),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000005",
                type = "EmergencyButtonPressed",
                tagId = "tag-person-001"
            }),
            RawEventProcessingStatus.Processed);

        var rawProximity = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000006"),
            "ProximityAlertRaised",
            DateTime.UtcNow.AddMinutes(-4),
            "tag-person-002",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000006",
                type = "ProximityAlertRaised",
                tagId = "tag-person-002",
                peerTagId = "tag-vehicle-001",
                distance = 1.8m,
                threshold = 2.5m,
                severity = "Critical"
            }),
            RawEventProcessingStatus.Processed);

        var rawUwb = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000007"),
            "UWBRangingCompleted",
            DateTime.UtcNow.AddMinutes(-2),
            "tag-person-001",
            "anchor-001",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000007",
                type = "UWBRangingCompleted",
                tagId = "tag-person-001",
                anchorId = "anchor-001",
                distance = 6.45m
            }),
            RawEventProcessingStatus.Processed);

        var rawTagToTag = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000008"),
            "UWBTagToTagRangingCompleted",
            DateTime.UtcNow.AddMinutes(-4),
            "tag-person-002",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000008",
                type = "UWBTagToTagRangingCompleted",
                tagId = "tag-person-002",
                peerTagId = "tag-vehicle-001",
                distance = 1.8m
            }),
            RawEventProcessingStatus.Processed);

        var rawBle = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000009"),
            "BLEAdvertisementReceived",
            DateTime.UtcNow.AddMinutes(-2),
            "tag-person-003",
            "anchor-004",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000009",
                type = "BLEAdvertisementReceived",
                tagId = "tag-person-003",
                anchorId = "anchor-004",
                rssi = -67
            }),
            RawEventProcessingStatus.Processed);

        var rawTagData = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000010"),
            "TagDataReceived",
            DateTime.UtcNow.AddMinutes(-2),
            "tag-person-003",
            "anchor-004",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000010",
                type = "TagDataReceived",
                tagId = "tag-person-003",
                anchorId = "anchor-004",
                tagType = "CodignoTagPro"
            }),
            RawEventProcessingStatus.Processed);

        db.SaveChanges();

        EnsureBatteryEvent(
            db,
            Guid.Parse("40000000-0000-0000-0000-000000000001"),
            rawBattery.Id,
            tagsByExternalId["tag-asset-001"].Id,
            anchorsByExternalId["anchor-004"].Id,
            DateTime.UtcNow.AddMinutes(-10),
            15);

        EnsureEmergencyEvent(
            db,
            Guid.Parse("40000000-0000-0000-0000-000000000002"),
            rawEmergency.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-8));

        EnsureProximityEvent(
            db,
            Guid.Parse("40000000-0000-0000-0000-000000000003"),
            rawProximity.Id,
            tagsByExternalId["tag-person-002"].Id,
            tagsByExternalId["tag-vehicle-001"].Id,
            DateTime.UtcNow.AddMinutes(-4),
            1.8m,
            2.5m,
            ProximitySeverity.Critical);

        EnsureUwbRangingEvent(
            db,
            Guid.Parse("40000000-0000-0000-0000-000000000004"),
            rawUwb.Id,
            anchorsByExternalId["anchor-001"].Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-2),
            6.45m);

        EnsureUwbTagToTagRangingEvent(
            db,
            Guid.Parse("40000000-0000-0000-0000-000000000005"),
            rawTagToTag.Id,
            tagsByExternalId["tag-person-002"].Id,
            tagsByExternalId["tag-vehicle-001"].Id,
            DateTime.UtcNow.AddMinutes(-4),
            1.8m);

        EnsureBleAdvertisementEvent(
            db,
            Guid.Parse("40000000-0000-0000-0000-000000000006"),
            rawBle.Id,
            anchorsByExternalId["anchor-004"].Id,
            tagsByExternalId["tag-person-003"].Id,
            DateTime.UtcNow.AddMinutes(-2),
            -67);

        EnsureTagDataEvent(
            db,
            Guid.Parse("40000000-0000-0000-0000-000000000007"),
            rawTagData.Id,
            anchorsByExternalId["anchor-004"].Id,
            tagsByExternalId["tag-person-003"].Id,
            DateTime.UtcNow.AddMinutes(-2),
            "CodignoTagPro");

        db.SaveChanges();

        // =========================================================
        // ANCHOR HEARTBEAT / STATUS / HEALTH
        // =========================================================
        var rawHeartbeat = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000011"),
            "AnchorHeartbeatReceived",
            DateTime.UtcNow.AddMinutes(-1),
            null,
            "anchor-001",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000011",
                type = "AnchorHeartbeatReceived",
                anchorId = "anchor-001",
                ipAddress = "10.10.1.11"
            }),
            RawEventProcessingStatus.Processed);

        var rawStatus = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000012"),
            "AnchorStatusChanged",
            DateTime.UtcNow.AddMinutes(-7),
            null,
            "anchor-005",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000012",
                type = "AnchorStatusChanged",
                anchorId = "anchor-005",
                status = "Error",
                previousStatus = "Online",
                reason = "Packet loss threshold exceeded"
            }),
            RawEventProcessingStatus.Processed);

        var rawHealth = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000013"),
            "AnchorHealthReported",
            DateTime.UtcNow.AddMinutes(-5),
            null,
            "anchor-001",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000013",
                type = "AnchorHealthReported",
                anchorId = "anchor-001",
                uptime = 86400000,
                temperature = 42.5,
                cpuUsage = 21.3,
                memoryUsage = 44.8,
                tagCount = 12,
                packetLossRate = 0.8
            }),
            RawEventProcessingStatus.Processed);

        db.SaveChanges();

        EnsureAnchorHeartbeatEvent(
            db,
            Guid.Parse("50000000-0000-0000-0000-000000000001"),
            rawHeartbeat.Id,
            anchorsByExternalId["anchor-001"].Id,
            DateTime.UtcNow.AddMinutes(-1),
            "10.10.1.11");

        EnsureAnchorStatusEvent(
            db,
            Guid.Parse("50000000-0000-0000-0000-000000000002"),
            rawStatus.Id,
            anchorsByExternalId["anchor-005"].Id,
            DateTime.UtcNow.AddMinutes(-7),
            AnchorStatus.Error,
            AnchorStatus.Online,
            "Packet loss threshold exceeded");

        EnsureAnchorHealthEvent(
            db,
            Guid.Parse("50000000-0000-0000-0000-000000000003"),
            rawHealth.Id,
            anchorsByExternalId["anchor-001"].Id,
            DateTime.UtcNow.AddMinutes(-5),
            86400000,
            42.5m,
            21.3m,
            44.8m,
            12,
            0.8m);

        db.SaveChanges();

        // =========================================================
        // CONFIG RAW EVENTS + EVENTS + SNAPSHOTS
        // =========================================================
        var rawAnchorCfg = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000014"),
            "AnchorConfigReported",
            DateTime.UtcNow.AddMinutes(-30),
            null,
            "anchor-001",
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000014",
                type = "AnchorConfigReported",
                anchorId = "anchor-001"
            }),
            RawEventProcessingStatus.Processed);

        var rawBleCfg = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000015"),
            "BLEConfigReported",
            DateTime.UtcNow.AddMinutes(-25),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000015",
                type = "BLEConfigReported",
                tagId = "tag-person-001"
            }),
            RawEventProcessingStatus.Processed);

        var rawUwbCfg = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000016"),
            "UWBConfigReported",
            DateTime.UtcNow.AddMinutes(-24),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000016",
                type = "UWBConfigReported",
                tagId = "tag-person-001"
            }),
            RawEventProcessingStatus.Processed);

        var rawDioCfg = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000017"),
            "DIOConfigReported",
            DateTime.UtcNow.AddMinutes(-23),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000017",
                type = "DIOConfigReported",
                tagId = "tag-person-001",
                pin = 1
            }),
            RawEventProcessingStatus.Processed);

        var rawDioVal = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000018"),
            "DIOValueReported",
            DateTime.UtcNow.AddMinutes(-5),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000018",
                type = "DIOValueReported",
                tagId = "tag-person-001",
                pin = 1,
                value = true
            }),
            RawEventProcessingStatus.Processed);

        var rawI2cCfg = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000019"),
            "I2CConfigReported",
            DateTime.UtcNow.AddMinutes(-22),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000019",
                type = "I2CConfigReported",
                tagId = "tag-person-001"
            }),
            RawEventProcessingStatus.Processed);

        var rawI2cData = EnsureRawEvent(
            db,
            Guid.Parse("10000000-0000-0000-0000-000000000020"),
            "I2CDataReceived",
            DateTime.UtcNow.AddMinutes(-4),
            "tag-person-001",
            null,
            JsonSerializer.Serialize(new
            {
                id = "10000000-0000-0000-0000-000000000020",
                type = "I2CDataReceived",
                tagId = "tag-person-001",
                address = 64,
                register = 1
            }),
            RawEventProcessingStatus.Processed);

        db.SaveChanges();

        EnsureAnchorConfigEventAndSnapshot(
            db,
            Guid.Parse("60000000-0000-0000-0000-000000000001"),
            Guid.Parse("70000000-0000-0000-0000-000000000001"),
            rawAnchorCfg.Id,
            anchorsByExternalId["anchor-001"].Id,
            DateTime.UtcNow.AddMinutes(-30));

        EnsureBleConfigEventAndSnapshot(
            db,
            Guid.Parse("60000000-0000-0000-0000-000000000002"),
            Guid.Parse("70000000-0000-0000-0000-000000000002"),
            rawBleCfg.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-25));

        EnsureUwbConfigEventAndSnapshot(
            db,
            Guid.Parse("60000000-0000-0000-0000-000000000003"),
            Guid.Parse("70000000-0000-0000-0000-000000000003"),
            rawUwbCfg.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-24));

        EnsureDioConfigEventAndSnapshot(
            db,
            Guid.Parse("60000000-0000-0000-0000-000000000004"),
            Guid.Parse("70000000-0000-0000-0000-000000000004"),
            rawDioCfg.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-23),
            1);

        EnsureDioValueEventAndSnapshot(
            db,
            Guid.Parse("60000000-0000-0000-0000-000000000005"),
            Guid.Parse("70000000-0000-0000-0000-000000000005"),
            rawDioVal.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-5),
            1,
            true);

        EnsureI2cConfigEventAndSnapshot(
            db,
            Guid.Parse("60000000-0000-0000-0000-000000000006"),
            Guid.Parse("70000000-0000-0000-0000-000000000006"),
            rawI2cCfg.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-22));

        EnsureI2cDataEvent(
            db,
            Guid.Parse("60000000-0000-0000-0000-000000000007"),
            rawI2cData.Id,
            tagsByExternalId["tag-person-001"].Id,
            DateTime.UtcNow.AddMinutes(-4));

        db.SaveChanges();

        // =========================================================
        // ALARMS
        // =========================================================
        EnsureAlarm(
            db,
            Guid.Parse("80000000-0000-0000-0000-000000000001"),
            rawEmergency.Id,
            AlarmType.EmergencyButtonPressed,
            AlarmSeverity.Critical,
            AlarmStatus.Active,
            tagsByExternalId["tag-person-001"].Id,
            null,
            null,
            worker1.Id,
            DateTime.UtcNow.AddMinutes(-8),
            null,
            null,
            null,
            "Acil durum butonu basıldı",
            "Worker1 üzerindeki tag emergency alarm üretti.",
            JsonSerializer.Serialize(new { Demo = true, Scenario = "Emergency" }));

        EnsureAlarm(
            db,
            Guid.Parse("80000000-0000-0000-0000-000000000002"),
            rawProximity.Id,
            AlarmType.ProximityAlert,
            AlarmSeverity.Critical,
            AlarmStatus.Acknowledged,
            tagsByExternalId["tag-person-002"].Id,
            tagsByExternalId["tag-vehicle-001"].Id,
            null,
            worker2.Id,
            DateTime.UtcNow.AddMinutes(-4),
            null,
            DateTime.UtcNow.AddMinutes(-3),
            safety1.Id,
            "Araç-personel yakınlık alarmı",
            "Kritik yakınlık eşiği aşıldı.",
            JsonSerializer.Serialize(new { Demo = true, Scenario = "Proximity" }));

        EnsureAlarm(
            db,
            Guid.Parse("80000000-0000-0000-0000-000000000003"),
            rawBattery.Id,
            AlarmType.LowBattery,
            AlarmSeverity.Warning,
            AlarmStatus.Active,
            tagsByExternalId["tag-asset-001"].Id,
            null,
            anchorsByExternalId["anchor-004"].Id,
            null,
            DateTime.UtcNow.AddMinutes(-10),
            null,
            null,
            null,
            "Düşük batarya",
            "Asset tag batarya seviyesi kritik eşiğe yaklaştı.",
            JsonSerializer.Serialize(new { Demo = true, Scenario = "BatteryLow" }));

        EnsureAlarm(
            db,
            Guid.Parse("80000000-0000-0000-0000-000000000004"),
            rawStatus.Id,
            AlarmType.AnchorError,
            AlarmSeverity.Critical,
            AlarmStatus.Resolved,
            null,
            null,
            anchorsByExternalId["anchor-005"].Id,
            null,
            DateTime.UtcNow.AddMinutes(-7),
            DateTime.UtcNow.AddMinutes(-1),
            DateTime.UtcNow.AddMinutes(-6),
            maintenance1.Id,
            "Anchor hata alarmı",
            "Anchor-005 hata durumuna geçti.",
            JsonSerializer.Serialize(new { Demo = true, Scenario = "AnchorError" }));

        db.SaveChanges();

        // =========================================================
        // COMMAND REQUESTS + STATUS HISTORY + OUTBOX
        // =========================================================
        var cmd1 = EnsureCommandRequest(
            db,
            Guid.Parse("90000000-0000-0000-0000-000000000001"),
            RtlsCommandType.SetTagAlert,
            RtlsCommandStatus.Pending,
            RtlsCommandTargetType.Tag,
            tagsByExternalId["tag-person-001"].Id,
            null,
            dispatch1.Id,
            JsonSerializer.Serialize(new
            {
                type = "SetTagAlert",
                tagId = "tag-person-001",
                buzzerEnabled = true,
                vibrationEnabled = true,
                ledEnabled = true,
                ledColor = "Red"
            }),
            DateTime.UtcNow.AddMinutes(-2));

        var cmd2 = EnsureCommandRequest(
            db,
            Guid.Parse("90000000-0000-0000-0000-000000000002"),
            RtlsCommandType.SetDIOValue,
            RtlsCommandStatus.Queued,
            RtlsCommandTargetType.Tag,
            tagsByExternalId["tag-person-001"].Id,
            null,
            maintenance1.Id,
            JsonSerializer.Serialize(new
            {
                type = "SetDIOValue",
                tagId = "tag-person-001",
                pin = 1,
                value = true
            }),
            DateTime.UtcNow.AddMinutes(-15),
            queuedAt: DateTime.UtcNow.AddMinutes(-14));

        var cmd3 = EnsureCommandRequest(
            db,
            Guid.Parse("90000000-0000-0000-0000-000000000003"),
            RtlsCommandType.ResetDevice,
            RtlsCommandStatus.Sent,
            RtlsCommandTargetType.Anchor,
            null,
            anchorsByExternalId["anchor-005"].Id,
            maintenance1.Id,
            JsonSerializer.Serialize(new
            {
                type = "ResetDevice",
                anchorId = "anchor-005"
            }),
            DateTime.UtcNow.AddMinutes(-20),
            queuedAt: DateTime.UtcNow.AddMinutes(-19),
            sentAt: DateTime.UtcNow.AddMinutes(-18),
            externalCorrelationId: "vendor-cmd-0003");

        var cmd4 = EnsureCommandRequest(
            db,
            Guid.Parse("90000000-0000-0000-0000-000000000004"),
            RtlsCommandType.SetUWBConfig,
            RtlsCommandStatus.Succeeded,
            RtlsCommandTargetType.Tag,
            tagsByExternalId["tag-person-002"].Id,
            null,
            maintenance1.Id,
            JsonSerializer.Serialize(new
            {
                type = "SetUWBConfig",
                tagId = "tag-person-002",
                enabled = true,
                channel = 5,
                txPower = 12.5,
                rangingInterval = 1000
            }),
            DateTime.UtcNow.AddHours(-1),
            queuedAt: DateTime.UtcNow.AddMinutes(-58),
            sentAt: DateTime.UtcNow.AddMinutes(-57),
            completedAt: DateTime.UtcNow.AddMinutes(-56),
            externalCorrelationId: "vendor-cmd-0004",
            responseJson: JsonSerializer.Serialize(new { success = true, message = "Accepted" }));

        var cmd5 = EnsureCommandRequest(
            db,
            Guid.Parse("90000000-0000-0000-0000-000000000005"),
            RtlsCommandType.RequestConfig,
            RtlsCommandStatus.Failed,
            RtlsCommandTargetType.Anchor,
            null,
            anchorsByExternalId["anchor-006"].Id,
            maintenance1.Id,
            JsonSerializer.Serialize(new
            {
                type = "RequestConfig",
                anchorId = "anchor-006"
            }),
            DateTime.UtcNow.AddHours(-2),
            queuedAt: DateTime.UtcNow.AddHours(-2).AddMinutes(1),
            sentAt: DateTime.UtcNow.AddHours(-2).AddMinutes(2),
            failedAt: DateTime.UtcNow.AddHours(-2).AddMinutes(3),
            externalCorrelationId: "vendor-cmd-0005",
            failureReason: "Device offline",
            responseJson: JsonSerializer.Serialize(new { success = false, message = "Device offline" }));

        db.SaveChanges();

        EnsureCommandHistory(
            db,
            Guid.Parse("91000000-0000-0000-0000-000000000001"),
            cmd2.Id,
            null,
            RtlsCommandStatus.Queued,
            maintenance1.Id,
            DateTime.UtcNow.AddMinutes(-14),
            "Command queued",
            null);

        EnsureCommandHistory(
            db,
            Guid.Parse("91000000-0000-0000-0000-000000000002"),
            cmd3.Id,
            null,
            RtlsCommandStatus.Sent,
            null,
            DateTime.UtcNow.AddMinutes(-18),
            "Integration marked command as sent",
            JsonSerializer.Serialize(new { externalCorrelationId = "vendor-cmd-0003" }));

        EnsureCommandHistory(
            db,
            Guid.Parse("91000000-0000-0000-0000-000000000003"),
            cmd4.Id,
            RtlsCommandStatus.Sent,
            RtlsCommandStatus.Succeeded,
            null,
            DateTime.UtcNow.AddMinutes(-56),
            "Integration marked command as succeeded",
            JsonSerializer.Serialize(new { externalCorrelationId = "vendor-cmd-0004" }));

        EnsureCommandHistory(
            db,
            Guid.Parse("91000000-0000-0000-0000-000000000004"),
            cmd5.Id,
            RtlsCommandStatus.Sent,
            RtlsCommandStatus.Failed,
            null,
            DateTime.UtcNow.AddHours(-2).AddMinutes(3),
            "Integration marked command as failed",
            JsonSerializer.Serialize(new { externalCorrelationId = "vendor-cmd-0005" }));

        db.SaveChanges();

        EnsureOutboxMessage(
            db,
            Guid.Parse("92000000-0000-0000-0000-000000000001"),
            "CommandRequest",
            cmd2.Id,
            "RtlsCommandQueued",
            JsonSerializer.Serialize(new
            {
                commandRequestId = cmd2.Id,
                commandType = "SetDIOValue",
                targetType = "Tag",
                tagId = cmd2.TagId,
                payload = "demo"
            }),
            OutboxMessageStatus.Pending,
            DateTime.UtcNow.AddMinutes(-14));

        EnsureOutboxMessage(
            db,
            Guid.Parse("92000000-0000-0000-0000-000000000002"),
            "CommandRequest",
            cmd3.Id,
            "RtlsCommandQueued",
            JsonSerializer.Serialize(new
            {
                commandRequestId = cmd3.Id,
                commandType = "ResetDevice",
                targetType = "Anchor",
                anchorId = cmd3.AnchorId,
                payload = "demo"
            }),
            OutboxMessageStatus.Dispatched,
            DateTime.UtcNow.AddMinutes(-19),
            dispatchedAt: DateTime.UtcNow.AddMinutes(-18));

        EnsureOutboxMessage(
            db,
            Guid.Parse("92000000-0000-0000-0000-000000000003"),
            "CommandRequest",
            cmd5.Id,
            "RtlsCommandQueued",
            JsonSerializer.Serialize(new
            {
                commandRequestId = cmd5.Id,
                commandType = "RequestConfig",
                targetType = "Anchor",
                anchorId = cmd5.AnchorId,
                payload = "demo"
            }),
            OutboxMessageStatus.Failed,
            DateTime.UtcNow.AddHours(-2).AddMinutes(1),
            failedAt: DateTime.UtcNow.AddHours(-2).AddMinutes(2),
            failureReason: "Network timeout to integration");

        db.SaveChanges();

        Console.WriteLine("✓ RTLS demo verileri tamamlandı.");

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("✅ SeedData başarıyla tamamlandı.");
        Console.WriteLine("========================================");
        Console.WriteLine();
    }

    private sealed record UserSeedItem(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string RoleName,
        string UserTypeCode,
        string SpecializationCode
    );

    private static void SetTagRuntime(Tag tag, TagStatus status, int? batteryLevel, DateTime? lastSeenAt)
    {
        if (lastSeenAt.HasValue)
        {
            tag.SetStatus(status, lastSeenAt.Value);
            tag.MarkSeen(lastSeenAt.Value);
        }
        else
        {
            tag.SetStatus(status);
        }

        if (batteryLevel.HasValue)
        {
            tag.UpdateBattery(batteryLevel.Value, lastSeenAt ?? DateTime.UtcNow);
        }
    }

    private static void SetAnchorRuntime(Anchor anchor, AnchorStatus status, string ipAddress, DateTime at)
    {
        anchor.RegisterHeartbeat(at, ipAddress);
        anchor.ChangeStatus(status, at);
    }

    private static void EnsureTagAssignment(AppDbContext db, Guid tagId, Guid userId, bool isPrimary)
    {
        if (db.TagAssignments.Any(x => x.TagId == tagId && x.UserId == userId && x.UnassignedAt == null))
            return;

        db.TagAssignments.Add(TagAssignment.Create(tagId, userId, isPrimary));
    }

    private static RawEvent EnsureRawEvent(
        AppDbContext db,
        Guid externalEventId,
        string eventType,
        DateTime eventTimestamp,
        string? tagExternalId,
        string? anchorExternalId,
        string payloadJson,
        RawEventProcessingStatus status)
    {
        var existing = db.RawEvents.FirstOrDefault(x => x.ExternalEventId == externalEventId);
        if (existing is not null)
            return existing;

        var raw = RawEvent.Create(
            externalEventId,
            eventType,
            eventTimestamp,
            payloadJson,
            tagExternalId,
            anchorExternalId,
            eventTimestamp);

        switch (status)
        {
            case RawEventProcessingStatus.Processed:
                raw.MarkProcessed();
                break;
            case RawEventProcessingStatus.Failed:
                raw.MarkFailed("Seed failed raw event");
                break;
            case RawEventProcessingStatus.Ignored:
                raw.MarkIgnored("Seed ignored raw event");
                break;
        }

        db.RawEvents.Add(raw);
        return raw;
    }

    private static void EnsureLocationEvent(AppDbContext db, Guid id, Guid rawEventId, Guid tagId, DateTime eventAt,
        decimal x, decimal y, decimal z, decimal accuracy, decimal confidence, string usedAnchorsJson)
    {
        if (db.LocationEvents.Any(xe => xe.Id == id))
            return;

        var entity = LocationEvent.Create(rawEventId, tagId, eventAt, x, y, z, accuracy, confidence, usedAnchorsJson);
        SetEntityId(entity, id);
        db.LocationEvents.Add(entity);
    }

    private static void EnsureCurrentLocation(AppDbContext db, Guid id, Guid tagId, Guid? userId,
        decimal x, decimal y, decimal z, decimal accuracy, decimal confidence,
        DateTime lastEventAt, Guid lastRawEventId, int anchorCount)
    {
        if (db.CurrentLocations.Any(xe => xe.TagId == tagId))
            return;

        var entity = CurrentLocation.Create(tagId, userId, x, y, z, accuracy, confidence, lastEventAt, lastRawEventId, anchorCount);
        SetEntityId(entity, id);
        db.CurrentLocations.Add(entity);
    }

    private static void EnsureBatteryEvent(AppDbContext db, Guid id, Guid rawEventId, Guid tagId, Guid anchorId, DateTime eventAt, int batteryLevel)
    {
        if (db.BatteryEvents.Any(x => x.Id == id))
            return;

        var entity = BatteryEvent.Create(rawEventId, tagId, anchorId, eventAt, batteryLevel);
        SetEntityId(entity, id);
        db.BatteryEvents.Add(entity);
    }

    private static void EnsureEmergencyEvent(AppDbContext db, Guid id, Guid rawEventId, Guid tagId, DateTime eventAt)
    {
        if (db.EmergencyEvents.Any(x => x.Id == id))
            return;

        var entity = EmergencyEvent.Create(rawEventId, tagId, eventAt);
        SetEntityId(entity, id);
        db.EmergencyEvents.Add(entity);
    }

    private static void EnsureProximityEvent(AppDbContext db, Guid id, Guid rawEventId, Guid tagId, Guid peerTagId, DateTime eventAt,
        decimal distance, decimal threshold, ProximitySeverity severity)
    {
        if (db.ProximityEvents.Any(x => x.Id == id))
            return;

        var entity = ProximityEvent.Create(rawEventId, tagId, peerTagId, eventAt, distance, threshold, severity);
        SetEntityId(entity, id);
        db.ProximityEvents.Add(entity);
    }

    private static void EnsureUwbRangingEvent(AppDbContext db, Guid id, Guid rawEventId, Guid anchorId, Guid tagId, DateTime eventAt, decimal distance)
    {
        if (db.UwbRangingEvents.Any(x => x.Id == id))
            return;

        var entity = NewEntity<UwbRangingEvent>();
        SetEntityId(entity, id);
        Set(entity, "RawEventId", rawEventId);
        Set(entity, "AnchorId", anchorId);
        Set(entity, "TagId", tagId);
        Set(entity, "EventTimestamp", eventAt);
        Set(entity, "Distance", distance);
        SetCommonAudit(entity);
        db.UwbRangingEvents.Add(entity);
    }

    private static void EnsureUwbTagToTagRangingEvent(AppDbContext db, Guid id, Guid rawEventId, Guid tagId, Guid peerTagId, DateTime eventAt, decimal distance)
    {
        if (db.UwbTagToTagRangingEvents.Any(x => x.Id == id))
            return;

        var entity = NewEntity<UwbTagToTagRangingEvent>();
        SetEntityId(entity, id);
        Set(entity, "RawEventId", rawEventId);
        Set(entity, "TagId", tagId);
        Set(entity, "PeerTagId", peerTagId);
        Set(entity, "EventTimestamp", eventAt);
        Set(entity, "Distance", distance);
        SetCommonAudit(entity);
        db.UwbTagToTagRangingEvents.Add(entity);
    }

    private static void EnsureBleAdvertisementEvent(AppDbContext db, Guid id, Guid rawEventId, Guid anchorId, Guid tagId, DateTime eventAt, int rssi)
    {
        if (db.BleAdvertisementEvents.Any(x => x.Id == id))
            return;

        var entity = NewEntity<BleAdvertisementEvent>();
        SetEntityId(entity, id);
        Set(entity, "RawEventId", rawEventId);
        Set(entity, "AnchorId", anchorId);
        Set(entity, "TagId", tagId);
        Set(entity, "EventTimestamp", eventAt);
        Set(entity, "Rssi", rssi);
        SetCommonAudit(entity);
        db.BleAdvertisementEvents.Add(entity);
    }

    private static void EnsureTagDataEvent(AppDbContext db, Guid id, Guid rawEventId, Guid anchorId, Guid tagId, DateTime eventAt, string? reportedTagType)
    {
        if (db.TagDataEvents.Any(x => x.Id == id))
            return;

        var entity = NewEntity<TagDataEvent>();
        SetEntityId(entity, id);
        Set(entity, "RawEventId", rawEventId);
        Set(entity, "AnchorId", anchorId);
        Set(entity, "TagId", tagId);
        Set(entity, "EventTimestamp", eventAt);
        Set(entity, "ReportedTagType", reportedTagType);
        SetCommonAudit(entity);
        db.TagDataEvents.Add(entity);
    }

    private static void EnsureAnchorHeartbeatEvent(AppDbContext db, Guid id, Guid rawEventId, Guid anchorId, DateTime eventAt, string ipAddress)
    {
        if (db.AnchorHeartbeatEvents.Any(x => x.Id == id))
            return;

        var entity = AnchorHeartbeatEvent.Create(rawEventId, anchorId, eventAt, ipAddress);
        SetEntityId(entity, id);
        db.AnchorHeartbeatEvents.Add(entity);
    }

    private static void EnsureAnchorStatusEvent(AppDbContext db, Guid id, Guid rawEventId, Guid anchorId, DateTime eventAt,
        AnchorStatus status, AnchorStatus previousStatus, string? reason)
    {
        if (db.AnchorStatusEvents.Any(x => x.Id == id))
            return;

        var entity = AnchorStatusEvent.Create(rawEventId, anchorId, eventAt, status, previousStatus, reason);
        SetEntityId(entity, id);
        db.AnchorStatusEvents.Add(entity);
    }

    private static void EnsureAnchorHealthEvent(AppDbContext db, Guid id, Guid rawEventId, Guid anchorId, DateTime eventAt,
        long uptime, decimal temperature, decimal cpuUsage, decimal memoryUsage, int tagCount, decimal packetLossRate)
    {
        if (db.AnchorHealthEvents.Any(x => x.Id == id))
            return;

        var entity = NewEntity<AnchorHealthEvent>();
        SetEntityId(entity, id);
        Set(entity, "RawEventId", rawEventId);
        Set(entity, "AnchorId", anchorId);
        Set(entity, "EventTimestamp", eventAt);
        Set(entity, "Uptime", uptime);
        Set(entity, "Temperature", temperature);
        Set(entity, "CpuUsage", cpuUsage);
        Set(entity, "MemoryUsage", memoryUsage);
        Set(entity, "TagCount", tagCount);
        Set(entity, "PacketLossRate", packetLossRate);
        SetCommonAudit(entity);
        db.AnchorHealthEvents.Add(entity);
    }

    private static void EnsureAnchorConfigEventAndSnapshot(AppDbContext db, Guid eventId, Guid snapshotId, Guid rawEventId, Guid anchorId, DateTime reportedAt)
    {
        var firmwareVersion = "1.2.0";
        var positionJson = JsonSerializer.Serialize(new { x = 0, y = 0, z = 0 });
        var networkJson = JsonSerializer.Serialize(new { ipAddress = "10.10.1.11", gateway = "10.10.1.1", subnet = "255.255.255.0" });
        var uwbJson = JsonSerializer.Serialize(new { channel = 5, enabled = true });
        var bleJson = JsonSerializer.Serialize(new { enabled = true });

        if (!db.AnchorConfigEvents.Any(x => x.Id == eventId))
        {
            var evt = NewEntity<AnchorConfigEvent>();
            SetEntityId(evt, eventId);
            Set(evt, "RawEventId", rawEventId);
            Set(evt, "AnchorId", anchorId);
            Set(evt, "EventTimestamp", reportedAt);
            Set(evt, "FirmwareVersion", firmwareVersion);
            Set(evt, "PositionJson", positionJson);
            Set(evt, "NetworkJson", networkJson);
            Set(evt, "UwbJson", uwbJson);
            Set(evt, "BleJson", bleJson);
            Set(evt, "HeartbeatInterval", 5000L);
            Set(evt, "ReportInterval", 10000L);
            SetCommonAudit(evt);
            db.AnchorConfigEvents.Add(evt);
        }

        if (!db.AnchorConfigSnapshots.Any(x => x.AnchorId == anchorId))
        {
            var snap = NewEntity<AnchorConfigSnapshot>();
            SetEntityId(snap, snapshotId);
            Set(snap, "AnchorId", anchorId);
            Set(snap, "LastRawEventId", rawEventId);
            Set(snap, "LastReportedAt", reportedAt);
            Set(snap, "FirmwareVersion", firmwareVersion);
            Set(snap, "PositionJson", positionJson);
            Set(snap, "NetworkJson", networkJson);
            Set(snap, "UwbJson", uwbJson);
            Set(snap, "BleJson", bleJson);
            Set(snap, "HeartbeatInterval", 5000L);
            Set(snap, "ReportInterval", 10000L);
            SetCommonAudit(snap);
            db.AnchorConfigSnapshots.Add(snap);
        }
    }

    private static void EnsureBleConfigEventAndSnapshot(AppDbContext db, Guid eventId, Guid snapshotId, Guid rawEventId, Guid tagId, DateTime reportedAt)
    {
        if (!db.BleConfigEvents.Any(x => x.Id == eventId))
        {
            var evt = NewEntity<BleConfigEvent>();
            SetEntityId(evt, eventId);
            Set(evt, "RawEventId", rawEventId);
            Set(evt, "TagId", tagId);
            Set(evt, "EventTimestamp", reportedAt);
            Set(evt, "Enabled", true);
            Set(evt, "TxPower", 8.5m);
            Set(evt, "AdvertisementInterval", 1000L);
            Set(evt, "MeshEnabled", false);
            SetCommonAudit(evt);
            db.BleConfigEvents.Add(evt);
        }

        if (!db.TagBleConfigSnapshots.Any(x => x.TagId == tagId))
        {
            var snap = NewEntity<TagBleConfigSnapshot>();
            SetEntityId(snap, snapshotId);
            Set(snap, "TagId", tagId);
            Set(snap, "LastRawEventId", rawEventId);
            Set(snap, "LastReportedAt", reportedAt);
            Set(snap, "Enabled", true);
            Set(snap, "TxPower", 8.5m);
            Set(snap, "AdvertisementInterval", 1000L);
            Set(snap, "MeshEnabled", false);
            SetCommonAudit(snap);
            db.TagBleConfigSnapshots.Add(snap);
        }
    }

    private static void EnsureUwbConfigEventAndSnapshot(AppDbContext db, Guid eventId, Guid snapshotId, Guid rawEventId, Guid tagId, DateTime reportedAt)
    {
        if (!db.UwbConfigEvents.Any(x => x.Id == eventId))
        {
            var evt = NewEntity<UwbConfigEvent>();
            SetEntityId(evt, eventId);
            Set(evt, "RawEventId", rawEventId);
            Set(evt, "TagId", tagId);
            Set(evt, "EventTimestamp", reportedAt);
            Set(evt, "Enabled", true);
            Set(evt, "Channel", 5);
            Set(evt, "TxPower", 12.5m);
            Set(evt, "RangingInterval", 1000L);
            Set(evt, "TagToTagEnabled", true);
            Set(evt, "TagToTagInterval", 500L);
            SetCommonAudit(evt);
            db.UwbConfigEvents.Add(evt);
        }

        if (!db.TagUwbConfigSnapshots.Any(x => x.TagId == tagId))
        {
            var snap = NewEntity<TagUwbConfigSnapshot>();
            SetEntityId(snap, snapshotId);
            Set(snap, "TagId", tagId);
            Set(snap, "LastRawEventId", rawEventId);
            Set(snap, "LastReportedAt", reportedAt);
            Set(snap, "Enabled", true);
            Set(snap, "Channel", 5);
            Set(snap, "TxPower", 12.5m);
            Set(snap, "RangingInterval", 1000L);
            Set(snap, "TagToTagEnabled", true);
            Set(snap, "TagToTagInterval", 500L);
            SetCommonAudit(snap);
            db.TagUwbConfigSnapshots.Add(snap);
        }
    }

    private static void EnsureDioConfigEventAndSnapshot(AppDbContext db, Guid eventId, Guid snapshotId, Guid rawEventId, Guid tagId, DateTime reportedAt, int pin)
    {
        var lowPassFilterJson = JsonSerializer.Serialize(new { enabled = false, alpha = 0.0 });

        if (!db.DioConfigEvents.Any(x => x.Id == eventId))
        {
            var evt = NewEntity<DioConfigEvent>();
            SetEntityId(evt, eventId);
            Set(evt, "RawEventId", rawEventId);
            Set(evt, "TagId", tagId);
            Set(evt, "EventTimestamp", reportedAt);
            Set(evt, "Pin", pin);
            Set(evt, "Direction", DioPinDirection.Output);
            Set(evt, "PeriodicReportEnabled", true);
            Set(evt, "PeriodicReportInterval", 5000L);
            Set(evt, "ReportOnChange", true);
            Set(evt, "LowPassFilterJson", lowPassFilterJson);
            SetCommonAudit(evt);
            db.DioConfigEvents.Add(evt);
        }

        if (!db.TagDioConfigSnapshots.Any(x => x.TagId == tagId && x.Pin == pin))
        {
            var snap = NewEntity<TagDioConfigSnapshot>();
            SetEntityId(snap, snapshotId);
            Set(snap, "TagId", tagId);
            Set(snap, "Pin", pin);
            Set(snap, "LastRawEventId", rawEventId);
            Set(snap, "LastReportedAt", reportedAt);
            Set(snap, "Direction", DioPinDirection.Output);
            Set(snap, "PeriodicReportEnabled", true);
            Set(snap, "PeriodicReportInterval", 5000L);
            Set(snap, "ReportOnChange", true);
            Set(snap, "LowPassFilterJson", lowPassFilterJson);
            SetCommonAudit(snap);
            db.TagDioConfigSnapshots.Add(snap);
        }
    }

    private static void EnsureDioValueEventAndSnapshot(AppDbContext db, Guid eventId, Guid snapshotId, Guid rawEventId, Guid tagId, DateTime reportedAt, int pin, bool value)
    {
        if (!db.DioValueEvents.Any(x => x.Id == eventId))
        {
            var evt = NewEntity<DioValueEvent>();
            SetEntityId(evt, eventId);
            Set(evt, "RawEventId", rawEventId);
            Set(evt, "TagId", tagId);
            Set(evt, "EventTimestamp", reportedAt);
            Set(evt, "Pin", pin);
            Set(evt, "Value", value);
            SetCommonAudit(evt);
            db.DioValueEvents.Add(evt);
        }

        if (!db.TagDioValueSnapshots.Any(x => x.TagId == tagId && x.Pin == pin))
        {
            var snap = NewEntity<TagDioValueSnapshot>();
            SetEntityId(snap, snapshotId);
            Set(snap, "TagId", tagId);
            Set(snap, "Pin", pin);
            Set(snap, "LastRawEventId", rawEventId);
            Set(snap, "LastReportedAt", reportedAt);
            Set(snap, "Value", value);
            SetCommonAudit(snap);
            db.TagDioValueSnapshots.Add(snap);
        }
    }

    private static void EnsureI2cConfigEventAndSnapshot(AppDbContext db, Guid eventId, Guid snapshotId, Guid rawEventId, Guid tagId, DateTime reportedAt)
    {
        var devicesJson = JsonSerializer.Serialize(new[]
        {
            new { address = 64, name = "TempSensor" },
            new { address = 65, name = "PressureSensor" }
        });

        if (!db.I2cConfigEvents.Any(x => x.Id == eventId))
        {
            var evt = NewEntity<I2cConfigEvent>();
            SetEntityId(evt, eventId);
            Set(evt, "RawEventId", rawEventId);
            Set(evt, "TagId", tagId);
            Set(evt, "EventTimestamp", reportedAt);
            Set(evt, "Enabled", true);
            Set(evt, "ClockSpeed", 400000);
            Set(evt, "DevicesJson", devicesJson);
            SetCommonAudit(evt);
            db.I2cConfigEvents.Add(evt);
        }

        if (!db.TagI2cConfigSnapshots.Any(x => x.TagId == tagId))
        {
            var snap = NewEntity<TagI2cConfigSnapshot>();
            SetEntityId(snap, snapshotId);
            Set(snap, "TagId", tagId);
            Set(snap, "LastRawEventId", rawEventId);
            Set(snap, "LastReportedAt", reportedAt);
            Set(snap, "Enabled", true);
            Set(snap, "ClockSpeed", 400000);
            Set(snap, "DevicesJson", devicesJson);
            SetCommonAudit(snap);
            db.TagI2cConfigSnapshots.Add(snap);
        }
    }

    private static void EnsureI2cDataEvent(AppDbContext db, Guid eventId, Guid rawEventId, Guid tagId, DateTime reportedAt)
    {
        if (db.I2cDataEvents.Any(x => x.Id == eventId))
            return;

        var evt = NewEntity<I2cDataEvent>();
        SetEntityId(evt, eventId);
        Set(evt, "RawEventId", rawEventId);
        Set(evt, "TagId", tagId);
        Set(evt, "EventTimestamp", reportedAt);
        Set(evt, "Address", 64);
        Set(evt, "Register", 1);
        Set(evt, "Direction", I2cDataDirection.Read);
        Set(evt, "Ack", true);
        Set(evt, "DataJson", JsonSerializer.Serialize(new[] { 24, 25, 26 }));
        SetCommonAudit(evt);
        db.I2cDataEvents.Add(evt);
    }

    private static Alarm EnsureAlarm(AppDbContext db, Guid id, Guid? rawEventId, AlarmType alarmType, AlarmSeverity severity,
        AlarmStatus status, Guid? tagId, Guid? peerTagId, Guid? anchorId, Guid? userId,
        DateTime startedAt, DateTime? endedAt, DateTime? acknowledgedAt, Guid? acknowledgedBy,
        string title, string? description, string? dataJson)
    {
        var existing = db.Alarms.FirstOrDefault(x => x.Id == id);
        if (existing is not null)
            return existing;

        var alarm = Alarm.Create(alarmType, severity, title, startedAt, rawEventId, tagId, peerTagId, anchorId, userId, description, dataJson);
        SetEntityId(alarm, id);

        if (status == AlarmStatus.Acknowledged)
        {
            alarm.Acknowledge(acknowledgedBy ?? Guid.Empty, acknowledgedAt ?? DateTime.UtcNow);
        }
        else if (status == AlarmStatus.Resolved)
        {
            if (acknowledgedAt.HasValue && acknowledgedBy.HasValue)
                alarm.Acknowledge(acknowledgedBy.Value, acknowledgedAt.Value);
            alarm.Resolve(endedAt ?? DateTime.UtcNow);
        }
        else if (status == AlarmStatus.Closed)
        {
            if (acknowledgedAt.HasValue && acknowledgedBy.HasValue)
                alarm.Acknowledge(acknowledgedBy.Value, acknowledgedAt.Value);
            alarm.Close(endedAt ?? DateTime.UtcNow);
        }

        db.Alarms.Add(alarm);
        return alarm;
    }

    private static CommandRequest EnsureCommandRequest(
        AppDbContext db,
        Guid id,
        RtlsCommandType commandType,
        RtlsCommandStatus status,
        RtlsCommandTargetType targetType,
        Guid? tagId,
        Guid? anchorId,
        Guid requestedByUserId,
        string payloadJson,
        DateTime requestedAt,
        DateTime? queuedAt = null,
        DateTime? sentAt = null,
        DateTime? completedAt = null,
        DateTime? failedAt = null,
        string? externalCorrelationId = null,
        string? failureReason = null,
        string? responseJson = null)
    {
        var existing = db.CommandRequests.FirstOrDefault(x => x.Id == id);
        if (existing is not null)
            return existing;

        var command = CommandRequest.Create(commandType, targetType, requestedByUserId, payloadJson, tagId, anchorId, requestedAt);
        SetEntityId(command, id);

        if (status == RtlsCommandStatus.Queued)
        {
            command.MarkQueued();
            if (queuedAt.HasValue) Set(command, "QueuedAt", queuedAt.Value);
        }
        else if (status == RtlsCommandStatus.Sent)
        {
            command.MarkQueued();
            command.MarkSent(externalCorrelationId);
            if (queuedAt.HasValue) Set(command, "QueuedAt", queuedAt.Value);
            if (sentAt.HasValue) Set(command, "SentAt", sentAt.Value);
        }
        else if (status == RtlsCommandStatus.Succeeded)
        {
            command.MarkQueued();
            command.MarkSent(externalCorrelationId);
            command.MarkSucceeded(responseJson, externalCorrelationId);
            if (queuedAt.HasValue) Set(command, "QueuedAt", queuedAt.Value);
            if (sentAt.HasValue) Set(command, "SentAt", sentAt.Value);
            if (completedAt.HasValue) Set(command, "CompletedAt", completedAt.Value);
        }
        else if (status == RtlsCommandStatus.Failed)
        {
            command.MarkQueued();
            command.MarkSent(externalCorrelationId);
            command.MarkFailed(failureReason, responseJson, externalCorrelationId);
            if (queuedAt.HasValue) Set(command, "QueuedAt", queuedAt.Value);
            if (sentAt.HasValue) Set(command, "SentAt", sentAt.Value);
            if (failedAt.HasValue) Set(command, "FailedAt", failedAt.Value);
        }

        db.CommandRequests.Add(command);
        return command;
    }

    private static void EnsureCommandHistory(AppDbContext db, Guid id, Guid commandRequestId, RtlsCommandStatus? oldStatus,
        RtlsCommandStatus newStatus, Guid? changedByUserId, DateTime changedAt, string? note, string? dataJson)
    {
        if (db.CommandStatusHistories.Any(x => x.Id == id))
            return;

        var history = CommandStatusHistory.Create(commandRequestId, oldStatus, newStatus, changedByUserId, note, dataJson);
        SetEntityId(history, id);
        Set(history, "ChangedAt", changedAt);
        db.CommandStatusHistories.Add(history);
    }

    private static void EnsureOutboxMessage(AppDbContext db, Guid id, string aggregateType, Guid aggregateId, string messageType,
        string payloadJson, OutboxMessageStatus status, DateTime occurredAt,
        DateTime? dispatchedAt = null, DateTime? failedAt = null, string? failureReason = null)
    {
        if (db.OutboxMessages.Any(x => x.Id == id))
            return;

        var message = OutboxMessage.Create(aggregateType, aggregateId, messageType, payloadJson, occurredAt);
        SetEntityId(message, id);

        if (status == OutboxMessageStatus.Dispatched)
        {
            message.MarkDispatched();
            if (dispatchedAt.HasValue) Set(message, "DispatchedAt", dispatchedAt.Value);
        }
        else if (status == OutboxMessageStatus.Failed)
        {
            message.MarkFailed(failureReason);
            if (failedAt.HasValue) Set(message, "FailedAt", failedAt.Value);
        }

        db.OutboxMessages.Add(message);
    }

    private static T NewEntity<T>() where T : class
        => (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;

    private static void SetEntityId(object entity, Guid id) => Set(entity, nameof(BaseEntity.Id), id);

    private static void SetCommonAudit(object entity)
    {
        Set(entity, nameof(BaseEntity.CreatedAt), DateTime.UtcNow);
        Set(entity, nameof(BaseEntity.UpdatedAt), null);
        Set(entity, nameof(BaseEntity.DeletedAt), null);
    }

    private static void Set(object entity, string propertyName, object? value)
    {
        var prop = entity.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop == null)
            throw new InvalidOperationException($"{entity.GetType().Name}.{propertyName} property not found.");

        prop.SetValue(entity, value);
    }
}