# Coal RTLS Platform

Coal RTLS Platform is a real-time location tracking and industrial telemetry platform developed for underground mining and industrial environments.

The system processes telemetry data coming from RTLS hardware devices, manages device configurations, tracks personnel and assets in real time, and provides a centralized API layer for operational monitoring.

The project is built with a clean and modular architecture focused on scalability, maintainability and realtime event processing.

---

# What This Platform Does

The platform receives events from RTLS hardware infrastructure and converts them into meaningful operational data.

Main capabilities:

- Personnel tracking
- Vehicle tracking
- Asset monitoring
- Emergency event handling
- Proximity safety alerts
- Device health monitoring
- Device configuration management
- Historical movement tracking
- Command management for RTLS devices
- Realtime dashboard integration

---

# General Architecture

```text
RTLS Devices
     │
     ▼
Hardware Gateway / Vendor API
     │
     ▼
Integration Service
     │
     ▼
Coal Core API
     │
     ├── Event Processing
     ├── Alarm Engine
     ├── Tracking Engine
     ├── Config Snapshot Engine
     ├── Command Management
     └── SignalR Hub
     │
     ▼
PostgreSQL
```

---

# Main Features

## Realtime Tracking

- Live personnel location tracking
- Vehicle tracking
- Asset tracking
- Current location projection
- Historical movement history

---

## Event Processing

The platform supports all telemetry and device events provided by the RTLS hardware layer.

Supported events include:

- LocationCalculated
- BatteryLevelReported
- EmergencyButtonPressed
- ProximityAlertRaised
- IMUEventDetected
- AnchorHeartbeatReceived
- AnchorStatusChanged
- BLEAdvertisementReceived
- DIOValueReported
- I2CDataReceived
- UWB ranging events
- Configuration snapshot events

Each incoming event is persisted as immutable raw data and then projected into domain-specific tables.

---

## Device Configuration Management

The system stores latest known configuration snapshots for devices.

Supported snapshot types:

- BLE configuration
- UWB configuration
- DIO configuration
- I2C configuration
- Anchor configuration

This allows the system to always know the latest configuration state of each hardware device.

---

## Command Management

The platform includes a command management layer for sending actions to RTLS devices.

Supported command types:

- RequestConfig
- ResetDevice
- SetBLEConfig
- SetDIOConfig
- SetDIOValue
- SetI2CConfig
- SetUWBConfig
- SetTagAlert
- WriteI2CData

Command lifecycle is fully tracked through database state transitions.

---

## Alarm Management

The system automatically generates operational alarms for critical situations.

Examples:

- Emergency button events
- Proximity violations
- Low battery alerts
- Offline anchors
- Device communication problems

---

# Technology Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 9 |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Architecture | Clean Architecture |
| Application Pattern | CQRS |
| Mediator | MediatR |
| Validation | FluentValidation |
| Authentication | JWT |
| Realtime | SignalR |
| Documentation | Swagger / OpenAPI |

---


# Event Processing Flow

```text
RTLS Device
   ↓
Hardware API
   ↓
Integration Service
   ↓
EventProcessingController
   ↓
RawEvent
   ↓
Domain Event Tables
   ↓
Current State Projection
   ↓
SignalR Notifications
```

---

# Database Design

The platform uses an event-driven persistence model.

Important concepts:

| Table | Purpose |
|---|---|
| RawEvents | Stores original incoming hardware payloads |
| LocationEvents | Historical location calculations |
| CurrentLocations | Latest known location projection |
| BatteryEvents | Battery telemetry history |
| AlarmEvents | Operational alarms |
| CommandRequests | Device command queue |
| OutboxMessages | Integration delivery queue |
| ConfigSnapshots | Latest known device configurations |

The database keeps both immutable historical telemetry and realtime projected state.

---

# Demo Seed Data

The project includes realistic demo data for development and testing.

Included demo data:

- Personnel tags
- Vehicle tags
- Asset tags
- Underground anchors
- Alarm scenarios
- Historical telemetry
- Device configuration snapshots
- Command history

