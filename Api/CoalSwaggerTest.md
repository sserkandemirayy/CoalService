# RTLS Core API — Demo Seed Verileri, Swagger Payloadları ve Smoke Test Planı

Bu doküman, RTLS Core API'nin seed data ile birlikte Swagger üzerinden hızlıca test edilmesi için hazırlanmıştır.

Amaç:

- sistemde yüklü demo verileri net göstermek
- hangi endpoint'e hangi payload atılacağını örneklemek
- event, tracking, alarm, command ve integration/outbox akışlarını uçtan uca test etmek
- projeyi devralan bir geliştiricinin sistemi hızlıca simüle edebilmesini sağlamak

---

# 1. Ön Koşullar

Teste başlamadan önce aşağıdakiler hazır olmalıdır:

- uygulama ayağa kalkmış olmalı
- migration'lar çalışmış olmalı
- seed data başarıyla yüklenmiş olmalı
- Swagger erişilebilir olmalı
- auth gerekiyorsa login ile token alınmış olmalı
- integration endpoint testlerinde `X-Integration-Api-Key` header değeri bilinmeli

---

# 2. Demo Seed Veri Kataloğu

## 2.1 Kullanıcılar

Aşağıdaki kullanıcılar seed ile oluşur.

| Email                                                        | Şifre     | Rol                   | UserType   | Specialization          |
| ------------------------------------------------------------ | --------- | --------------------- | ---------- | ----------------------- |
| [admin@rtls.local](mailto\:admin@rtls.local)                 | Admin123! | super\_admin          | EMPLOYEE   | SUPERVISOR              |
| [companyadmin@maden.local](mailto\:companyadmin@maden.local) | Admin123! | company\_admin        | EMPLOYEE   | SUPERVISOR              |
| [branchadmin@maden.local](mailto\:branchadmin@maden.local)   | Admin123! | branch\_admin         | EMPLOYEE   | SUPERVISOR              |
| [dispatch1@maden.local](mailto\:dispatch1@maden.local)       | 123456    | dispatch\_operator    | EMPLOYEE   | DISPATCHER              |
| [dispatch2@maden.local](mailto\:dispatch2@maden.local)       | 123456    | dispatch\_operator    | EMPLOYEE   | DISPATCHER              |
| [safety1@maden.local](mailto\:safety1@maden.local)           | 123456    | safety\_operator      | EMPLOYEE   | SAFETY\_SPECIALIST      |
| [security1@maden.local](mailto\:security1@maden.local)       | 123456    | security\_operator    | EMPLOYEE   | SECURITY\_STAFF         |
| [maintenance1@maden.local](mailto\:maintenance1@maden.local) | 123456    | maintenance\_operator | EMPLOYEE   | MAINTENANCE\_TECHNICIAN |
| [mechanic1@maden.local](mailto\:mechanic1@maden.local)       | 123456    | maintenance\_operator | EMPLOYEE   | MECHANIC                |
| [electric1@maden.local](mailto\:electric1@maden.local)       | 123456    | maintenance\_operator | EMPLOYEE   | ELECTRICIAN             |
| [supervisor1@maden.local](mailto\:supervisor1@maden.local)   | 123456    | field\_supervisor     | EMPLOYEE   | SUPERVISOR              |
| [worker1@maden.local](mailto\:worker1@maden.local)           | 123456    | viewer                | EMPLOYEE   | UNDERGROUND\_WORKER     |
| [worker2@maden.local](mailto\:worker2@maden.local)           | 123456    | viewer                | EMPLOYEE   | UNDERGROUND\_WORKER     |
| [worker3@maden.local](mailto\:worker3@maden.local)           | 123456    | viewer                | EMPLOYEE   | SURFACE\_WORKER         |
| [contractor1@maden.local](mailto\:contractor1@maden.local)   | 123456    | viewer                | CONTRACTOR | CONTRACTOR\_TECHNICIAN  |
| [contractor2@maden.local](mailto\:contractor2@maden.local)   | 123456    | viewer                | CONTRACTOR | CONTRACTOR\_OPERATOR    |
| [visitor1@maden.local](mailto\:visitor1@maden.local)         | 123456    | viewer                | VISITOR    | VISITOR\_STANDARD       |
| [auditor1@maden.local](mailto\:auditor1@maden.local)         | 123456    | viewer                | VISITOR    | AUDITOR                 |

