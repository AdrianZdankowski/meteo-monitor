const { ethers } = require('ethers');
const fs = require('fs');
const path = require('path');

async function main() {
    // Connect to Anvil
    const provider = new ethers.JsonRpcProvider(process.env.RPC_URL || 'http://localhost:8545');

    // Use first Anvil account (deterministic)
    const privateKey = process.env.PRIVATE_KEY || '0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80';
    const wallet = new ethers.Wallet(privateKey, provider);

    console.log('Deploying contracts with account:', wallet.address);
    console.log('Account balance:', ethers.formatEther(await provider.getBalance(wallet.address)), 'ETH');

    // Read contract source
    const contractPath = path.join(__dirname, '../contracts/SensorToken.sol');
    const source = fs.readFileSync(contractPath, 'utf8');

    // Compile contract (simple compilation for single file)
    const solc = require('solc');
    const input = {
        language: 'Solidity',
        sources: {
            'SensorToken.sol': {
                content: source
            }
        },
        settings: {
            outputSelection: {
                '*': {
                    '*': ['abi', 'evm.bytecode']
                }
            }
        }
    };

    console.log('Compiling contract...');
    const output = JSON.parse(solc.compile(JSON.stringify(input)));

    if (output.errors) {
        output.errors.forEach(error => {
            console.error(error.formattedMessage);
        });
        if (output.errors.some(error => error.severity === 'error')) {
            throw new Error('Compilation failed');
        }
    }

    const contract = output.contracts['SensorToken.sol']['SensorToken'];
    const abi = contract.abi;
    const bytecode = contract.evm.bytecode.object;

    // Deploy contract
    console.log('Deploying SensorToken...');
    const factory = new ethers.ContractFactory(abi, bytecode, wallet);

    // Constructor parameters
    const initialAmount = ethers.parseUnits('1000000', 18); // 1 million tokens
    const tokenName = 'SensorToken';
    const decimalUnits = 18;
    const tokenSymbol = 'SENS';

    const sensorToken = await factory.deploy(initialAmount, tokenName, decimalUnits, tokenSymbol);
    await sensorToken.waitForDeployment();

    const contractAddress = await sensorToken.getAddress();
    console.log('SensorToken deployed to:', contractAddress);

    // Save deployment info
    const deploymentInfo = {
        contractAddress: contractAddress,
        deployerAddress: wallet.address,
        deployerPrivateKey: privateKey,
        tokenName: tokenName,
        tokenSymbol: tokenSymbol,
        decimals: decimalUnits,
        initialSupply: initialAmount.toString(),
        network: 'anvil',
        chainId: (await provider.getNetwork()).chainId.toString(),
        deployedAt: new Date().toISOString()
    };

    const deploymentPath = path.join(__dirname, '../deployment.json');
    fs.writeFileSync(deploymentPath, JSON.stringify(deploymentInfo, null, 2));
    console.log('Deployment info saved to:', deploymentPath);

    // Save ABI
    const abiPath = path.join(__dirname, '../SensorToken.abi.json');
    fs.writeFileSync(abiPath, JSON.stringify(abi, null, 2));
    console.log('ABI saved to:', abiPath);

    console.log('\n=== Deployment Summary ===');
    console.log('Contract Address:', contractAddress);
    console.log('Deployer Address:', wallet.address);
    console.log('Initial Supply:', ethers.formatUnits(initialAmount, 18), tokenSymbol);
    console.log('========================\n');

    console.log('Add this to your backend appsettings.json:');
    console.log(JSON.stringify({
        Blockchain: {
            Enabled: true,
            RpcUrl: 'http://anvil:8545',
            PrivateKey: privateKey,
            ContractAddress: contractAddress,
            RewardAmount: 10,
            ChainId: 31337
        }
    }, null, 2));
}

main()
    .then(() => process.exit(0))
    .catch((error) => {
        console.error(error);
        process.exit(1);
    });