This allows the API to be tested directly from Swagger or Postman after startup.

---

# API Documentation

Swagger endpoint:

```text
https://localhost:53311/swagger
```

A Postman collection and environment file are also included in the repository.

---

# Realtime Communication

SignalR is used for realtime communication between backend services and frontend dashboards.

Realtime updates include:

- Location updates
- Alarm updates
- Device status changes
- Command state changes
- Telemetry notifications

---

# Maps Modülü Kullanımı

Maps modülü; harita kayıtlarını, harita dosyalarını, anchor konumlarını, zone alanlarını ve RTLS koordinat dönüşümünü yönetir.

## Genel Mantık

```
RTLS sistemi X,Y,Z koordinatı üretir
        │
        ▼
Maps modülü calibration bilgisine göre koordinatı dönüştürür
        │
        ▼
Frontend tag veya anchor'ı harita üzerinde doğru noktaya çizer
```

---

# 1. Map

Map, sistemdeki fiziksel alanın harita kaydıdır.

Örnekler;

- Yeraltı Kat Planı
- Tesis Haritası
- Ana Saha Haritası

## Map Oluşturma

**POST** `/api/Maps`

### Request

```json
{
  "code": "MAP-MINE-001",
  "name": "Yeraltı Haritası",
  "description": "Yeraltı ocak haritası",
  "companyId": null,
  "branchId": null,
  "width": 100,
  "height": 100,
  "unit": "meter"
}
```

### Response

```json
{
  "id": "92cbc4b4-b214-43ac-b0ef-826284e30428"
}
```

> Map oluşturulduğunda sistem otomatik olarak **Default Calibration** oluşturur.

---

# 2. Map Dosyaları

Haritaya ait farklı dosya tipleri sisteme yüklenebilir.

| Dosya Tipi | Açıklama |
|------------|----------|
| OriginalDwg | Orijinal AutoCAD DWG |
| OriginalDxf | DXF dosyası |
| Glb | 3D model |
| Svg | 2D vektörel harita |
| PreviewImage | PNG/JPG önizleme resmi |
| Other | Diğer dosyalar |

## Dosya Yükleme

**POST** `/api/Maps/{mapId}/files`

Form-Data

| Alan | Değer |
|------|-------|
| file | harita.glb |
| fileType | Glb |
| version | 1 |

### Response

```json
{
    "id":"file-guid"
}
```

## Dosyaları Listeleme

**GET** `/api/Maps/{mapId}/files`

---

# 3. Calibration

Calibration, RTLS koordinatının harita koordinatına nasıl çevrileceğini belirler.

Yani sistem şunu öğrenir;

> "RTLS'ten gelen koordinatı haritada nereye çizeceğim?"

Map oluşturulunca otomatik olarak aşağıdaki değerler oluşturulur.

```json
{
  "sourceOriginX":0,
  "sourceOriginY":0,
  "sourceOriginZ":0,
  "mapOriginX":0,
  "mapOriginY":0,
  "mapOriginZ":0,
  "scaleX":1,
  "scaleY":1,
  "scaleZ":1,
  "rotationDegrees":0
}
```

Bu ayarlar şu anlama gelir;

```
RTLS X = Map X
RTLS Y = Map Y
RTLS Z = Map Z
```

Yani herhangi bir dönüşüm yapılmaz.

## Calibration Listeleme

**GET** `/api/Maps/{mapId}/calibrations`

## Calibration Oluşturma

**POST** `/api/Maps/{mapId}/calibrations`

### Request

```json
{
  "name":"Main Calibration",
  "sourceOriginX":0,
  "sourceOriginY":0,
  "sourceOriginZ":0,
  "mapOriginX":100,
  "mapOriginY":50,
  "mapOriginZ":0,
  "scaleX":10,
  "scaleY":10,
  "scaleZ":1,
  "rotationDegrees":0,
  "isDefault":true,
  "isActive":true
}
```

Bu örnekte;

- RTLS'deki 1 metre haritada 10 birim olur.
- Haritanın başlangıç noktası (100,50) kabul edilir.