Ayrıca sistem kullanıcısı vardır:

- `system@local`

---

## 2.2 Şirket ve Şubeler

### Company

- **Demo Maden İşletmeleri A.Ş.**

### Branch

- **Merkez Saha**
- **Yeraltı Ocak 1**
- **Zenginleştirme Tesisi**

---

## 2.3 Tag'ler

### Personel Tag'leri

- `tag-person-001` / `TAG-P-001` / Yeraltı Çalışanı - İsmail
- `tag-person-002` / `TAG-P-002` / Yeraltı Çalışanı - Kemal
- `tag-person-003` / `TAG-P-003` / Yerüstü Çalışanı - Yusuf
- `tag-person-004` / `TAG-P-004` / Yüklenici Teknisyen - Murat
- `tag-person-005` / `TAG-P-005` / Yüklenici Operatör - Cem

### Araç Tag'leri

- `tag-vehicle-001` / `TAG-V-001` / Yeraltı Araç 1
- `tag-vehicle-002` / `TAG-V-002` / Servis Aracı 2

### Asset Tag'leri

- `tag-asset-001` / `TAG-A-001` / Jeneratör 1
- `tag-asset-002` / `TAG-A-002` / Pompa 2

### Runtime Durumları

- `tag-person-001` → Online, batarya 78
- `tag-person-002` → Online, batarya 64
- `tag-person-003` → Online, batarya 92
- `tag-person-004` → Online, batarya 55
- `tag-person-005` → Online, batarya 48
- `tag-vehicle-001` → Online, batarya 81
- `tag-vehicle-002` → Offline, batarya 73
- `tag-asset-001` → Online, batarya 15
- `tag-asset-002` → Inactive

---

## 2.4 Anchor'lar

- `anchor-001` / `ANCH-001` / Yeraltı Anchor 1 / Online
- `anchor-002` / `ANCH-002` / Yeraltı Anchor 2 / Online
- `anchor-003` / `ANCH-003` / Yeraltı Anchor 3 / Online
- `anchor-004` / `ANCH-004` / Merkez Anchor 1 / Online
- `anchor-005` / `ANCH-005` / Tesis Anchor 1 / Error
- `anchor-006` / `ANCH-006` / Tesis Anchor 2 / Offline

### Runtime Durumları

- `anchor-001`, `anchor-002`, `anchor-003`, `anchor-004` → aktif heartbeat alan anchor
- `anchor-005` → Error senaryosu
- `anchor-006` → Offline senaryosu

---

## 2.5 Tag Assignment'lar

Aktif assignment örnekleri:

- `tag-person-001` → `worker1@maden.local`
- `tag-person-002` → `worker2@maden.local`
- `tag-person-003` → `worker3@maden.local`
- `tag-person-004` → `contractor1@maden.local`
- `tag-person-005` → `contractor2@maden.local`

---

## 2.6 Current Location Seed'leri

Current location seed kayıtları vardır:

- `tag-person-001`
- `tag-person-002`
- `tag-vehicle-001`

Bu kayıtlar dashboard ve tracking ekranlarını test etmek için hazırdır.

---

## 2.7 Alarm Seed'leri

Sistemde aşağıdaki örnek alarm'lar bulunur:

- **EmergencyButtonPressed** → aktif alarm
- **ProximityAlert** → acknowledged alarm
- **LowBattery** → aktif alarm
- **AnchorError** → resolved alarm

