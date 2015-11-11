using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Services;

namespace LeaseManager
{
    public class LeaseManager : StatefulService, ILeaseManager
    {
        protected override ICommunicationListener CreateCommunicationListener()
        {
            return new ServiceCommunicationListener<LeaseManager>(this);
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(this, "LeaseManager Started");

            return Task.FromResult(true);
        }

        public Task<LeaseResult> AcquireLeaseAsync(string resourceName, string leaseId)
        {
            return Task.FromResult(new LeaseResult
            {
                Acquired = false,
                LeaseId = leaseId,
                ResourceName = resourceName,
                ValidUntilUtc = DateTime.UtcNow.AddSeconds(30)
            });
        }

        public Task ReleaseLeaseAsync(string resourceName, string leaseId)
        {
            return Task.FromResult(true);
        }
    }
}