---

# 4. Transform

Transform endpoint'i RTLS koordinatını harita koordinatına çevirir.

**POST** `/api/Maps/{mapId}/transform`

### Request

```json
{
  "x":25,
  "y":40,
  "z":0
}
```

### Response

```json
{
  "sourceX":25,
  "sourceY":40,
  "sourceZ":0,
  "mapX":350,
  "mapY":450,
  "mapZ":0
}
```

### Alanlar

| Alan | Açıklama |
|------|----------|
| sourceX/Y/Z | RTLS'ten gelen koordinat |
| mapX/Y/Z | Harita üzerinde kullanılacak koordinat |

Frontend her zaman **mapX**, **mapY**, **mapZ** değerlerini kullanarak tag'i çizer.

## Hesaplama Mantığı

Sistem sırasıyla;

1. Source Origin çıkarılır.
2. Scale uygulanır.
3. Rotation uygulanır.
4. Map Origin eklenir.

Kod olarak mantık şöyledir;

```
dx = (x - sourceOriginX) * scaleX
dy = (y - sourceOriginY) * scaleY
dz = (z - sourceOriginZ) * scaleZ

Rotation uygulanır.

mapX = mapOriginX + rotatedX
mapY = mapOriginY + rotatedY
mapZ = mapOriginZ + dz
```

Default calibration kullanılırsa;

```
RTLS (25,40)
↓

Map (25,40)
```

olarak döner.

---

# 5. Anchor Position

Anchor'ın harita üzerindeki sabit konumudur.

RTLS cihazlarının harita üzerinde doğru yerde gösterilebilmesi için kullanılır.

## Anchor Ekle / Güncelle

**POST** `/api/Maps/{mapId}/anchors`

### Request

```json
{
  "anchorId":"anchor-guid",
  "x":10,
  "y":20,
  "z":0,
  "rotation":0,
  "metadataJson":"{\"note\":\"Main Anchor\"}"
}
```

### Response

```json
{
  "id":"anchor-position-guid",
  "status":"created"
}
```

Aynı Map + Anchor tekrar gönderilirse yeni kayıt oluşturulmaz, mevcut kayıt güncellenir.

## Anchor Listeleme

**GET** `/api/Maps/{mapId}/anchors`

---

# 6. Zone

Zone, harita üzerindeki özel alanlardır.

Örneğin;

- Normal
- Restricted
- Dangerous
- AssemblyPoint
- EntryExit
- Tunnel
- Storage
- Other

## Zone Oluşturma

**POST** `/api/Maps/{mapId}/zones`

### Request

```json
{
  "name":"Riskli Bölge",
  "zoneType":"Dangerous",
  "color":"#F44336",
  "geometryJson":"[{\"x\":70,\"y\":70},{\"x\":95,\"y\":70},{\"x\":95,\"y\":95},{\"x\":70,\"y\":95}]"
}
```

`geometryJson`, polygonun köşe noktalarını tutar.

Örnekte 4 köşeli bir alan tanımlanmıştır.

## Zone Listeleme

**GET** `/api/Maps/{mapId}/zones`

### Zone Hesaplama

Sistem her gelen koordinatta şu kontrolü yapar;

```
Tag koordinatı polygon içinde mi?

Evet  → Tag bu zone içindedir.

Hayır → Zone dışında kabul edilir.
```

---

# Kullanım Sırası

1. Map oluştur.
2. Harita dosyalarını yükle (DWG, DXF, GLB, SVG, Preview Image vb.).
3. Anchor konumlarını ekle.
4. Zone alanlarını oluştur.
5. Gerekirse Calibration ayarla.
6. RTLS koordinatını Transform endpoint'i ile harita koordinatına çevir.
7. Frontend mapX/mapY değerleriyle tag'i haritada gösterir.

---

# Özet