Bu sayede alarm listesi, alarm detay, acknowledge ve filtreleme testleri yapılabilir.

---

## 2.8 Event Seed'leri

Sistemde örnek event kayıtları bulunur:

- `LocationCalculated`
- `BatteryLevelReported`
- `EmergencyButtonPressed`
- `ProximityAlertRaised`
- `UWBRangingCompleted`
- `UWBTagToTagRangingCompleted`
- `BLEAdvertisementReceived`
- `TagDataReceived`
- `AnchorHeartbeatReceived`
- `AnchorStatusChanged`
- `AnchorHealthReported`

---

## 2.9 Config Snapshot Seed'leri

Aşağıdaki config / snapshot verileri seed ile doludur:

- Anchor config snapshot
- BLE config snapshot
- UWB config snapshot
- DIO config snapshot
- DIO value snapshot
- I2C config snapshot

Bu sayede config query ekranları boş gelmez.

---

## 2.10 Command Seed'leri

Sistemde command request örnekleri vardır:

- `Pending`
- `Queued`
- `Sent`
- `Succeeded`
- `Failed`

Örnek command türleri:

- `SetTagAlert`
- `SetDIOValue`
- `ResetDevice`
- `SetUWBConfig`
- `RequestConfig`

---

## 2.11 Outbox Seed'leri

Sistemde outbox mesaj örnekleri vardır:

- `Pending`
- `Dispatched`
- `Failed`

Bu sayede integration handoff akışı Swagger ile test edilebilir.

---

# 3. Genel Test Sırası

Önerilen test sırası:

1. Auth
2. Device management
3. Tracking
4. Alarm management
5. Event processing query
6. Event processing post
7. Command management
8. Integration / outbox
9. Negatif testler

---

# 4. Auth Testleri

## 4.1 Login

### Endpoint

`POST /api/Auth/login`

### Payload

```json
{
  "email": "admin@rtls.local",
  "password": "Admin123!"
}
```

### Beklenen Sonuç

- HTTP 200
- access token dönmeli
- refresh token dönmeli
- token Swagger Authorize alanına eklenebilmeli

---

# 5. Device Management Testleri

## 5.1 Tag Listesi

### Endpoint

`GET /api/DeviceManagement/tags`

### Beklenen Sonuç

- HTTP 200
- seed edilmiş tag kayıtları listelenmeli
- personel, araç ve asset tipleri görünmeli

---

## 5.2 Tekil Tag Detay

### Endpoint

`GET /api/DeviceManagement/tags/{id}`

### Beklenen Sonuç

- HTTP 200
- `ExternalId`, `Code`, `TagType`, `Status`, `BatteryLevel`, `MetadataJson` dolu gelmeli

---

## 5.3 Yeni Tag Oluşturma

### Endpoint

`POST /api/DeviceManagement/tags`

### Payload

```json
{
  "externalId": "tag-person-099",
  "code": "TAG-P-099",
  "name": "Demo Personel Tag",
  "serialNumber": "SN-TAG-P-099",
  "tagType": "Personnel",
  "metadataJson": "{\"Category\":\"Personnel\",\"Demo\":true}"
}
```

### Beklenen Sonuç

- HTTP 200
- yeni tag id dönmeli
- tag listesinde görünmeli

---

## 5.4 Tag Güncelleme

### Endpoint

`PUT /api/DeviceManagement/tags/{id}`

### Payload

```json
{
  "id": "PUT_EDILECEK_TAG_ID",
  "code": "TAG-P-099",
  "name": "Demo Personel Tag Güncel",
  "serialNumber": "SN-TAG-P-099-REV1",
  "tagType": "Personnel",
  "metadataJson": "{\"Category\":\"Personnel\",\"Demo\":true,\"Rev\":1}"
}
```

### Beklenen Sonuç

- HTTP 200
- tag detayında yeni isim ve metadata görünmeli

---

## 5.5 Tag Activate

### Endpoint

`POST /api/DeviceManagement/tags/{id}/activate`

