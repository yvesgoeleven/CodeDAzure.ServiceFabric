using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric;
using Microsoft.ServiceFabric.Services;

namespace Gateway
{
    public class Gateway : StatelessService
    {
        protected override ICommunicationListener CreateCommunicationListener()
        {
            // TODO: Replace this with an ICommunicationListener implementation if your service needs to handle user requests.
            return base.CreateCommunicationListener();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(this, "Gateway Started");

            var leaseId = Guid.NewGuid().ToString();
            var resource = Guid.NewGuid().ToString();

            var leaseManagerUri = new Uri("fabric:/LeaseManagerHost/LeaseManager");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var leaseClient = ServiceProxy.Create<ILeaseManager>((long)1, leaseManagerUri);

                    ServiceEventSource.Current.ServiceMessage(this, "Trying to acquire lease {0} on resource {1}", leaseId, resource);

                    var lease = await leaseClient.AcquireLeaseAsync(resource, leaseId);

                    ServiceEventSource.Current.ServiceMessage(this, "Lease on resource {0} {1} ", resource, lease.Acquired ? "acquired" : "not acquired");

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                catch (AggregateException ex)
                {
                    ServiceEventSource.Current.ServiceMessage(this, "Failed to acquire lease {0} on resource {1}, retrying", leaseId, resource);

                    var inner = ex.InnerException;
                    while (inner != null)
                    {
                        ServiceEventSource.Current.ServiceMessage(this, "Exception details {0}", ex);
                        inner = inner.InnerException;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
        }
    }
}
