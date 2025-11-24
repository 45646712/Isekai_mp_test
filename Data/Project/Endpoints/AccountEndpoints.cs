using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace Data.Endpoints;

public class AccountEndpoints
{
    [CloudCodeFunction("ValidateAccountData")]
    public async Task ValidateAccountData(IExecutionContext context, IGameApiClient gameApiClient) => await Account.ValidateAccountData(context, gameApiClient);
}