### Beklenen Sonuç

- HTTP 200
- tag aktif olmalı

---

## 5.6 Tag Deactivate

### Endpoint

`POST /api/DeviceManagement/tags/{id}/deactivate`

### Beklenen Sonuç

- HTTP 200
- tag status uygun şekilde inactive / pasif duruma geçmeli

---

## 5.7 Anchor Listesi

### Endpoint

`GET /api/DeviceManagement/anchors`

### Beklenen Sonuç

- HTTP 200
- `anchor-001` ... `anchor-006` görünmeli

---

## 5.8 Tekil Anchor Detay

### Endpoint

`GET /api/DeviceManagement/anchors/{id}`

### Beklenen Sonuç

- HTTP 200
- ip, branch, company, status alanları dolu gelmeli

---

## 5.9 Yeni Anchor Oluşturma

### Endpoint

`POST /api/DeviceManagement/anchors`

### Payload

```json
{
  "externalId": "anchor-099",
  "code": "ANCH-099",
  "name": "Demo Anchor",
  "ipAddress": "10.10.9.99",
  "companyId": "COMPANY_ID",
  "branchId": "BRANCH_ID",
  "metadataJson": "{\"Zone\":\"Demo\",\"Demo\":true}"
}
```

### Beklenen Sonuç

- HTTP 200
- anchor oluşmalı
- anchor listesinde görünmeli

---

## 5.10 Tag'i Kullanıcıya Atama

### Endpoint

`POST /api/DeviceManagement/tags/assign`

### Payload

```json
{
  "tagId": "TAG_ID",
  "userId": "USER_ID",
  "isPrimary": true
}
```

### Beklenen Sonuç

- HTTP 200
- yeni assignment oluşmalı
- ilgili kullanıcı assignment listesinde görünmeli

---

## 5.11 Tag Unassign

### Endpoint

`POST /api/DeviceManagement/tags/{tagId}/unassign`

### Beklenen Sonuç

- HTTP 200
- aktif assignment kapanmalı

---

## 5.12 Tag Assignment Geçmişi

### Endpoint

`GET /api/DeviceManagement/tags/{tagId}/assignments`

### Beklenen Sonuç

- HTTP 200
- assignment geçmişi görünmeli

---

## 5.13 Kullanıcının Assignment Listesi

### Endpoint

`GET /api/DeviceManagement/users/{userId}/tag-assignments`

### Beklenen Sonuç

- HTTP 200
- kullanıcıya bağlı tag kayıtları görünmeli

---

# 6. Tracking Testleri

## 6.1 Current Location Listesi

### Endpoint

`GET /api/Tracking/current-locations`

### Beklenen Sonuç

- HTTP 200
- en az 3 kayıt görünmeli
- personel ve araç konumu bulunmalı

---

## 6.2 Tag Bazlı Current Location

### Endpoint

`GET /api/Tracking/current-locations/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- `X`, `Y`, `Z`, `Accuracy`, `Confidence`, `LastEventAt` dolu gelmeli

---

## 6.3 History By Tag

### Endpoint

`GET /api/Tracking/history/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- geçmiş location event noktaları görünmeli

---

## 6.4 Dashboard

### Endpoint

`GET /api/Tracking/dashboard`

### Beklenen Sonuç

- HTTP 200
- toplam tag, online tag, aktif alarm, online/offline/error anchor özetleri dönmeli

---

# 7. Alarm Management Testleri

## 7.1 Aktif Alarm Listesi

### Endpoint

`GET /api/AlarmManagement/active`

### Beklenen Sonuç

- HTTP 200
- emergency, proximity, low battery gibi alarm'lar görünmeli

---

## 7.2 Alarm Detay

### Endpoint

`GET /api/AlarmManagement/{id}`

### Beklenen Sonuç

- HTTP 200
- alarm type, severity, status, title, startedAt dolu gelmeli

---

