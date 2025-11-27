# Meteo Monitor

IoT weather monitoring system with blockchain-based sensor token rewards.

## Features

- **Real-time Sensor Data**: MQTT-based sensor data collection and real-time dashboard updates via SignalR
- **Blockchain Integration**: ERC-20 token rewards for sensors contributing data
- **Modern Stack**: 
  - Backend: ASP.NET Core 9.0
  - Frontend: React with Vite
  - Database: MongoDB
  - Blockchain: Ethereum (Anvil for local development)
  - Message Broker: MQTT (Eclipse Mosquitto)

## Quick Start

```bash
# Start all services
docker-compose up --build

# Deploy smart contract
cd blockchain
npm install
npm run deploy

# Update contract address in docker-compose.yaml and restart backend
docker-compose restart backend
```

## Services

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5252
- **MongoDB**: localhost:27017
- **MQTT Broker**: localhost:1883
- **Anvil Blockchain**: localhost:8545

## Documentation

See [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md) for detailed setup instructions and troubleshooting.

## License

MIT