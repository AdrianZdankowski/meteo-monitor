# Meteo Monitor - Blockchain Integration Setup

## Prerequisites

- Docker and Docker Compose
- Node.js 18+ (for blockchain deployment script)
- .NET 9.0 SDK (for backend development)

## Quick Start

### 1. Start the Infrastructure

```bash
docker-compose up --build -d
```

This will start:
- MongoDB (port 27017)
- MQTT Broker (port 1883)
- Anvil Blockchain (port 8545)
- Backend API (port 5252)
- Frontend (port 3000)

### 2. Deploy the Smart Contract

```bash
cd blockchain
npm install
npm run deploy
```

**Important**: Copy the contract address from the deployment output.

### 3. Configure the Backend

Update the `Blockchain__ContractAddress` environment variable in `docker-compose.yaml` with the deployed contract address, then restart the backend:

```bash
docker-compose restart backend
```

Alternatively, you can update `backend/appsettings.json`:

```json
{
  "Blockchain": {
    "Enabled": true,
    "RpcUrl": "http://anvil:8545",
    "PrivateKey": "0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80",
    "ContractAddress": "<YOUR_CONTRACT_ADDRESS>",
    "RewardAmount": 10,
    "ChainId": 31337
  }
}
```

### 4. Test the System

1. **Start the Simulator** (if available):
   ```bash
   cd simulator
   python simulator.py
   ```

2. **Access the Frontend**:
   Open http://localhost:3000 and navigate to the "Token Rewards" tab

3. **Verify Rewards**:
   - Send sensor data via MQTT
   - Check backend logs for reward transactions
   - View updated balances in the frontend

## Architecture

### Smart Contract
- **Location**: `blockchain/contracts/SensorToken.sol`
- **Type**: ERC-20 token
- **Symbol**: SENS
- **Decimals**: 18
- **Network**: Anvil (local Ethereum development network)

### Backend Integration
- **Service**: `BlockchainService.cs`
- **Features**:
  - Auto-generates wallet addresses for new sensors
  - Rewards sensors with tokens on each MQTT message
  - Queries token balances via Nethereum

### Frontend
- **Component**: `TokenDashboard.jsx`
- **Features**:
  - Displays all sensors with wallet addresses
  - Shows real-time token balances
  - Auto-refreshes every 10 seconds
  - Real-time updates via SignalR

## Configuration

### Blockchain Settings

| Setting | Description | Default |
|---------|-------------|---------|
| `Enabled` | Enable/disable blockchain integration | `true` |
| `RpcUrl` | Anvil RPC endpoint | `http://anvil:8545` |
| `PrivateKey` | Deployer private key (Anvil account #0) | `0xac09...` |
| `ContractAddress` | Deployed SensorToken address | `` (must be set) |
| `RewardAmount` | Tokens per sensor reading | `10` |
| `ChainId` | Network chain ID | `31337` |

### Default Anvil Accounts

Anvil provides deterministic test accounts:

- **Account #0** (Deployer): `0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266`
  - Private Key: `0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80`
  - Initial Balance: 10,000 ETH

## Troubleshooting

### Contract Not Deployed

**Error**: "Contract address not configured"

**Solution**: Deploy the contract and update the configuration:
```bash
cd blockchain
npm run deploy
# Copy the contract address and update docker-compose.yaml or appsettings.json
```

### Blockchain Service Disabled

**Error**: "Blockchain is disabled"

**Solution**: Set `Blockchain__Enabled=true` in environment variables or appsettings.json

### No Wallet Address for Sensor

**Issue**: Sensors show "Not assigned" wallet address

**Solution**: This happens for sensors registered before blockchain integration. Delete the sensor from MongoDB and let it re-register:
```bash
docker exec -it <mongodb-container> mongosh
use meteomonitor
db.sensors.deleteMany({})
```

### Balance Not Updating

**Check**:
1. Backend logs for transaction hashes
2. Anvil logs for mined blocks
3. Contract address is correctly configured
4. Deployer account has sufficient token balance

## API Endpoints

### Get All Sensors with Balances
```
GET /api/sensors
```

Response:
```json
[
  {
    "id": "...",
    "sensorId": "sensor_1",
    "sensorType": "temperature",
    "walletAddress": "0x...",
    "tokenBalance": 150.0
  }
]
```

### Get Sensor Balance
```
GET /api/sensors/{sensorId}/balance
```

Response:
```json
150.0
```

## Development

### Rebuild Smart Contract

```bash
cd blockchain
npm run deploy
```

### View Blockchain Logs

```bash
docker logs -f <anvil-container-name>
```

### View Backend Logs

```bash
docker logs -f <backend-container-name>
```

## Production Deployment

For production, replace Anvil with a testnet or mainnet:

1. Update `RpcUrl` to testnet RPC (e.g., Sepolia, Base Sepolia)
2. Use a secure private key (not the Anvil default)
3. Deploy contract to the network
4. Update `ChainId` to match the network
5. Consider gas optimization and transaction monitoring

## License

MIT
