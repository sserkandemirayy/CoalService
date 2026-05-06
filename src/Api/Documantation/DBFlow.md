
# Genel işlem mantığı

Her hardware event akışı temelde şöyle çalışır:

```text
Hardware / Integration Service
→ Core API endpoint
→ RawEvents
→ ilgili event tablosu
→ gerekiyorsa snapshot / current state update
→ gerekiyorsa alarm
```

Komut akışı ise şöyle:

```text
Kullanıcı / Core API
→ CommandRequests
→ CommandStatusHistories
→ OutboxMessages
→ Integration Service
→ Hardware
```

---

# Sistem / Organizasyon / Kullanıcı tabloları

## `Companies`

Şirket bilgisini tutar.
Ne zaman yazılır: sistem kurulurken seed ile veya admin yeni şirket açınca.

## `Branches`

Şirketin saha, ocak, tesis gibi şubelerini tutar.
Ne zaman yazılır: şirket altında lokasyon/şube tanımlanınca.

## `Users`

Sisteme giriş yapan kişileri tutar.
Ne zaman yazılır: kullanıcı oluşturulunca. Tag ataması yapılacak personel de burada olur.

## `UserTypes`

Kullanıcı tipini tutar. Örnek: çalışan, yüklenici, ziyaretçi.
Ne zaman yazılır: seed sırasında, genelde sabit veridir.

## `UserSpecializations`

Kullanıcının görev/kimlik detayını tutar. Örnek: yeraltı çalışanı, İSG uzmanı, bakım teknisyeni.
Ne zaman yazılır: seed sırasında veya admin yeni uzmanlık ekleyince.

## `Roles`

Sistemdeki rolleri tutar. Örnek: super_admin, safety_operator, viewer.
Ne zaman yazılır: seed sırasında.

## `Permissions`

Sistemdeki yetkileri tutar. Örnek: view_tracking, manage_tags, create_commands.
Ne zaman yazılır: seed sırasında.

## `RolePermissions`

Hangi rolün hangi yetkiye sahip olduğunu tutar.
Ne zaman yazılır: seed sırasında veya rol yetkisi değişince.

## `UserRoles`

Kullanıcının rollerini tutar.
Ne zaman yazılır: kullanıcıya rol verilince.

## `UserCompanies`

Kullanıcının hangi şirkete bağlı olduğunu tutar.
Ne zaman yazılır: kullanıcı şirkete atanırken.

## `UserBranches`

Kullanıcının hangi şube/sahaya bağlı olduğunu tutar.
Ne zaman yazılır: kullanıcı şubeye atanırken.

## `RefreshTokens`

Login sonrası refresh token kayıtlarını tutar.
Ne zaman yazılır: kullanıcı login olunca veya token yenilenince.

## `Settings`

Sistem veya kullanıcı ayarlarını tutar.
Ne zaman yazılır: seed sırasında veya ayar değiştirildiğinde.

## `AuditLogs`

Sistemde yapılan önemli işlemlerin logudur.
Ne zaman yazılır: kullanıcı oluşturma, güncelleme, silme, PII görüntüleme, kritik işlem gibi durumlarda.

## `__EFMigrationsHistory`

Entity Framework migration geçmişini tutar.
Ne zaman yazılır: migration çalıştırılınca EF otomatik yazar.

---

# Cihaz ana tabloları

## `Tags`

Takip edilen cihazları tutar. Personel tag’i, araç tag’i, asset tag’i olabilir.
Ne zaman yazılır: sisteme yeni tag tanımlanınca.
Ne zaman güncellenir: event geldikçe `LastSeenAt`, `LastEventAt`, `BatteryLevel`, `Status` değişebilir.

## `Anchors`

Sabit okuyucu cihazları tutar. Anchor, tag’lerden veri alır.
Ne zaman yazılır: sisteme yeni anchor tanımlanınca.
Ne zaman güncellenir: heartbeat, status, BLE veya config eventleri geldikçe.

## `TagAssignments`

Bir tag’in hangi kullanıcıya/personel’e atandığını tutar.
Ne zaman yazılır: tag bir kullanıcıya verildiğinde.
Ne zaman güncellenir: tag kullanıcıdan geri alındığında `UnassignedAt` dolar.

---

# Raw event tablosu

## `RawEvents`

Core API’ye gelen her hardware event’in ham halini tutar.
Ne zaman yazılır: event endpoint’lerinden herhangi biri çağrıldığı anda önce buraya yazılır.
Sonra ne olur: event başarılı işlenirse `ProcessingStatus = Processed`, hata olursa `Failed` olur.

Örnek:

```text
LocationCalculated geldi
→ RawEvents insert
→ LocationEvents insert
→ CurrentLocations update
```

---

# Konum / tracking tabloları