| Kavram | Açıklama |
|---------|----------|
| Map | Harita kaydı |
| Map File | Haritaya ait dosyalar |
| Calibration | RTLS → Map koordinat dönüşümü |
| Transform | Koordinatı dönüştüren endpoint |
| Anchor Position | Anchor'ın haritadaki konumu |
| Zone | Harita üzerindeki özel alan |

En basit kullanımda hiçbir ayar yapmaya gerek yoktur.

Map oluşturulduğunda gelen Default Calibration sayesinde;

```
RTLS (10,20)

↓

Map (10,20)
```

olarak çalışır.

Sadece gerçek harita ölçüsü farklıysa Calibration değiştirilir. Bunun dışında frontend veya event mantığında herhangi bir değişiklik yapılmasına gerek yoktur.


# System Health

System Health modülü, API'nin çalıştığı ortamın temel sağlık durumunu takip etmek için kullanılır.

Şu an sistem 3 ana başlığı takip eder:

- API Health
- Database Health
- Server Health

Amaç frontend veya yönetim ekranının sistemin çalışır durumda olup olmadığını tek noktadan görebilmesidir.

---

# 1. API Health

API Health, Coal Core API'nin genel çalışma durumunu gösterir.

Takip edilen temel bilgiler:

- API ayakta mı
- API uptime
- Son restart zamanı
- Process memory kullanımı
- CPU kullanımı
- Toplam request sayısı
- Hatalı request sayısı
- Ortalama response süresi
- Son 5 dakikadaki 5xx hata sayısı
- Son 15 dakikadaki 5xx hata sayısı

## Mantık

Her HTTP request'i `SystemMetricsMiddleware` üzerinden geçer.

Middleware request başlangıç ve bitiş zamanını ölçer.

```text
Request gelir
      ↓
SystemMetricsMiddleware
      ↓
Request süresi ölçülür
      ↓
HTTP status code kontrol edilir
      ↓
Metrics Store güncellenir
```

Örneğin API aşağıdaki gibi bilgi üretebilir:

```json
{
  "status": "Healthy",
  "uptimeSeconds": 86400,
  "startedAt": "2026-08-16T10:00:00Z",
  "processMemoryMb": 320.45,
  "cpuUsagePercent": 12.6,
  "totalRequests": 125430,
  "failedRequests": 214,
  "averageResponseTimeMs": 42.8,
  "serverErrorsLast5Minutes": 2,
  "serverErrorsLast15Minutes": 7
}
```

Buradaki `5xx` değerleri API tarafında oluşan server hatalarını ifade eder.

---

# 2. Database Health

Database Health, PostgreSQL veritabanına erişimin sağlıklı olup olmadığını kontrol eder.

Kontrol edilen bilgiler:

- PostgreSQL bağlantısı başarılı mı
- Database response süresi
- Son başarılı kontrol zamanı
- Son hata zamanı
- Son hata mesajı
- Aktif database connection bilgisi alınabiliyorsa connection durumu

## Mantık

Sistem belirli aralıklarla PostgreSQL'e basit bir sorgu gönderir.

Örneğin mantıksal olarak:

```sql
SELECT 1;
```

Kontrol akışı:

```text
API
 ↓
PostgreSQL'e test sorgusu
 ↓
Başarılı mı?
 ↓
EVET
Status = Healthy

HAYIR
Status = Unhealthy
```

Aynı zamanda sorgunun ne kadar sürede cevap verdiği ölçülür.

Örnek response:

```json
{
  "status": "Healthy",
  "isConnected": true,
  "responseTimeMs": 8.4,
  "lastSuccessfulCheckAt": "2026-08-17T07:40:00Z",
  "lastFailedCheckAt": null,
  "lastError": null
}
```

Database'e ulaşılamazsa örneğin:

```json
{
  "status": "Unhealthy",
  "isConnected": false,
  "responseTimeMs": null,
  "lastSuccessfulCheckAt": "2026-08-17T07:39:30Z",
  "lastFailedCheckAt": "2026-08-17T07:40:00Z",
  "lastError": "Database connection failed."
}
```

---

# 3. Server Health

Server Health, API'nin çalıştığı makine veya container üzerindeki temel kaynak kullanımını gösterir.