## 7.3 Alarm Acknowledge

### Endpoint

`POST /api/AlarmManagement/{id}/acknowledge`

### Beklenen Sonuç

- HTTP 200
- alarm status `Acknowledged` olmalı

---

## 7.4 Tag Bazlı Alarm Listesi

### Endpoint

`GET /api/AlarmManagement/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- ilgili tag'e bağlı alarm'lar dönmeli

---

## 7.5 Anchor Bazlı Alarm Listesi

### Endpoint

`GET /api/AlarmManagement/by-anchor/{anchorId}`

### Beklenen Sonuç

- HTTP 200
- anchor hata alarmı görünmeli

---

# 8. Event Processing Query Testleri

## 8.1 Raw Event Listesi

### Endpoint

`GET /api/EventProcessing/raw`

### Beklenen Sonuç

- HTTP 200
- seed event kayıtları görünmeli

---

## 8.2 Tekil Raw Event

### Endpoint

`GET /api/EventProcessing/raw/{id}`

### Beklenen Sonuç

- HTTP 200
- payload ve processing status dolu gelmeli

---

## 8.3 Location Event Listesi

### Endpoint

`GET /api/EventProcessing/locations/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- location geçmişi görünmeli

---

## 8.4 Battery Event Listesi

### Endpoint

`GET /api/EventProcessing/battery/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- düşük batarya event kaydı görünmeli

---

## 8.5 IMU Event Listesi

### Endpoint

`GET /api/EventProcessing/imu/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- varsa seed veya POST sonrası oluşan IMU eventleri görünmeli

---

## 8.6 Proximity Event Listesi

### Endpoint

`GET /api/EventProcessing/proximity/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- personel-araç proximity kaydı görünmeli

---

## 8.7 Emergency Event Listesi

### Endpoint

`GET /api/EventProcessing/emergency/by-tag/{tagId}`

### Beklenen Sonuç

- HTTP 200
- emergency kayıtları görünmeli

---

## 8.8 Anchor Heartbeat Listesi

### Endpoint

`GET /api/EventProcessing/anchor-heartbeats/by-anchor/{anchorId}`

### Beklenen Sonuç

- HTTP 200
- heartbeat kayıtları görünmeli

---

## 8.9 Anchor Status Listesi

### Endpoint

`GET /api/EventProcessing/anchor-status/by-anchor/{anchorId}`

### Beklenen Sonuç

- HTTP 200
- status transition kayıtları görünmeli

---

# 9. Event Processing POST Testleri

Bu bölümde yeni event işleyip business effect üretimi test edilir.

## 9.1 LocationCalculated

### Endpoint

`POST /api/EventProcessing/location-calculated`

### Payload

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "type": "LocationCalculated",
  "timestamp": 1735689600000,
  "tagId": "tag-person-001",
  "x": 18.4,
  "y": 9.2,
  "z": 0.0,
  "accuracy": 0.7,
  "confidence": 94,
  "usedAnchors": [
    { "anchorId": "anchor-001" },
    { "anchorId": "anchor-002" },
    { "anchorId": "anchor-003" }
  ]
}
```

### Beklenen Sonuç

- HTTP 200
- location event oluşmalı
- current location güncellenmeli
- raw event listesinde görünmeli

---

## 9.2 EmergencyButtonPressed

### Endpoint

`POST /api/EventProcessing/emergency-button-pressed`

### Payload

```json
{
  "id": "22222222-2222-2222-2222-222222222222",
  "type": "EmergencyButtonPressed",
  "timestamp": 1735689601000,
  "tagId": "tag-person-001"
}
```

### Beklenen Sonuç

- HTTP 200
- emergency event oluşmalı
- aktif alarm oluşmalı

---

## 9.3 BatteryLevelReported

### Endpoint

`POST /api/EventProcessing/battery-level-reported`

### Payload