## `LocationEvents`

Tag için hesaplanmış konum geçmişini tutar.
Ne zaman yazılır: `LocationCalculated` event geldiğinde.
Sonra ne olur: `CurrentLocations` tablosu güncellenir.

## `CurrentLocations`

Her tag’in en son bilinen konumunu tutar.
Ne zaman yazılır: ilk kez `LocationCalculated` geldiğinde.
Ne zaman güncellenir: aynı tag için yeni konum geldikçe update edilir.

---

# Telemetry / event tabloları

## `BleAdvertisementEvents`

Anchor’ın bir tag’den BLE reklam sinyali aldığını tutar.
Ne zaman yazılır: `BLEAdvertisementReceived` event geldiğinde.
Sonra ne olur: tag görüldü sayılır, anchor canlı sayılabilir.

## `BatteryEvents`

Tag batarya bilgisinin geçmişini tutar.
Ne zaman yazılır: `BatteryLevelReported` event geldiğinde.
Sonra ne olur: `Tags.BatteryLevel` güncellenir. Batarya düşükse alarm oluşabilir.

## `EmergencyEvents`

Tag üzerindeki acil durum butonuna basılmasını tutar.
Ne zaman yazılır: `EmergencyButtonPressed` event geldiğinde.
Sonra ne olur: kritik alarm oluşturulur.

## `ImuEvents`

IMU sensör olaylarını tutar. Örnek: düşme, hareketsizlik, darbe.
Ne zaman yazılır: `IMUEventDetected` event geldiğinde.
Sonra ne olur: olay tipine göre alarm oluşabilir.

## `ProximityEvents`

İki tag’in birbirine fazla yaklaşmasını tutar.
Ne zaman yazılır: `ProximityAlertRaised` event geldiğinde.
Sonra ne olur: proximity alarm oluşur.

## `UwbRangingEvents`

Anchor ile tag arasındaki UWB mesafe ölçümünü tutar.
Ne zaman yazılır: `UWBRangingCompleted` event geldiğinde.

## `UwbTagToTagRangingEvents`

İki tag arasındaki doğrudan UWB mesafe ölçümünü tutar.
Ne zaman yazılır: `UWBTagToTagRangingCompleted` event geldiğinde.

## `TagDataEvents`

Anchor’ın tag’den genel veri aldığını tutar.
Ne zaman yazılır: `TagDataReceived` event geldiğinde.
Genelde ham/genel takip datası için kullanılır.

## `DioValueEvents`

Tag üzerindeki dijital pin değerinin geçmişini tutar.
Ne zaman yazılır: `DIOValueReported` event geldiğinde.
Sonra ne olur: `TagDioValueSnapshots` güncellenir.

## `I2cDataEvents`

Tag’in I2C cihazından okuduğu veya yazdığı veriyi tutar.
Ne zaman yazılır: `I2CDataReceived` event geldiğinde.

---

# Anchor event tabloları

## `AnchorHeartbeatEvents`

Anchor’ın canlılık sinyallerini tutar.
Ne zaman yazılır: `AnchorHeartbeatReceived` event geldiğinde.
Sonra ne olur: `Anchors.LastHeartbeatAt` güncellenir.

## `AnchorStatusEvents`

Anchor durum değişim geçmişini tutar.
Ne zaman yazılır: `AnchorStatusChanged` event geldiğinde.
Sonra ne olur: `Anchors.Status` güncellenir. Offline/Error ise alarm oluşabilir.

## `AnchorHealthEvents`

Anchor sağlık/performance verilerini tutar. CPU, memory, temperature gibi.
Ne zaman yazılır: `AnchorHealthReported` event geldiğinde.
Kullanım amacı: bakım, izleme, sorun tespiti.

---

# Config event tabloları

Bu tablolar cihazların raporladığı konfigürasyon geçmişidir. Yani “cihaz o anda ayarlarını bildirdi” kayıtlarıdır.

## `AnchorConfigEvents`

Anchor config geçmişini tutar.
Ne zaman yazılır: `AnchorConfigReported` event geldiğinde.
Sonra ne olur: `AnchorConfigSnapshots` güncellenir.

## `BleConfigEvents`

Tag BLE config geçmişini tutar.
Ne zaman yazılır: `BLEConfigReported` event geldiğinde.
Sonra ne olur: `TagBleConfigSnapshots` güncellenir.

## `UwbConfigEvents`

Tag UWB config geçmişini tutar.
Ne zaman yazılır: `UWBConfigReported` event geldiğinde.
Sonra ne olur: `TagUwbConfigSnapshots` güncellenir.

## `DioConfigEvents`

Tag dijital pin config geçmişini tutar.
Ne zaman yazılır: `DIOConfigReported` event geldiğinde.
Sonra ne olur: `TagDioConfigSnapshots` güncellenir.

