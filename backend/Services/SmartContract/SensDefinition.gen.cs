using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace backend.Services.SmartContract.ContractDefinition
{


    public partial class SensDeployment : SensDeploymentBase
    {
        public SensDeployment() : base(BYTECODE) { }
        public SensDeployment(string byteCode) : base(byteCode) { }
    }

    public class SensDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "60806040523461034557610a738038038061001981610349565b928339810160808282031261034557815160208301519091906001600160401b038111610345578161004c91850161036e565b9260408101519160ff83168093036103455760608201516001600160401b0381116103455761007b920161036e565b91335f525f6020528060405f2055600255825160018060401b03811161025657600354600181811c9116801561033b575b602082101461023857601f81116102d8575b506020601f821160011461027557819293945f9261026a575b50508160011b915f199060031b1c1916176003555b6005805460ff191691909117905580516001600160401b03811161025657600454600181811c9116801561024c575b602082101461023857601f81116101d5575b50602091601f8211600114610175579181925f9261016a575b50508160011b915f199060031b1c1916176004555b6040516106b390816103c08239f35b015190505f80610146565b601f1982169260045f52805f20915f5b8581106101bd575083600195106101a5575b505050811b0160045561015b565b01515f1960f88460031b161c191690555f8080610197565b91926020600181928685015181550194019201610185565b60045f527f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b601f830160051c8101916020841061022e575b601f0160051c01905b818110610223575061012d565b5f8155600101610216565b909150819061020d565b634e487b7160e01b5f52602260045260245ffd5b90607f169061011b565b634e487b7160e01b5f52604160045260245ffd5b015190505f806100d7565b601f1982169060035f52805f20915f5b8181106102c0575095836001959697106102a8575b505050811b016003556100ec565b01515f1960f88460031b161c191690555f808061029a565b9192602060018192868b015181550194019201610285565b60035f527fc2575a0e9e593c00f959f8c92f12db2869c3395a3b0502d05e2516446f71f85b601f830160051c81019160208410610331575b601f0160051c01905b81811061032657506100be565b5f8155600101610319565b9091508190610310565b90607f16906100ac565b5f80fd5b6040519190601f01601f191682016001600160401b0381118382101761025657604052565b81601f82011215610345578051906001600160401b0382116102565761039d601f8301601f1916602001610349565b928284526020838301011161034557815f9260208093018386015e830101529056fe6080806040526004361015610012575f80fd5b5f3560e01c90816306fdde03146104ca57508063095ea7b31461045157806318160ddd1461043457806323b872dd146102e9578063313ce567146102c957806370a082311461029257806395d89b4114610177578063a9059cbb146100d35763dd62ed3e1461007f575f80fd5b346100cf5760403660031901126100cf576100986105c3565b6100a06105d9565b6001600160a01b039182165f908152600160209081526040808320949093168252928352819020549051908152f35b5f80fd5b346100cf5760403660031901126100cf576100ec6105c3565b60243590335f525f6020526101078260405f205410156105ef565b335f525f60205260405f2061011d83825461064f565b905560018060a01b031690815f525f60205260405f2061013e828254610670565b90556040519081527fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef60203392a3602060405160018152f35b346100cf575f3660031901126100cf576040515f6004548060011c90600181168015610288575b602083108114610274578285529081156102585750600114610203575b50819003601f01601f191681019067ffffffffffffffff8211818310176101ef576101eb82918260405282610599565b0390f35b634e487b7160e01b5f52604160045260245ffd5b905060045f527f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b5f905b828210610242575060209150820101826101bb565b600181602092548385880101520191019061022d565b90506020925060ff191682840152151560051b820101826101bb565b634e487b7160e01b5f52602260045260245ffd5b91607f169161019e565b346100cf5760203660031901126100cf576001600160a01b036102b36105c3565b165f525f602052602060405f2054604051908152f35b346100cf575f3660031901126100cf57602060ff60055416604051908152f35b346100cf5760603660031901126100cf576103026105c3565b61030a6105d9565b6044359160018060a01b031690815f525f60205261032e8360405f205410156105ef565b5f82815260016020908152604080832033845290915290205483116103dd5760207fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef91835f525f825260405f2061038686825461064f565b905560018060a01b031693845f525f825260405f206103a6828254610670565b90555f8481526001835260408082203383528452902080546103c990839061064f565b9055604051908152a3602060405160018152f35b60405162461bcd60e51b815260206004820152602960248201527f20616c6c6f77616e6365206973206c6f776572207468616e20616d6f756e74206044820152681c995c5d595cdd195960ba1b6064820152608490fd5b346100cf575f3660031901126100cf576020600254604051908152f35b346100cf5760403660031901126100cf5761046a6105c3565b335f8181526001602090815260408083206001600160a01b03909516808452948252918290206024359081905591519182527f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b92591a3602060405160018152f35b346100cf575f3660031901126100cf575f6003548060011c9060018116801561058f575b60208310811461027457828552908115610258575060011461053a5750819003601f01601f191681019067ffffffffffffffff8211818310176101ef576101eb82918260405282610599565b905060035f527fc2575a0e9e593c00f959f8c92f12db2869c3395a3b0502d05e2516446f71f85b5f905b828210610579575060209150820101826101bb565b6001816020925483858801015201910190610564565b91607f16916104ee565b602060409281835280519182918282860152018484015e5f828201840152601f01601f1916010190565b600435906001600160a01b03821682036100cf57565b602435906001600160a01b03821682036100cf57565b156105f657565b60405162461bcd60e51b815260206004820152602b60248201527f6163636f756e742062616c616e636520646f6573206e6f74206861766520656e60448201526a6f75676820746f6b656e7360a81b6064820152608490fd5b9190820391821161065c57565b634e487b7160e01b5f52601160045260245ffd5b9190820180921161065c5756fea2646970667358221220480033f437b1c8ac88504df5e7108202151fadee0ec444155c4abd8b84092c1664736f6c634300081e0033";
        public SensDeploymentBase() : base(BYTECODE) { }
        public SensDeploymentBase(string byteCode) : base(byteCode) { }
        [Parameter("uint256", "_initialAmount", 1)]
        public virtual BigInteger InitialAmount { get; set; }
        [Parameter("string", "_tokenName", 2)]
        public virtual string TokenName { get; set; }
        [Parameter("uint8", "_decimalUnits", 3)]
        public virtual byte DecimalUnits { get; set; }
        [Parameter("string", "_tokenSymbol", 4)]
        public virtual string TokenSymbol { get; set; }
    }

    public partial class AllowanceFunction : AllowanceFunctionBase { }

    [Function("allowance", "uint256")]
    public class AllowanceFunctionBase : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public virtual string Owner { get; set; }
        [Parameter("address", "_spender", 2)]
        public virtual string Spender { get; set; }
    }

    public partial class ApproveFunction : ApproveFunctionBase { }

    [Function("approve", "bool")]
    public class ApproveFunctionBase : FunctionMessage
    {
        [Parameter("address", "_spender", 1)]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "_value", 2)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class BalanceOfFunction : BalanceOfFunctionBase { }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunctionBase : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public virtual string Owner { get; set; }
    }

    public partial class DecimalsFunction : DecimalsFunctionBase { }

    [Function("decimals", "uint8")]
    public class DecimalsFunctionBase : FunctionMessage
    {

    }

    public partial class NameFunction : NameFunctionBase { }

    [Function("name", "string")]
    public class NameFunctionBase : FunctionMessage
    {

    }

    public partial class SymbolFunction : SymbolFunctionBase { }

    [Function("symbol", "string")]
    public class SymbolFunctionBase : FunctionMessage
    {

    }

    public partial class TotalSupplyFunction : TotalSupplyFunctionBase { }

    [Function("totalSupply", "uint256")]
    public class TotalSupplyFunctionBase : FunctionMessage
    {

    }

    public partial class TransferFunction : TransferFunctionBase { }

    [Function("transfer", "bool")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_value", 2)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TransferFromFunction : TransferFromFunctionBase { }

    [Function("transferFrom", "bool")]
    public class TransferFromFunctionBase : FunctionMessage
    {
        [Parameter("address", "_from", 1)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 2)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_value", 3)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class ApprovalEventDTO : ApprovalEventDTOBase { }

    [Event("Approval")]
    public class ApprovalEventDTOBase : IEventDTO
    {
        [Parameter("address", "_owner", 1, true )]
        public virtual string Owner { get; set; }
        [Parameter("address", "_spender", 2, true )]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "_value", 3, false )]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TransferEventDTO : TransferEventDTOBase { }

    [Event("Transfer")]
    public class TransferEventDTOBase : IEventDTO
    {
        [Parameter("address", "_from", 1, true )]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 2, true )]
        public virtual string To { get; set; }
        [Parameter("uint256", "_value", 3, false )]
        public virtual BigInteger Value { get; set; }
    }

    public partial class AllowanceOutputDTO : AllowanceOutputDTOBase { }

    [FunctionOutput]
    public class AllowanceOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "remaining", 1)]
        public virtual BigInteger Remaining { get; set; }
    }



    public partial class BalanceOfOutputDTO : BalanceOfOutputDTOBase { }

    [FunctionOutput]
    public class BalanceOfOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "balance", 1)]
        public virtual BigInteger Balance { get; set; }
    }

    public partial class DecimalsOutputDTO : DecimalsOutputDTOBase { }

    [FunctionOutput]
    public class DecimalsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint8", "", 1)]
        public virtual byte ReturnValue1 { get; set; }
    }

    public partial class NameOutputDTO : NameOutputDTOBase { }

    [FunctionOutput]
    public class NameOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class SymbolOutputDTO : SymbolOutputDTOBase { }

    [FunctionOutput]
    public class SymbolOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class TotalSupplyOutputDTO : TotalSupplyOutputDTOBase { }

    [FunctionOutput]
    public class TotalSupplyOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }




}
