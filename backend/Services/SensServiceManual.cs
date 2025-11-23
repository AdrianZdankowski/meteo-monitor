using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Microsoft.Extensions.Options;
using backend.Models;
using System.Numerics;
using backend.Services.SmartContract.ContractDefinition;

namespace backend.Services;

public class TestService
{
    private readonly Web3 _web3;

    public TestService()
    {
        var account = new Account("0x...");
        _web3 = new Web3(account, "http://...");
    }
}