Takip edilen bilgiler:

- CPU kullanımı
- RAM kullanımı
- Process RAM kullanımı
- Disk kullanımı
- Server uptime
- Process uptime

API Windows, Linux veya Docker üzerinde çalışabilir.

.NET tarafından erişilebilen bilgiler çalışma ortamına göre alınır.

Örnek response:

```json
{
  "status": "Healthy",
  "cpuUsagePercent": 18.2,
  "memoryUsagePercent": 61.4,
  "processMemoryMb": 325.7,
  "diskUsagePercent": 47.8,
  "processUptimeSeconds": 86400
}
```

---

# System Health Genel Mantık

```text
Frontend
   ↓
System Health API
   ↓
┌─────────────────────┐
│ API Health          │
│ Database Health     │
│ Server Health       │
└─────────────────────┘
   ↓
Tek response
```

Frontend bu bilgileri kullanarak örneğin şu şekilde gösterebilir:

```text
API       → Healthy
Database  → Healthy
Server    → Healthy
```

Sorun oluşursa:

```text
API       → Healthy
Database  → Unhealthy
Server    → Warning
```

Bu modül operasyon ekibinin API, database veya server kaynaklı problemleri hızlı şekilde ayırabilmesini sağlar.

---

# Equipment Management

Equipment Management modülü, madende veya işletme sahasında bulunan fiziksel ekipmanların yönetilmesi için kullanılır.

Örnek ekipmanlar:

- Yangın tüpü
- İlk yardım kiti
- Sedye
- Oksijen tüpü
- Acil durum telefonu
- Gaz dedektörü
- Kurtarma ekipmanı
- Elektrik panosu
- Bakım ekipmanı

Sistem sabit ekipman tipleri kullanmaz.

Önce kategori oluşturulur, daha sonra ekipmanlar bu kategoriye bağlanır.

Genel yapı:

```text
Equipment Category
        ↓
Equipment
        ↓
Equipment Inspection
```

---

# 1. Equipment Category

Equipment Category, ekipmanın türünü tanımlar.

Örneğin:

```text
Yangın Tüpü
İlk Yardım Kiti
Sedye
Gaz Dedektörü
```

Kategori üzerinde ayrıca:

- Icon
- Haritada göster / gösterme
- Active / Inactive

bilgileri bulunur.

## Kategori Oluşturma

**POST** `/api/Equipment/categories`

### Request

```json
{
  "companyId": "company-guid",
  "code": "FIRE_EXTINGUISHER",
  "name": "Yangın Tüpü",
  "description": "Yangın söndürme tüpleri",
  "icon": "fire-extinguisher",
  "showOnMap": true
}
```

### Response

```json
{
  "id": "category-guid"
}
```

`icon`, frontend'in kategori için kullanacağı icon bilgisidir.

`showOnMap`, bu kategoriye bağlı ekipmanların haritada gösterilip gösterilmeyeceğini belirler.

Örneğin:

```json
{
  "showOnMap": true
}
```

ise ekipman harita endpoint'inde dönebilir.

```json
{
  "showOnMap": false
}
```

ise o kategoriye ait ekipman kayıtları sistemde kalır fakat haritada gösterilmez.

## Kategorileri Listeleme

**GET** `/api/Equipment/categories`

Filtre örneği:

```text
GET /api/Equipment/categories?companyId={companyId}&isActive=true
```

---

# 2. Equipment

Equipment, sahadaki gerçek fiziksel ekipman kaydıdır.

Örneğin:

```text
Kategori : Yangın Tüpü
Kod      : YT-0001
Ad        : Galeri A Yangın Tüpü
```

Bir ekipman şu bilgilerle tutulabilir:

- Company
- Branch
- Category
- Code
- Name
- Serial Number
- Manufacturer
- Model
- Status
- Floor Map
- X, Y, Z
- Kurulum tarihi
- Son kullanma tarihi
- Son kontrol tarihi
- Sonraki kontrol tarihi
- Notes
- Metadata

