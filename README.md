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