```json
{
  "id": "33333333-3333-3333-3333-333333333333",
  "type": "BatteryLevelReported",
  "timestamp": 1735689602000,
  "anchorId": "anchor-004",
  "tagId": "tag-asset-001",
  "batteryLevel": 9
}
```

### Beklenen Sonuç

- HTTP 200
- battery event oluşmalı
- low battery alarmı oluşmalı

---

## 9.4 IMUEventDetected

### Endpoint

`POST /api/EventProcessing/imu-event-detected`

### Payload

```json
{
  "id": "44444444-4444-4444-4444-444444444444",
  "type": "IMUEventDetected",
  "timestamp": 1735689603000,
  "tagId": "tag-person-002",
  "eventType": "FallDetected"
}
```

### Beklenen Sonuç

- HTTP 200
- IMU event oluşmalı
- critical alarm oluşmalı

---

## 9.5 ProximityAlertRaised

### Endpoint

`POST /api/EventProcessing/proximity-alert-raised`

### Payload

```json
{
  "id": "55555555-5555-5555-5555-555555555555",
  "type": "ProximityAlertRaised",
  "timestamp": 1735689604000,
  "tagId": "tag-person-002",
  "peerTagId": "tag-vehicle-001",
  "distance": 1.2,
  "threshold": 2.5,
  "severity": "Critical"
}
```

### Beklenen Sonuç

- HTTP 200
- proximity event oluşmalı
- proximity alarmı oluşmalı

---

## 9.6 AnchorHeartbeatReceived

### Endpoint

`POST /api/EventProcessing/anchor-heartbeat-received`

### Payload

```json
{
  "id": "66666666-6666-6666-6666-666666666666",
  "type": "AnchorHeartbeatReceived",
  "timestamp": 1735689605000,
  "anchorId": "anchor-001",
  "ipAddress": "10.10.1.11"
}
```

### Beklenen Sonuç

- HTTP 200
- heartbeat event oluşmalı
- anchor last heartbeat güncellenmeli

---

## 9.7 AnchorStatusChanged

### Endpoint

`POST /api/EventProcessing/anchor-status-changed`

### Payload

```json
{
  "id": "77777777-7777-7777-7777-777777777777",
  "type": "AnchorStatusChanged",
  "timestamp": 1735689606000,
  "anchorId": "anchor-006",
  "status": "Offline",
  "previousStatus": "Online",
  "reason": "Heartbeat timeout"
}
```

### Beklenen Sonuç

- HTTP 200
- anchor status event oluşmalı
- offline alarmı oluşmalı

---

# 10. Command Management Testleri

## 10.1 Command Listesi

### Endpoint

`GET /api/CommandManagement`

### Beklenen Sonuç

- HTTP 200
- pending, queued, sent, succeeded, failed kayıtları görünmeli

---

## 10.2 Command Detay

### Endpoint

`GET /api/CommandManagement/{id}`

### Beklenen Sonuç

- HTTP 200
- type, target, payload, status görünmeli

---

## 10.3 Command History

### Endpoint

`GET /api/CommandManagement/{id}/history`

### Beklenen Sonuç

- HTTP 200
- state transition kayıtları görünmeli

---

## 10.4 SetTagAlert Command Oluşturma

### Endpoint

`POST /api/CommandManagement/set-tag-alert`

### Payload

```json
{
  "tagId": "tag-person-001",
  "buzzerEnabled": true,
  "vibrationEnabled": true,
  "ledEnabled": true,
  "ledColor": "Red"
}
```

### Beklenen Sonuç

- HTTP 200
- yeni command oluşmalı
- başlangıç status `Pending` olmalı

---

## 10.5 SetDIOValue Command Oluşturma

### Endpoint

`POST /api/CommandManagement/set-dio-value`

### Payload

```json
{
  "tagId": "tag-person-001",
  "pin": 1,
  "value": true
}
```

### Beklenen Sonuç

- HTTP 200
- yeni command oluşmalı

---

## 10.6 ResetDevice Command Oluşturma