## Equipment Oluşturma

**POST** `/api/Equipment`

### Request

```json
{
  "companyId": "company-guid",
  "branchId": "branch-guid",
  "categoryId": "category-guid",

  "code": "YT-0001",
  "name": "Galeri A Yangın Tüpü",

  "serialNumber": "SN-2026-00001",
  "manufacturer": "ABC Yangın",
  "model": "6KG-KKT",

  "status": "Active",

  "floorMapId": "map-guid",
  "x": 32.5,
  "y": 18.75,
  "z": 0,

  "installedAt": "2026-01-15T00:00:00Z",
  "expirationDate": "2028-01-15T00:00:00Z",
  "nextInspectionAt": "2026-09-01T00:00:00Z",

  "notes": "Galeri giriş kapısının sağ tarafında.",

  "metadataJson": "{\"capacityKg\":6,\"extinguisherType\":\"DryChemical\"}"
}
```

### Response

```json
{
  "id": "equipment-guid"
}
```

---

# 3. Equipment Status

Equipment için kullanılabilecek durumlar:

| Status | Açıklama |
|---|---|
| Active | Ekipman aktif ve kullanılabilir |
| Maintenance | Bakımda |
| OutOfService | Kullanım dışı |
| Missing | Yerinde bulunamadı |
| Expired | Kullanım veya geçerlilik süresi dolmuş |
| Retired | Kalıcı olarak kullanım dışı bırakılmış |

`IsActive` ile `Status` aynı şey değildir.

Örneğin:

```text
IsActive = true
Status = Maintenance
```

olabilir.

Bu durumda kayıt sistemde aktiftir fakat ekipman bakım durumundadır.

---

# 4. Equipment Listeleme

**GET** `/api/Equipment`

Örnek:

```text
GET /api/Equipment?page=1&pageSize=20
```

Filtreler birlikte kullanılabilir.

### Company

```text
GET /api/Equipment?companyId={companyId}
```

### Branch

```text
GET /api/Equipment?branchId={branchId}
```

### Category

```text
GET /api/Equipment?categoryId={categoryId}
```

### Status

```text
GET /api/Equipment?status=Active
```

### Map

```text
GET /api/Equipment?floorMapId={floorMapId}
```

### Search

```text
GET /api/Equipment?search=yangın
```

Search alanı ekipmanın kodu, adı, seri numarası, üreticisi veya modeli üzerinden arama yapmak için kullanılabilir.

---

# 5. Equipment ve Harita

Equipment bir FloorMap üzerine yerleştirilebilir.

Bunun için:

```text
FloorMapId
X
Y
Z
```

bilgileri kullanılır.

Mantık:

```text
FloorMapId yok
    ↓
Equipment haritaya bağlı değildir.

FloorMapId var
    ↓
X ve Y zorunludur.

Z opsiyoneldir.
```

Örneğin:

```json
{
  "floorMapId": "map-guid",
  "x": 32.5,
  "y": 18.75,
  "z": 0
}
```

Bu ekipmanın ilgili haritada:

```text
X = 32.5
Y = 18.75
Z = 0
```

noktasında olduğunu belirtir.

---

# 6. Haritada Gösterilecek Equipment Listesi

Frontend'in harita üzerinde ekipman göstermek için kullanacağı endpoint:

**GET** `/api/Equipment/map/{floorMapId}`

### Response

```json
[
  {
    "id": "equipment-guid",
    "categoryId": "category-guid",
    "categoryCode": "FIRE_EXTINGUISHER",
    "categoryName": "Yangın Tüpü",
    "icon": "fire-extinguisher",
    "code": "YT-0001",
    "name": "Galeri A Yangın Tüpü",
    "status": "Active",
    "x": 32.5,
    "y": 18.75,
    "z": 0
  }
]
```

Bir equipment'ın bu response içinde dönmesi için:

```text
Equipment aktif olmalı
        ↓
Category aktif olmalı
        ↓
Category ShowOnMap = true olmalı
        ↓
FloorMapId eşleşmeli
        ↓
X ve Y bilgileri bulunmalı
```

