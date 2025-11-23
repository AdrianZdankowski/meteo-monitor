namespace backend.Models;

public class BlockchainSettings
{
    public bool Enabled { get; set; } = true;
    public string RpcUrl { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
    public string ContractAddress { get; set; } = string.Empty;
    public decimal RewardAmount { get; set; } = 10;
    public long ChainId { get; set; } = 31337;
}
