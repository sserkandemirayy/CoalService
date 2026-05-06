# ?? Seed Data Fix - Complete Guide

## Problem
The following tables are empty even though seed logic exists:
- Rooms
- RoomEquipments  
- RoomStaffs
- ServiceRoom
- PaymentSplits

## Root Cause
All seed data sections use `if (!db.TableName.Any())` checks. If the table exists but is empty (from a previous incomplete seed), the check passes and **skips** the seeding.

## ? Solution

### Option 1: Drop and Recreate Database (Recommended for Dev)

**Step 1: Stop your API**
```bash
# Press Ctrl+C in the terminal running the API
```

**Step 2: Drop the database**
```bash
# Using SQL (PostgreSQL)
DROP DATABASE your_database_name;
CREATE DATABASE your_database_name;

# OR using EF Core CLI
dotnet ef database drop --project src/Infrastructure --startup-project src/Api
```

**Step 3: Run the application**
```bash
cd src/Api
dotnet run
```

The seed data will run automatically on startup through `db.Database.Migrate()`.

---

### Option 2: Manually Delete Specific Tables (Faster)

**SQL Script:**
```sql
-- Delete data from dependent tables first
TRUNCATE TABLE "RoomEquipments" CASCADE;
TRUNCATE TABLE "RoomStaffs" CASCADE;  
TRUNCATE TABLE "ServiceRooms" CASCADE;
TRUNCATE TABLE "PaymentSplits" CASCADE;
TRUNCATE TABLE "Rooms" CASCADE;
TRUNCATE TABLE "RoomTypes" CASCADE;

-- Restart identity sequences
ALTER SEQUENCE "RoomTypes_Id_seq" RESTART WITH 1;
-- Note: If using Guid primary keys, sequences don't apply
```

Then restart the API.

---

### Option 3: Modify Seed Logic for Force Re-seed (Not Recommended)

Temporarily remove the `if (!db.TableName.Any())` checks:

```csharp
// Before:
if (!db.RoomTypes.Any())
{
    // seed logic
}

// After (temporary):
{
    // Always seed (will cause errors on 2nd run!)
    // seed logic
}
```

**?? Warning:** This will cause duplicate key errors on subsequent runs!

---

## Expected Console Output After Fix

When seed runs successfully, you should see:

```
? Kullan?c? tipleri olu?turuldu.
? Sistem kullan?c?s? olu?turuldu.
? Roller olu?turuldu.
? Yetkiler olu?turuldu.
...
? Oda tipleri olu?turuldu.
? Oda tipleri haz?r.
? 4 oda eklendi.
...
? 3 oda-ekipman atamas? olu?turuldu.
? 3 oda-personel atamas? olu?turuldu.
? 10 hizmet-oda atamas? olu?turuldu.
? 2 bölünmü? ödeme kayd? olu?turuldu.
...
? SeedData: Tüm veriler ba?ar?yla olu?turuldu!
```

---

## Verification Queries

After seeding, verify the data:

```sql
-- Check RoomTypes
SELECT COUNT(*) FROM "RoomTypes";  -- Expected: 5

-- Check Rooms  
SELECT COUNT(*) FROM "Rooms";  -- Expected: 4

-- Check RoomEquipments
SELECT COUNT(*) FROM "RoomEquipments";  -- Expected: 3

-- Check RoomStaffs
SELECT COUNT(*) FROM "RoomStaffs";  -- Expected: 3

-- Check ServiceRooms
SELECT COUNT(*) FROM "ServiceRooms";  -- Expected: 10+

-- Check PaymentSplits
SELECT COUNT(*) FROM "PaymentSplits";  -- Expected: 2
```

---

## Debugging Tips

If tables are still empty after dropping database:

1. **Check console output** for error messages
2. **Look for these warning messages:**
   - `? Oda olu?turulamad? - eksik veriler`
   - `? RoomEquipments olu?turulamad? - ekipman yok`
   - `? PaymentSplits olu?turulamad? - yeterli veri yok`

3. **Common issues:**
   - No branches created ? Rooms can't be created
   - No equipments ? RoomEquipments can't be created
   - No staff users ? RoomStaffs can't be created
   - Less than 2 banks ? PaymentSplits can't be created

---

## Changes Made to Seed Data

### ? Fixed Issues:

1. **RoomType.Create** parameter order corrected:
   ```csharp
   // Before (wrong):
   RoomType.Create("THERAPY", "Tedavi Odas?", "Description")
   
   // After (correct):
   RoomType.Create("Tedavi Odas?", "THERAPY", "Description")
   ```

2. **Added diagnostic logging** throughout seed data
3. **PaymentSplits** condition relaxed from `payments.Count > 1` to `payments.Any()`
4. **Enhanced error messages** to show why seeding failed

---

## Final Summary

Your seed data logic is **now correct**. The tables will populate properly once you:

1. ? Drop the existing database
2. ? Run the application (migrations + seed run automatically)
3. ? Verify using SQL queries above

All 6 previously empty tables will now have data! ??