Bu şartlardan biri sağlanmazsa equipment harita listesinde dönmez.

Örneğin kategori:

```json
{
  "showOnMap": false
}
```

olarak değiştirilirse kategoriye ait ekipmanlar database'den silinmez.

Sadece harita endpoint'inden çıkar.

---

# 7. Equipment Inspection

Inspection, ekipmanın periyodik kontrol geçmişidir.

Örneğin yangın tüpü kontrolünde:

- Basınç kontrolü
- Mühür kontrolü
- Fiziksel durum
- Son kullanma tarihi
- Bir sonraki kontrol tarihi

gibi bilgiler tutulabilir.

## Inspection Ekleme

**POST** `/api/Equipment/{equipmentId}/inspections`

### Request

```json
{
  "result": "Passed",
  "inspectedAt": "2026-08-12T10:00:00Z",
  "note": "Basınç normal. Pim ve mühür sağlam.",
  "nextInspectionAt": "2026-11-12T10:00:00Z",
  "dataJson": "{\"pressure\":\"normal\",\"seal\":true}"
}
```

### Response

```json
{
  "id": "inspection-guid"
}
```

Inspection sonucu:

| Result | Açıklama |
|---|---|
| Passed | Kontrol başarılı |
| Failed | Kontrol başarısız |
| NeedsMaintenance | Bakım gerekiyor |

Inspection kaydı oluşturulduğunda equipment üzerindeki:

```text
LastInspectionAt
NextInspectionAt
```

alanları otomatik güncellenir.

---

# 8. Inspection Geçmişi

**GET** `/api/Equipment/{equipmentId}/inspections`

### Response

```json
[
  {
    "id": "inspection-guid",
    "equipmentId": "equipment-guid",
    "inspectedByUserId": "user-guid",
    "inspectedByFullName": "Ahmet Koç",
    "inspectedAt": "2026-08-12T10:00:00Z",
    "result": "Passed",
    "note": "Basınç normal. Pim ve mühür sağlam.",
    "nextInspectionAt": "2026-11-12T10:00:00Z",
    "dataJson": "{\"pressure\":\"normal\",\"seal\":true}"
  }
]
```

Böylece ekipmanın kim tarafından, ne zaman ve hangi sonuçla kontrol edildiği geçmişe dönük olarak görülebilir.

---

# Equipment Kullanım Sırası

1. Equipment Category oluştur.
2. Kategori için icon belirle.
3. Kategorinin haritada gösterilip gösterilmeyeceğini `ShowOnMap` ile belirle.
4. Equipment oluştur.
5. Company ve gerekirse Branch bağla.
6. Haritada gösterilecekse FloorMap ve X/Y/Z bilgilerini gir.
7. Periyodik kontrolleri Inspection olarak kaydet.
8. Frontend harita için `/api/Equipment/map/{floorMapId}` endpoint'ini kullanır.

---

# Equipment Özet

| Kavram | Açıklama |
|---|---|
| Equipment Category | Ekipmanın türü |
| Icon | Frontend'de kullanılacak kategori ikonu |
| ShowOnMap | Kategorinin haritada gösterilip gösterilmeyeceği |
| Equipment | Sahadaki fiziksel ekipman |
| FloorMapId | Equipment'ın bağlı olduğu harita |
| X/Y/Z | Harita üzerindeki konumu |
| Status | Equipment'ın operasyonel durumu |
| ExpirationDate | Son kullanma / geçerlilik tarihi |
| Inspection | Equipment kontrol geçmişi |
| LastInspectionAt | Son kontrol tarihi |
| NextInspectionAt | Bir sonraki kontrol tarihi |

En basit örnek:

```text
Yangın Tüpü kategorisi
        ↓
ShowOnMap = true
Icon = fire-extinguisher
        ↓
YT-0001 equipment
        ↓
FloorMap = Yeraltı Haritası
X = 32.5
Y = 18.75
        ↓
Frontend haritada yangın tüpü iconunu gösterir.
```