## `I2cConfigEvents`

Tag I2C config geçmişini tutar.
Ne zaman yazılır: `I2CConfigReported` event geldiğinde.
Sonra ne olur: `TagI2cConfigSnapshots` güncellenir.

---

# Snapshot tabloları

Snapshot tabloları geçmiş değil, **son durumu** tutar. Aynı tag/anchor için genelde tek kayıt olur.

## `AnchorConfigSnapshots`

Anchor’ın en güncel config bilgisini tutar.
Ne zaman yazılır: ilk `AnchorConfigReported` geldiğinde insert.
Ne zaman güncellenir: aynı anchor’dan yeni config geldikçe update.

## `TagBleConfigSnapshots`

Tag’in en güncel BLE ayarını tutar.
Ne zaman yazılır/güncellenir: `BLEConfigReported` geldiğinde.

## `TagUwbConfigSnapshots`

Tag’in en güncel UWB ayarını tutar.
Ne zaman yazılır/güncellenir: `UWBConfigReported` geldiğinde.

## `TagDioConfigSnapshots`

Tag’in pin bazlı en güncel DIO config bilgisini tutar.
Ne zaman yazılır/güncellenir: `DIOConfigReported` geldiğinde.
Not: Tag + Pin bazında tek kayıt mantığı vardır.

## `TagDioValueSnapshots`

Tag’in pin bazlı en son DIO değerini tutar.
Ne zaman yazılır/güncellenir: `DIOValueReported` geldiğinde.
Örnek: pin 1 şu an true/false.

## `TagI2cConfigSnapshots`

Tag’in en güncel I2C ayarını tutar.
Ne zaman yazılır/güncellenir: `I2CConfigReported` geldiğinde.

---

# Alarm tablosu

## `Alarms`

Sistemsel risk/uyarı kayıtlarını tutar.
Ne zaman yazılır:

* emergency button basılırsa
* proximity critical/warning oluşursa
* battery düşükse
* anchor offline/error olursa
* IMU fall/shock gibi olay oluşursa

Ne zaman güncellenir:

* kullanıcı alarmı acknowledge edince
* alarm resolved/closed olunca.

---

# Command tabloları

## `CommandRequests`

Core API’den hardware’e gönderilecek komutları tutar.
Ne zaman yazılır: kullanıcı bir komut oluşturduğunda.
Örnek komutlar: reset device, set UWB config, set DIO value, set tag alert.

Durum akışı:

```text
Pending → Queued → Sent → Succeeded / Failed / Cancelled
```

## `CommandStatusHistories`

Command status değişim geçmişini tutar.
Ne zaman yazılır: command durumu her değiştiğinde.
Örnek: Pending’den Queued’a geçti, Sent oldu, Failed oldu.

## `OutboxMessages`

Integration service’e iletilecek işleri tutar.
Ne zaman yazılır: command queue edildiğinde veya dış servise gönderilecek bir mesaj oluştuğunda.
Sonra ne olur: Integration service bunu okur, hardware API’ye gönderir, sonra status `Dispatched` veya `Failed` olur.

---

# İşlem sıraları

## 1. LocationCalculated geldiğinde

```text
1. Core API event’i alır
2. RawEvents insert
3. Tag bulunur
4. LocationEvents insert
5. CurrentLocations insert/update
6. Tags.LastSeenAt ve LastEventAt update
7. RawEvents.Processed
```

Yazılan/güncellenen tablolar:

```text
RawEvents
LocationEvents
CurrentLocations
Tags
```

---

## 2. BLEAdvertisementReceived geldiğinde

```text
1. RawEvents insert
2. Anchor bulunur
3. Tag bulunur
4. BleAdvertisementEvents insert
5. Tag görüldü diye update edilir
6. Anchor heartbeat gibi update edilir
7. RawEvents.Processed
```

Tablolar:

```text
RawEvents
BleAdvertisementEvents
Tags
Anchors
```

---

## 3. BatteryLevelReported geldiğinde

```text
1. RawEvents insert
2. Tag bulunur
3. Anchor bulunur
4. BatteryEvents insert
5. Tags.BatteryLevel update
6. Batarya düşükse Alarms insert
7. RawEvents.Processed
```

Tablolar:

```text
RawEvents
BatteryEvents
Tags
Alarms
```

---

## 4. EmergencyButtonPressed geldiğinde

```text
1. RawEvents insert
2. Tag bulunur
3. EmergencyEvents insert
4. Tag atanmış kullanıcı bulunur
5. Critical Alarm insert
6. Tags.LastSeenAt update
7. RawEvents.Processed
```

Tablolar:

```text
RawEvents
EmergencyEvents
Tags
TagAssignments
Alarms
```