### Endpoint

`POST /api/CommandManagement/reset-device`

### Payload

```json
{
  "anchorId": "anchor-005"
}
```

### Beklenen Sonuç

- HTTP 200
- anchor targetlı command oluşmalı

---

## 10.7 Command Queue

### Endpoint

`POST /api/CommandManagement/{id}/queue`

### Beklenen Sonuç

- HTTP 200
- status `Queued` olmalı
- history kaydı oluşmalı
- outbox kaydı oluşmalı

---

## 10.8 Command Cancel

### Endpoint

`POST /api/CommandManagement/{id}/cancel`

### Beklenen Sonuç

- HTTP 200
- status `Cancelled` olmalı

---

## 10.9 Command Retry

### Endpoint

`POST /api/CommandManagement/{id}/retry`

### Beklenen Sonuç

- HTTP 200
- status tekrar `Pending` olmalı

---

# 11. Integration / Outbox Testleri

Bu endpointlerde auth yerine servisler arası header kullanılır.

### Header

```http
X-Integration-Api-Key: YOUR_INTEGRATION_API_KEY
```

## 11.1 Pending Outbox Listesi

### Endpoint

`GET /api/CommandManagement/integration/outbox/pending?take=10`

### Beklenen Sonuç

- HTTP 200
- pending outbox kayıtları dönmeli

---

## 11.2 Outbox Mark Dispatched

### Endpoint

`POST /api/CommandManagement/integration/outbox/{id}/mark-dispatched`

### Beklenen Sonuç

- HTTP 200
- outbox status `Dispatched` olmalı

---

## 11.3 Outbox Mark Failed

### Endpoint

`POST /api/CommandManagement/integration/outbox/{id}/mark-failed`

### Payload

```json
{
  "failureReason": "Integration network timeout"
}
```

### Beklenen Sonuç

- HTTP 200
- outbox status `Failed` olmalı

---

## 11.4 Command Mark Sent

### Endpoint

`POST /api/CommandManagement/integration/commands/{id}/mark-sent`

### Payload

```json
{
  "externalCorrelationId": "vendor-cmd-1001",
  "note": "Sent to RTLS vendor"
}
```

### Beklenen Sonuç

- HTTP 200
- command status `Sent` olmalı

---

## 11.5 Command Mark Succeeded

### Endpoint

`POST /api/CommandManagement/integration/commands/{id}/mark-succeeded`

### Payload

```json
{
  "externalCorrelationId": "vendor-cmd-1001",
  "responseJson": "{\"success\":true,\"message\":\"Accepted\"}",
  "note": "Vendor confirmed command"
}
```

### Beklenen Sonuç

- HTTP 200
- command status `Succeeded` olmalı

---

## 11.6 Command Mark Failed

### Endpoint

`POST /api/CommandManagement/integration/commands/{id}/mark-failed`

### Payload

```json
{
  "externalCorrelationId": "vendor-cmd-1002",
  "failureReason": "Device offline",
  "responseJson": "{\"success\":false,\"message\":\"offline\"}",
  "note": "Vendor failed command"
}
```

### Beklenen Sonuç

- HTTP 200
- command status `Failed` olmalı

---

# 12. Negatif Testler

## 12.1 Duplicate Raw Event

Önceden kullanılmış aynı `id` ile tekrar event gönder.

### Beklenen Sonuç

- HTTP 400
- duplicate raw event hatası

---

## 12.2 Olmayan Tag ile Event

### Endpoint

`POST /api/EventProcessing/emergency-button-pressed`

### Payload

```json
{
  "id": "88888888-8888-8888-8888-888888888888",
  "type": "EmergencyButtonPressed",
  "timestamp": 1735689607000,
  "tagId": "tag-not-found"
}
```

### Beklenen Sonuç

- HTTP 400
- `Tag not found`

---

## 12.3 Olmayan Anchor ile Event

### Endpoint

