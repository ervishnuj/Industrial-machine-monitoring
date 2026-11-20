# Industrial Machine Monitoring System

A real-time industrial machine monitoring application built to track machine health, sensor readings, alerts, and downtime through a web-based dashboard.

## Project Overview

The Industrial Machine Monitoring System collects simulated industrial sensor data, stores it in a MySQL database, detects abnormal machine conditions, and displays machine performance through a live React dashboard.

This project demonstrates the integration of industrial monitoring concepts with modern software technologies.

## Technology Stack

| Component   | Technology                        |
| ----------- | --------------------------------- |
| Backend     | C#, ASP.NET Core Web API (.NET 8) |
| Database    | MySQL                             |
| ORM         | Entity Framework Core             |
| Frontend    | React.js, Vite                    |
| Charts      | Recharts                          |
| Simulator   | Python                            |
| API Testing | Postman                           |

## Key Features

- Real-time machine monitoring dashboard
- Multiple machine monitoring
- Temperature, motor current, and vibration tracking
- Automatic sensor data refresh
- Machine online/offline detection
- Configurable alert threshold detection
- HIGH and CRITICAL alert levels
- Browser notifications
- Alert history and acknowledgement
- Machine downtime tracking
- Downtime history with duration
- Interactive sensor analytics charts
- Machine-based dashboard filtering

## System Architecture

1. Python simulator generates sensor readings.
2. ASP.NET Core Web API receives the readings.
3. Entity Framework Core stores data in MySQL.
4. Backend checks sensor values against configured thresholds.
5. React dashboard fetches and displays the latest data.
6. Alerts and downtime information are displayed in the dashboard.

## Project Structure

```text
IndustrialMachineMonitoring/
│
├── Controllers/
├── Data/
├── Migrations/
├── Models/
├── Properties/
│
├── dashboard/
│   ├── src/
│   ├── package.json
│   └── ...
│
├── python-simulator/
│   └── simulator.py
│
├── Program.cs
├── IndustrialMachineMonitoring.csproj
├── appsettings.example.json
├── .gitignore
└── README.md
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- MySQL Server
- Node.js and npm
- Python 3
- Git

### 1. Clone the Repository

```bash
git clone YOUR_GITHUB_REPOSITORY_URL
cd IndustrialMachineMonitoring
```

### 2. Configure Database

Create a MySQL database named:

```sql
CREATE DATABASE machine_monitoring;
```

Create your local `appsettings.json` using `appsettings.example.json` as a reference.

Update the database connection string with your local MySQL credentials.

Never commit database passwords or private credentials.

### 3. Run Backend

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Backend API:

```text
http://localhost:5247/api
```

### 4. Run React Dashboard

Open another terminal:

```bash
cd dashboard
npm install
npm run dev
```

Open the local URL displayed by Vite in your terminal.

### 5. Run Python Simulator

Open another terminal:

```bash
cd python-simulator
pip install requests
python simulator.py
```

The simulator sends sensor readings to the backend periodically.

## API Endpoints

| Method | Endpoint                          | Purpose               |
| ------ | --------------------------------- | --------------------- |
| GET    | `/api/Machines`                   | Get machines          |
| GET    | `/api/SensorReadings`             | Get sensor readings   |
| POST   | `/api/SensorReadings`             | Submit sensor reading |
| GET    | `/api/MachineAlerts/{machineId}`  | Get machine alerts    |
| GET    | `/api/AlertLogs`                  | Get alert history     |
| PUT    | `/api/AlertLogs/{id}/acknowledge` | Acknowledge alert     |
| GET    | `/api/MachineDowntimes`           | Get downtime history  |

## Alert Thresholds

The current demonstration thresholds are:

| Sensor        | Warning Threshold |
| ------------- | ----------------: |
| Temperature   |       Above 80 °C |
| Motor Current |        Above 15 A |
| Vibration     |           Above 5 |

Critical alert levels are configured separately in the backend.

## Future Improvements

- PLC connectivity and real industrial sensor integration
- Modbus-based data acquisition
- Historical reporting and analytics
- Production deployment
- User authentication and role-based access
- Configurable machine-specific thresholds

## Project Purpose

This project demonstrates practical development skills in industrial monitoring, backend API development, database integration, real-time dashboards, sensor data processing, and alert management.