---

## 5. ProximityAlertRaised geldiğinde

```text
1. RawEvents insert
2. Tag bulunur
3. PeerTag bulunur
4. ProximityEvents insert
5. Alarm insert
6. İki tag de görüldü diye update edilir
7. RawEvents.Processed
```

Tablolar:

```text
RawEvents
ProximityEvents
Tags
Alarms
```

---

## 6. IMUEventDetected geldiğinde

```text
1. RawEvents insert
2. Tag bulunur
3. ImuEvents insert
4. EventType’a göre alarm insert
5. Tags.LastSeenAt update
6. RawEvents.Processed
```

Tablolar:

```text
RawEvents
ImuEvents
Tags
Alarms
```

---

## 7. AnchorHeartbeatReceived geldiğinde

```text
1. RawEvents insert
2. Anchor bulunur
3. AnchorHeartbeatEvents insert
4. Anchors.LastHeartbeatAt update
5. Anchors.Status Online olabilir
6. RawEvents.Processed
```

Tablolar:

```text
RawEvents
AnchorHeartbeatEvents
Anchors
```

---

## 8. AnchorStatusChanged geldiğinde

```text
1. RawEvents insert
2. Anchor bulunur
3. AnchorStatusEvents insert
4. Anchors.Status update
5. Status Offline/Error ise Alarm insert
6. RawEvents.Processed
```

Tablolar:

```text
RawEvents
AnchorStatusEvents
Anchors
Alarms
```

---

## 9. AnchorHealthReported geldiğinde

```text
1. RawEvents insert
2. Anchor bulunur
3. AnchorHealthEvents insert
4. RawEvents.Processed
```

Tablolar:

```text
RawEvents
AnchorHealthEvents
Anchors
```

---

## 10. ConfigReported geldiğinde

Örnek: `UWBConfigReported`

```text
1. RawEvents insert
2. Tag veya Anchor bulunur
3. İlgili ConfigEvents tablosuna insert
4. İlgili Snapshot tablosuna insert/update
5. RawEvents.Processed
```

Tablolar:

```text
RawEvents
UwbConfigEvents / BleConfigEvents / DioConfigEvents / I2cConfigEvents / AnchorConfigEvents
TagUwbConfigSnapshots / TagBleConfigSnapshots / TagDioConfigSnapshots / TagI2cConfigSnapshots / AnchorConfigSnapshots
```

---

## 11. DIOValueReported geldiğinde

```text
1. RawEvents insert
2. Tag bulunur
3. DioValueEvents insert
4. TagDioValueSnapshots insert/update
5. Tags.LastSeenAt update
6. RawEvents.Processed
```

Tablolar:

```text
RawEvents
DioValueEvents
TagDioValueSnapshots
Tags
```

---

## 12. I2CDataReceived geldiğinde

```text
1. RawEvents insert
2. Tag bulunur
3. I2cDataEvents insert
4. Tags.LastSeenAt update
5. RawEvents.Processed
```

Tablolar:

```text
RawEvents
I2cDataEvents
Tags
```

---

## 13. Kullanıcı command oluşturduğunda

```text
1. Kullanıcı Core API’ye command request atar
2. CommandRequests insert edilir
3. Status Pending olur
```

Tablolar:

```text
CommandRequests
```

---

## 14. Command queue edildiğinde

```text
1. CommandRequests.Status Queued olur
2. CommandStatusHistories insert
3. OutboxMessages insert
4. Integration service bunu alır
```

Tablolar:

```text
CommandRequests
CommandStatusHistories
OutboxMessages
```

---

## 15. Integration service command’i hardware’e gönderdiğinde

```text
1. CommandRequests.Status Sent olur
2. ExternalCorrelationId yazılır
3. CommandStatusHistories insert
4. OutboxMessages Dispatched yapılabilir
```

Tablolar:

```text
CommandRequests
CommandStatusHistories
OutboxMessages
```

---

## 16. Hardware command sonucu döndüğünde

Başarılıysa:

```text
CommandRequests.Status = Succeeded
CommandStatusHistories insert
ResponseJson yazılır
```

Hatalıysa:

```text
CommandRequests.Status = Failed
CommandStatusHistories insert
FailureReason yazılır
```

Tablolar:

```text
CommandRequests
CommandStatusHistories
```

---

# En kısa özet

```text
RawEvents = dışarıdan gelen ham kayıt
Event tabloları = geçmiş
Snapshot tabloları = son durum
Tags / Anchors = cihaz kartı
CurrentLocations = son konum
Alarms = iş kuralı sonucu oluşan uyarılar
CommandRequests = cihaza gönderilecek istek
CommandStatusHistories = komut geçmişi
OutboxMessages = integration service’in alacağı işler
```
