using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services;

namespace Contracts
{
    public interface ILeaseManager : IService
    {
        Task<LeaseResult> AcquireLeaseAsync(string resourceName, string leaseId);

        Task ReleaseLeaseAsync(string resourceName, string leaseId);
    }
}
