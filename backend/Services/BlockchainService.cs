using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Microsoft.Extensions.Options;
using backend.Models;
using System.Numerics;
using backend.Services.ContractDefinition.SensorContract;
using backend.Services.ContractDefinition.SensorContract.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;

namespace backend.Services;

public class BlockchainService
{
    private readonly ILogger<BlockchainService> _logger;
    private readonly BlockchainSettings _settings;
    private readonly Web3 _web3;
    private SensorContractService? _sensorContractService;
    private readonly Account _account;

    public BlockchainService(ILogger<BlockchainService> logger, IOptions<BlockchainSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        if (!_settings.Enabled)
        {
            _logger.LogInformation("Blockchain service is disabled");
            return;
        }

        try
        {
            _account = new Account(_settings.PrivateKey, _settings.ChainId);
            _web3 = new Web3(_account, _settings.RpcUrl);
            _logger.LogInformation("Blockchain service initialized with account: {Address}", _account.Address);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize blockchain service");
        }
    }

    public async Task InitializeAsync()
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Blockchain is disabled, skipping initialization");
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(_settings.ContractAddress))
            {
                _logger.LogInformation("No contract address configured. Deploying new contract...");

                var deployment = new SensorContractDeployment()
                {
                    InitialAmount = Web3.Convert.ToWei(1000000),
                    TokenName = "SensorToken",
                    DecimalUnits = 18,
                    TokenSymbol = "SENS"
                };

                _sensorContractService = await SensorContractService.DeployContractAndGetServiceAsync(_web3, deployment);
                var contractAddress = _sensorContractService.ContractAddress;
                
                _logger.LogInformation("Contract deployed to: {Address}", contractAddress);
                
                // Update settings in memory (optional, but good for consistency)
                _settings.ContractAddress = contractAddress;
            }
            else
            {
                _logger.LogInformation("Connecting to SensorToken contract at: {Address}", _settings.ContractAddress);
                _sensorContractService = new SensorContractService(_web3, _settings.ContractAddress);
            }
            
            // Verify contract connection
            var name = await _sensorContractService.NameQueryAsync();
            var symbol = await _sensorContractService.SymbolQueryAsync();
            var balance = await _sensorContractService.BalanceOfQueryAsync(_account.Address);
            
            _logger.LogInformation("Connected to token: {Name} ({Symbol})", name, symbol);
            _logger.LogInformation("Contract owner balance: {Balance} tokens", Web3.Convert.FromWei(balance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to SensorToken contract");
        }
    }

    public string GenerateNewWallet()
    {
        try
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var address = ecKey.GetPublicAddress();
            _logger.LogInformation("Generated new wallet address: {Address}", address);
            return address;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate wallet address");
            return string.Empty;
        }
    }

    public async Task<decimal> GetBalanceAsync(string walletAddress)
    {
        if (!_settings.Enabled || _sensorContractService == null)
        {
            return 0;
        }

        try
        {
            var balance = await _sensorContractService.BalanceOfQueryAsync(walletAddress);
            return Web3.Convert.FromWei(balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting balance for {Wallet}", walletAddress);
            return 0;
        }
    }

    public async Task RewardSensorAsync(string sensorWalletAddress)
    {
        if (!_settings.Enabled)
        {
            _logger.LogDebug("Blockchain disabled, skipping reward");
            return;
        }

        if (_sensorContractService == null)
        {
            _logger.LogWarning("Blockchain service not initialized. Skipping reward.");
            return;
        }

        if (string.IsNullOrEmpty(sensorWalletAddress))
        {
            _logger.LogWarning("Sensor wallet address is empty. Skipping reward.");
            return;
        }

        try
        {
            // Convert reward amount to Wei (assuming 18 decimals)
            var amountInWei = Web3.Convert.ToWei(_settings.RewardAmount);

            _logger.LogInformation("Sending {Amount} tokens to {Wallet}", _settings.RewardAmount, sensorWalletAddress);

            var receipt = await _sensorContractService.TransferRequestAndWaitForReceiptAsync(sensorWalletAddress, amountInWei);

            _logger.LogInformation("Reward transaction successful: {TransactionHash}", receipt.TransactionHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rewarding sensor {Wallet}", sensorWalletAddress);
        }
    }
}
