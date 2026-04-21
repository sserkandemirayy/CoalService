using Domain.Entities;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        db.Database.Migrate();

        // === USER TYPES ===
        if (!db.UserTypes.Any())
        {
            db.UserTypes.AddRange(
                UserType.Create("USER", "Kullanıcı", "Genel sistem kullanıcısı", true),
                UserType.Create("DOCTOR", "Doktor", "Tıbbi personel - hekim", true),
                UserType.Create("STAFF", "Personel", "Destek personeli", true),
                UserType.Create("PATIENT", "Hasta", "Tedavi gören danışan", true)
            );
            db.SaveChanges();
            Console.WriteLine("✓ Kullanıcı tipleri oluşturuldu.");
        }

        // === SYSTEM USER ===
        var systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userTypeSys = db.UserTypes.First(x => x.Code == "USER").Id;
        if (!db.Users.Any(u => u.Id == systemUserId))
        {
            var sysUser = User.Create("system@local", "!!SYSTEM!!", "Sistem", "Kullanıcısı", userTypeSys);
            typeof(User).GetProperty(nameof(User.Id))!.SetValue(sysUser, systemUserId);
            db.Users.Add(sysUser);
            Console.WriteLine("✓ Sistem kullanıcısı oluşturuldu.");
        }

        // === ROLES ===
        var roleNames = new[] { "admin", "user", "patient", "doctor", "staff" };
        foreach (var name in roleNames)
        {
            if (!db.Roles.Any(r => r.Name == name))
                db.Roles.Add(Role.Create(name));
        }
        db.SaveChanges();
        Console.WriteLine("✓ Roller oluşturuldu.");

        // === PERMISSIONS ===
        var permissions = new[]
        {
            "manage_users", "manage_roles", "manage_permissions",
            "access_patient_data", "edit_patient_data", "delete_patient_data",
            "create_appointment", "manage_patients", "view_reports",
            "manage_companies", "manage_branches", "manage_services", "manage_performance",
            "manage_expenses", "view_expenses", "manage_payments", "view_payments",
            "manage_products", "manage_warehouses",
            "view_pii", "edit_pii", "export_pii",
            "room:view", "room:manage", "room:assignEquipment", "room:assignStaff", "room:maintain", 
            "manage_campaigns"
        };

        foreach (var perm in permissions)
        {
            if (!db.Permissions.Any(p => p.Name == perm))
                db.Permissions.Add(Permission.Create(perm, $"{perm} yetkisi"));
        }
        db.SaveChanges();
        Console.WriteLine("✓ Yetkiler oluşturuldu.");

        // === ROLE-PERMISSION MAPPING ===
        var roles = db.Roles.ToDictionary(r => r.Name, r => r.Id);
        var perms = db.Permissions.ToDictionary(p => p.Name, p => p.Id);

        var mappings = new List<(string role, string perm)>
        {
            ("admin","manage_users"),("admin","manage_roles"),("admin","manage_permissions"),
            ("admin","view_reports"),("admin","access_patient_data"),("admin","edit_patient_data"),
            ("admin","delete_patient_data"),("admin","create_appointment"),("admin","manage_companies"),
            ("admin","manage_branches"),("admin","manage_services"),("admin","manage_expenses"),
            ("admin","view_expenses"),("admin","manage_products"),("admin","manage_warehouses"),
            ("admin","manage_performance"),("admin","manage_payments"),("admin","view_payments"),
            ("admin","manage_campaigns"),("admin","view_pii"),("admin","edit_pii"),("admin","export_pii"),
            ("doctor","access_patient_data"),("doctor","edit_patient_data"),("doctor","create_appointment"),
            ("staff","create_appointment"),("staff","view_reports"),
            ("patient","access_patient_data")
        };

        foreach (var (role, perm) in mappings)
        {
            if (roles.TryGetValue(role, out var rid) && perms.TryGetValue(perm, out var pid))
            {
                if (!db.RolePermissions.Any(rp => rp.RoleId == rid && rp.PermissionId == pid))
                    db.RolePermissions.Add(RolePermission.Create(rid, pid));
            }
        }
        db.SaveChanges();
        Console.WriteLine("✓ Rol-yetki eşleştirmeleri yapıldı.");

        // === USER SPECIALIZATIONS ===
        var userTypeDoctorId = db.UserTypes.First(x => x.Code == "DOCTOR").Id;
        if (!db.UserSpecializations.Any())
        {
            db.UserSpecializations.AddRange(
                UserSpecialization.Create(userTypeDoctorId, "DERMATOLOGY", "Dermatoloji", "Cilt hastalıkları uzmanı", false),
                UserSpecialization.Create(userTypeDoctorId, "AESTHETICS", "Estetik", "Estetik cerrahisi uzmanı", false),
                UserSpecialization.Create(userTypeDoctorId, "COSMETOLOGY", "Kozmetoloji", "Kozmetik tedaviler uzmanı", false)
            );
            db.SaveChanges();
            Console.WriteLine("✓ Uzmanlık alanları oluşturuldu.");
        }

        // === USERS ===
        var userTypeUser = db.UserTypes.First(x => x.Code == "USER").Id;
        var userTypeDoctor = db.UserTypes.First(x => x.Code == "DOCTOR").Id;
        var userTypeStaff = db.UserTypes.First(x => x.Code == "STAFF").Id;
        var userTypePatient = db.UserTypes.First(x => x.Code == "PATIENT").Id;

        var hasher = new BcryptPasswordHasher();
        var userSeeds = new List<(string email, string pass, string first, string last, string role, Guid typeId)>
        {
            ("admin@clinicpro.com","Admin123!","Sistem","Yönetici","admin", userTypeUser),
            ("ayse.yilmaz@clinicpro.com","123456","Ayşe","Yılmaz","doctor", userTypeDoctor),
            ("mehmet.demir@clinicpro.com","123456","Mehmet","Demir","doctor", userTypeDoctor),
            ("zeynep.kaya@clinicpro.com","123456","Zeynep","Kaya","staff", userTypeStaff),
            ("ali.celik@clinicpro.com","123456","Ali","Çelik","staff", userTypeStaff),
            ("fatma.arslan@gmail.com","123456","Fatma","Arslan","patient", userTypePatient),
            ("ahmet.ozturk@gmail.com","123456","Ahmet","Öztürk","patient", userTypePatient),
            ("elif.sahin@gmail.com","123456","Elif","Şahin","patient", userTypePatient),
            ("can.yildiz@gmail.com","123456","Can","Yıldız","patient", userTypePatient),
            ("selin.acar@gmail.com","123456","Selin","Acar","patient", userTypePatient)
        };

        foreach (var (email, pass, first, last, role, typeId) in userSeeds)
        {
            if (!db.Users.Any(u => u.Email == email))
            {
                var u = User.Create(email, hasher.Hash(pass), first, last, typeId);
                db.Users.Add(u);
                db.SaveChanges();

                var roleEnt = db.Roles.FirstOrDefault(r => r.Name == role);
                if (roleEnt != null)
                {
                    db.UserRoles.Add(UserRole.Create(u.Id, roleEnt.Id));
                    db.SaveChanges();
                }
                Console.WriteLine($"✓ {role} kullanıcı oluşturuldu: {first} {last}");
            }
        }

        // === SYSTEM USER ADMIN ROLE ===
        var adminRoleId = db.Roles.First(r => r.Name == "admin").Id;
        if (!db.UserRoles.Any(ur => ur.UserId == systemUserId && ur.RoleId == adminRoleId))
        {
            db.UserRoles.Add(UserRole.Create(systemUserId, adminRoleId));
            db.SaveChanges();
        }

        // === COMPANY & BRANCHES ===
        if (!db.Companies.Any())
        {
            var comp = Company.Create("Estetik Merkezi AŞ", "1234567890", "Atatürk Bulvarı No:123 Çankaya", "+90 312 555 01 01", "info@estetik.com.tr");
            db.Companies.Add(comp);
            db.SaveChanges();

            db.Branches.AddRange(
                Branch.Create(comp.Id, "Ankara Merkez", "Atatürk Bulvarı No:123 Çankaya/ANKARA", "+90 312 555 01 01", "ankara@estetik.com.tr"),
                Branch.Create(comp.Id, "İstanbul Şube", "Bağdat Caddesi No:456 Kadıköy/İSTANBUL", "+90 216 555 02 02", "istanbul@estetik.com.tr"),
                Branch.Create(comp.Id, "İzmir Şube", "Alsancak Mahallesi No:789 Konak/İZMİR", "+90 232 555 03 03", "izmir@estetik.com.tr")
            );
            db.SaveChanges();
            Console.WriteLine("✓ Şirket ve şubeler oluşturuldu.");
        }


        // === SETTINGS ===
        if (!db.Settings.Any())
        {
            db.Settings.AddRange(
                Setting.Create("DefaultCurrency", "TRY", SettingScope.System),
                Setting.Create("DefaultLanguage", "tr", SettingScope.System),
                Setting.Create("Theme", "light", SettingScope.User),
                Setting.Create("WorkingHoursStart", "09:00", SettingScope.System),
                Setting.Create("WorkingHoursEnd", "18:00", SettingScope.System),
                Setting.Create("AppointmentDuration", "30", SettingScope.System)
            );
            db.SaveChanges();
            Console.WriteLine("✓ Sistem ayarları oluşturuldu.");
        }

        // ========================================
        // === OPERATIONAL/TRANSACTIONAL DATA ===
        // ========================================

        Console.WriteLine("\n Operasyonel veriler ekleniyor...\n");

        // Get references for operational data
        var doctor1 = db.Users.FirstOrDefault(u => u.Email == "ayse.yilmaz@clinicpro.com");
        var doctor2 = db.Users.FirstOrDefault(u => u.Email == "mehmet.demir@clinicpro.com");
        var staff1 = db.Users.FirstOrDefault(u => u.Email == "zeynep.kaya@clinicpro.com");
        var staff2 = db.Users.FirstOrDefault(u => u.Email == "ali.celik@clinicpro.com");
        var patients = db.Users.Where(u => u.UserType!.Code == "PATIENT").ToList();
    
        var firstBranch = db.Branches.First();
        

        // === USER-COMPANY & USER-BRANCH ASSIGNMENTS ===
        var company = db.Companies.First();
        var branches = db.Branches.ToList();
        var allStaff = db.Users.Where(u => u.UserType!.Code == "DOCTOR" || u.UserType!.Code == "STAFF").ToList();

        foreach (var staff in allStaff)
        {
            if (!db.UserCompanies.Any(uc => uc.UserId == staff.Id))
            {
                db.UserCompanies.Add(UserCompany.Create(staff.Id, company.Id));
            }

            if (!db.UserBranches.Any(ub => ub.UserId == staff.Id))
            {
                db.UserBranches.Add(UserBranch.Create(staff.Id, firstBranch.Id));
            }
        }
        db.SaveChanges();
        Console.WriteLine($"✓ {allStaff.Count} personel şirket/şube ataması yapıldı.");

       

        // === REFRESH TOKENS ===
        if (!db.RefreshTokens.Any())
        {
            var users = db.Users.Take(3).ToList();
            foreach (var user in users)
            {
                db.RefreshTokens.Add(
                    RefreshToken.Create(
                        user.Id,
                        "refresh_token_" + Guid.NewGuid().ToString().Substring(0, 20),
                        DateTime.UtcNow.AddDays(30),
                        null
                    )
                );
            }
            db.SaveChanges();
            Console.WriteLine($"✓ {users.Count} refresh token oluşturuldu.");
        }

        Console.WriteLine("\n========================================");
        Console.WriteLine("✅ SeedData: Tüm veriler başarıyla oluşturuldu!");
        Console.WriteLine("========================================\n");
    }
}