`POST /api/EventProcessing/anchor-heartbeat-received`

### Payload

```json
{
  "id": "99999999-9999-9999-9999-999999999999",
  "type": "AnchorHeartbeatReceived",
  "timestamp": 1735689608000,
  "anchorId": "anchor-not-found",
  "ipAddress": "10.10.9.99"
}
```

### Beklenen Sonuç

- HTTP 400
- `Anchor not found`

---

## 12.4 Geçersiz Integration API Key

### Beklenen Sonuç

- HTTP 401
- invalid or missing api key

---

## 12.5 Geçersiz Enum Değeri

Örneğin proximity severity için geçersiz değer gönder.

### Beklenen Sonuç

- HTTP 400
- parse / validation hatası

---

# 13. 25 Maddelik Hızlı Smoke Test Planı

1. Login ol
2. Tag listesini aç
3. Tekil tag detayını aç
4. Anchor listesini aç
5. Tekil anchor detayını aç
6. Tag assignment listesini aç
7. Current location listesini aç
8. Tag location history aç
9. Dashboard özetini aç
10. Aktif alarm listesini aç
11. Alarm detayını aç
12. Bir alarmı acknowledge et
13. Raw event listesini aç
14. `LocationCalculated` POST et
15. Current location güncellendi mi kontrol et
16. `EmergencyButtonPressed` POST et
17. Alarm oluştu mu kontrol et
18. `BatteryLevelReported` POST et
19. Low battery alarmını kontrol et
20. `ProximityAlertRaised` POST et
21. Proximity alarmını kontrol et
22. Yeni command oluştur
23. Command queue et
24. Pending outbox listesini kontrol et
25. Integration mark-sent ve mark-succeeded çağrılarıyla command lifecycle'ı tamamla

---

# 14. Test Başarı Kriterleri

Test başarılı sayılır eğer:

- seed verileri doğru görünüyorsa
- GET endpointleri veri döndürüyorsa
- event POST endpointleri doğru entity ve business effect üretiyorsa
- tracking state güncelleniyorsa
- alarm state yönetimi çalışıyorsa
- command lifecycle çalışıyorsa
- outbox lifecycle çalışıyorsa
- integration callback endpointleri çalışıyorsa
- negatif testler beklenen hatayı dönüyorsa

---

# 15. Kısa Demo Senaryo Önerileri

Bu seed ile aşağıdaki demo senaryoları kolayca anlatılabilir:

## Senaryo 1 — Personel Takibi

- `tag-person-001` ve `tag-person-002` üzerinden tracking ekranı gösterilir
- current location ve history endpointleri test edilir

## Senaryo 2 — Araç-Personel Yakınlık Uyarısı

- `tag-person-002` ile `tag-vehicle-001` proximity örneği gösterilir
- proximity event ve alarm akışı anlatılır

## Senaryo 3 — Acil Durum Butonu

- `tag-person-001` için emergency event atılır
- alarm üretimi ve alarm yönetimi gösterilir

## Senaryo 4 — Düşük Batarya

- `tag-asset-001` için battery event ile kritik seviye gösterilir
- low battery alarmı gözlemlenir

## Senaryo 5 — Anchor Sağlığı

- `anchor-001` heartbeat/health
- `anchor-005` error
- `anchor-006` offline senaryoları ile cihaz sağlık ekranları test edilir

## Senaryo 6 — Command Lifecycle

- command oluştur
- queue et
- outbox'a düşmesini izle
- integration callback ile sent/succeeded/failure akışını göster

---

# 16. Not

Bu doküman canlı öncesi kalite kontrol, demo hazırlığı ve geliştirici onboarding süreçlerinde kullanılabilir.

Ek geliştirme yapılırsa aşağıdaki başlıklar ayrıca genişletilmelidir:

- endpoint bazlı permission matrisi
- response schema örnekleri
- error code kataloğu
- integration servis akış diyagramı
- event type / command type referans tablosu

