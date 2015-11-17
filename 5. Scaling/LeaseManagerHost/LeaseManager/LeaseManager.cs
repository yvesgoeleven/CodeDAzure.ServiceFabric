using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services;

namespace LeaseManager
{
    public class LeaseManager : StatefulService, ILeaseManager
    {
        private readonly TimeSpan _validity = TimeSpan.FromSeconds(30);
        IReliableDictionary<string, Lease> _leases;

        protected override ICommunicationListener CreateCommunicationListener()
        {
            return new ServiceCommunicationListener<LeaseManager>(this);
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await EnsureLeases();

            ServiceEventSource.Current.ServiceMessage(this, "LeaseManager Started");
        }

        public async Task<LeaseResult> AcquireLeaseAsync(string resourceName, string leaseId)
        {
            await EnsureLeases();

            bool acquired = false;
            Lease result;
            using (var tx = this.StateManager.CreateTransaction())
            {
                result = await _leases.AddOrUpdateAsync(tx, resourceName, 
                s => new Lease // add
                {
                    LeaseId = leaseId,
                    ResourceName = resourceName,
                    ValidUntilUtc = DateTime.UtcNow.Add(_validity)
                }, 
                (s, original) => { // update
                    if ((original.LeaseId == leaseId) || original.ValidUntilUtc < DateTime.UtcNow.Add(_validity))
                    {
                        original.LeaseId = leaseId;
                        original.ResourceName = resourceName;
                        original.ValidUntilUtc = DateTime.UtcNow.Add(_validity);
                    }
                    return original;
                });

                await tx.CommitAsync();
            }

            acquired = leaseId == result.LeaseId;

            return new LeaseResult
            {
                Acquired = acquired,
                LeaseId = leaseId,
                ResourceName = resourceName,
                ValidUntilUtc = acquired ? result.ValidUntilUtc : DateTime.MinValue
            };
        }

        public async Task ReleaseLeaseAsync(string resourceName, string leaseId)
        {
            await EnsureLeases();

            using (var tx = this.StateManager.CreateTransaction())
            {
                var conditional = await _leases.TryGetValueAsync(tx, resourceName);
                if (conditional.HasValue)
                {
                    var original = conditional.Value;
                    if (original.LeaseId == leaseId)
                    {
                        await _leases.TryRemoveAsync(tx, leaseId);
                        await tx.CommitAsync();
                    }
                }                
            }
        }

        private async Task EnsureLeases()
        {
            if (_leases == null)
            {
                 _leases = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Lease>>("leases");
            }
        }

        protected override void InitializeStateSerializers()
        {
            this.StateManager.TryAddStateSerializer(new JsonStateSerializer<Lease>());
        }
    }
}
