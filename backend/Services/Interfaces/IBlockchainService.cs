namespace backend.Services.Interfaces;

public interface IBlockchainService
{
    Task InitializeAsync();
    string GenerateNewWallet();
    Task<decimal> GetBalanceAsync(string walletAddress);
    Task RewardSensorAsync(string sensorWalletAddress);
}
