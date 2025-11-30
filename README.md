# 🌤️ Meteo Monitor

A comprehensive IoT weather monitoring platform with real-time data visualization, blockchain-based sensor rewards, and distributed microservices architecture.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
![React](https://img.shields.io/badge/React-19.2-61DAFB)
![Solidity](https://img.shields.io/badge/Solidity-0.8-363636)

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Technology Stack](#%EF%B8%8F-technology-stack)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Services](#-services)
  - [Backend API](#backend-api)
  - [Frontend](#frontend)
  - [Sensor Simulator](#sensor-simulator)
  - [Blockchain](#blockchain)
- [API Reference](#-api-reference)
- [Real-time Communication](#-real-time-communication)
- [Configuration](#%EF%B8%8F-configuration)
- [License](#-license)

## 🔭 Overview

Meteo Monitor is a full-stack IoT weather monitoring solution that simulates multiple weather sensors, collects their data via MQTT protocol, stores readings in MongoDB, and provides real-time visualization through a React dashboard. The platform features an innovative blockchain-based reward system using ERC-20 tokens (SETO) to incentivize sensor contributions.

## ✨ Features

- **🌡️ Multi-Sensor Support**: Temperature, humidity, pressure, and wind sensors
- **📊 Real-time Dashboard**: Live sensor data visualization with automatic updates
- **📈 Historical Data**: Filter, sort, and analyze historical sensor readings with charts
- **🔗 Blockchain Rewards**: ERC-20 token rewards for sensor data contributions
- **💰 Token Dashboard**: Track sensor wallets and token balances
- **🔄 MQTT Integration**: Lightweight pub/sub messaging for IoT communication
- **📡 SignalR WebSocket**: Real-time push notifications to connected clients
- **🐳 Docker Ready**: Complete containerized deployment with Docker Compose

### 🏗️ Data Flow

1. **Sensor Simulator** generates realistic weather data and publishes to MQTT topics
2. **MQTT Broker (Mosquitto)** receives and routes sensor messages
3. **Backend Service** subscribes to MQTT, processes data, and stores in MongoDB
4. **Backend** rewards sensors with SETO tokens via smart contract
5. **Frontend** fetches data via REST API and receives real-time updates via SignalR
6. **Dashboard** displays live sensor readings, historical charts, and token balances

## 🛠️ Technology Stack

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core | 9.0 | Web API Framework |
| MongoDB.Driver | 2.29.0 | Database connectivity |
| MQTTnet | 4.3.7 | MQTT client library |
| Nethereum | 4.29.0 | Ethereum/.NET integration |
| SignalR | Built-in | WebSocket real-time communication |

### Frontend
| Technology | Version | Purpose |
|------------|---------|---------|
| React | 19.2 | UI Framework |
| Vite | 7.2 | Build tool & dev server |
| Axios | 1.13.2 | HTTP client |
| Recharts | 3.4.1 | Data visualization |
| SignalR Client | 10.0.0 | Real-time updates |
| Lucide React | 0.554.0 | Icons |

### Infrastructure
| Technology | Purpose |
|------------|---------|
| Docker & Docker Compose | Containerization |
| MongoDB | Document database |
| Eclipse Mosquitto | MQTT message broker |
| Anvil (Foundry) | Local Ethereum blockchain |
| Nginx | Frontend web server |

### Blockchain
| Technology | Version | Purpose |
|------------|---------|---------|
| Solidity | 0.8.x | Smart contract language |
| ERC-20 | Standard | Token implementation |

## 📋 Prerequisites

- **Docker** & **Docker Compose** (recommended)
- Or for manual setup:
  - .NET 9.0 SDK
  - Node.js 18+ & npm
  - Python 3.8+
  - MongoDB 6+
  - Mosquitto MQTT Broker

## 🚀 Getting Started

### Docker Deployment

The easiest way to run the entire platform:

```bash
# Clone the repository
git clone https://github.com/AdrianZdankowski/meteo-monitor.git
cd meteo-monitor

# Start all services
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

**Access the application:**
- 🌐 **Frontend**: http://localhost:3000
- 🔌 **Backend API**: http://localhost:5252/api
- 📊 **MongoDB**: localhost:27017
- 📡 **MQTT Broker**: localhost:1883 (TCP), localhost:9001 (WebSocket)
- ⛓️ **Blockchain RPC**: http://localhost:8545

## 🔧 Services

### Backend API

The ASP.NET Core backend provides REST APIs and real-time communication:

#### Key Services

| Service | Description |
|---------|-------------|
| `MongoDbService` | MongoDB database operations |
| `MqttService` | MQTT message subscription & processing |
| `BlockchainService` | Ethereum smart contract interactions |
| `DashboardHub` | SignalR hub for real-time updates |

#### Sensor Processing Flow

1. MQTT message received on `sensors/#` topic
2. Parse JSON payload (sensor_id, sensor_type, timestamp, value)
3. Register new sensors with generated Ethereum wallet
4. Store reading in MongoDB
5. Broadcast update via SignalR
6. Reward sensor with SETO tokens (async)

### Frontend

React-based SPA with three main views:

| View | Description |
|------|-------------|
| **Dashboard** | Real-time sensor cards with live values |
| **History** | Filterable data table and charts |
| **Tokens** | Sensor wallet addresses and token balances |

### Sensor Simulator

Python-based weather station simulator with 16 pre-configured sensors:

| Type | Sensors | Unit | Range |
|------|---------|------|-------|
| Temperature | TEMP_001 - TEMP_004 | °C | -30 to 45 |
| Humidity | HUM_001 - HUM_004 | % | 0 to 100 |
| Pressure | PRESS_001 - PRESS_004 | hPa | ~1000-1030 |
| Wind | WIND_001 - WIND_004 | m/s | 0 to 30 |

#### Installation

```bash
cd simulator
pip install -r requirements.txt
```

#### Usage

**Continuous Simulation Mode (default):**

Runs all 16 sensors continuously, publishing data at randomized intervals (0.5-1.5 seconds):

```bash
python simulator.py
```

Press `Ctrl+C` to stop the simulator.

**Single Reading Mode:**

Send a single reading from a specific sensor and exit:

```bash
python simulator.py --single --sensor <SENSOR_ID> --value '<JSON_VALUE>'
```

**Examples:**

```bash
# Send a temperature reading
python simulator.py --single --sensor TEMP_001 --value '22.5'

# Send a humidity reading
python simulator.py --single --sensor HUM_002 --value '65.3'

# Send a pressure reading
python simulator.py --single --sensor PRESS_001 --value '1013.25'

# Send a wind speed reading
python simulator.py --single --sensor WIND_003 --value '8.7'
```

#### Command Line Arguments

| Argument | Description |
|----------|-------------|
| `--single` | Enable single-send mode (send one reading and exit) |
| `--sensor <ID>` | Sensor ID to use (e.g., TEMP_001, HUM_002) |
| `--value <JSON>` | Value to send as JSON or plain number |

#### Available Sensor IDs

- **Temperature**: `TEMP_001`, `TEMP_002`, `TEMP_003`, `TEMP_004`
- **Humidity**: `HUM_001`, `HUM_002`, `HUM_003`, `HUM_004`
- **Pressure**: `PRESS_001`, `PRESS_002`, `PRESS_003`, `PRESS_004`
- **Wind**: `WIND_001`, `WIND_002`, `WIND_003`, `WIND_004`

#### MQTT Connection

By default, the simulator connects to the MQTT broker at `localhost:1883`. When running with Docker Compose, ensure the MQTT broker container is running before starting the simulator.

### Blockchain

ERC-20 token contract (SensorToken - SETO) deployed on local Anvil chain:

| Property | Value |
|----------|-------|
| Name | SensorToken |
| Symbol | SETO |
| Decimals | 18 |
| Initial Supply | 1,000,000 SETO |
| Reward per Reading | 10 SETO |

**Contract Functions:**
- `transfer(address _to, uint256 _value)` - Transfer tokens
- `balanceOf(address _owner)` - Query balance
- `approve(address _spender, uint256 _value)` - Approve spending
- `transferFrom(address _from, address _to, uint256 _value)` - Delegated transfer

## 📚 API Reference

### Sensors

#### Get All Sensors

```http
GET /api/sensors
```

**Response:**
```json
[
  {
    "id": "...",
    "sensorId": "TEMP_001",
    "sensorType": "temperature",
    "walletAddress": "0x...",
    "tokenBalance": 150.0
  }
]
```

#### Get Sensor Balance

```http
GET /api/sensors/{sensorId}/balance
```

### Readings

#### Get Readings

```http
GET /api/readings?sensorId={id}&sensorType={type}&from={timestamp}&to={timestamp}&sort={asc|desc}
```

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| sensorId | string | Filter by sensor ID |
| sensorType | string | Filter by type (temperature, humidity, etc.) |
| from | double | Unix timestamp start |
| to | double | Unix timestamp end |
| sort | string | Sort order (asc/desc) |

### Dashboard

#### Get Dashboard Data

```http
GET /api/dashboard
```

**Response:**
```json
[
  {
    "sensorId": "TEMP_001",
    "sensorType": "temperature",
    "lastValue": 22.5,
    "lastTimestamp": 1732982400,
    "averageLast100": 21.8
  }
]
```

## 🔄 Real-time Communication

SignalR hub at `/dashboardHub` broadcasts sensor updates:

**Event:** `ReceiveSensorUpdate`

```javascript
connection.on('ReceiveSensorUpdate', (sensorId, value, timestamp) => {
  console.log(`Sensor ${sensorId}: ${value} at ${timestamp}`);
});
```

## ⚙️ Configuration

### Backend Configuration (`appsettings.json`)

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://admin:admin123@mongodb:27017/?authSource=admin",
    "DatabaseName": "meteomonitor"
  },
  "Mqtt": {
    "BrokerHost": "mqtt-broker",
    "BrokerPort": 1883,
    "ClientId": "backend-service",
    "Topics": ["sensors/#"]
  },
  "Blockchain": {
    "Enabled": true,
    "RpcUrl": "http://anvil:8545",
    "PrivateKey": "0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80",
    "ContractAddress": "",
    "RewardAmount": 10,
    "ChainId": 31337
  }
}
```

### MQTT Topic Structure

```
sensors/
├── temperature/
│   ├── TEMP_001
│   ├── TEMP_002
│   └── ...
├── humidity/
│   └── ...
├── pressure/
│   └── ...
└── wind/
    └── ...
```

**Message Format:**
```json
{
  "sensor_id": "TEMP_001",
  "sensor_type": "temperature",
  "timestamp": 1732982400.123,
  "value": 22.5
}
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
