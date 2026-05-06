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


