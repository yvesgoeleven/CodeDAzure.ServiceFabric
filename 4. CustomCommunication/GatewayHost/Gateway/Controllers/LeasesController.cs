using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Contracts;
using Microsoft.ServiceFabric.Services;

namespace Gateway.Controllers
{
    public class LeasesController : ApiController
    {
        // Get gateway/leases
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Status: OK");
            response.Content = new StringContent("Status: OK", Encoding.Unicode);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromSeconds(1)
            };
            return response;
        }

        // POST gateway/leases/{resource}/{leaseId}
        public async Task<LeaseResult> Post(string resource, string leaseId)
        {
            var leaseManagerUri = new Uri("fabric:/LeaseManagerHost/LeaseManager");
            var leaseClient = ServiceProxy.Create<ILeaseManager>((long)1, leaseManagerUri);

            Trace.WriteLine($"Trying to acquire lease {leaseId} on resource {resource}");

            var lease = await leaseClient.AcquireLeaseAsync(resource, leaseId);

            Trace.WriteLine($"Lease on resource {resource} {(lease.Acquired ? "acquired" : "not acquired")} ");

            return lease;
        }

        // DELETE gateway/leases/{resource}/{leaseId}
        public async Task Delete(string resource, string leaseId)
        {
            var leaseManagerUri = new Uri("fabric:/LeaseManagerHost/LeaseManager");
            var leaseClient = ServiceProxy.Create<ILeaseManager>((long)1, leaseManagerUri);

            Trace.WriteLine($"Trying to release lease {leaseId} on resource {resource}");

            await leaseClient.ReleaseLeaseAsync(resource, leaseId);

            Trace.WriteLine($"Lease {leaseId} on resource {resource} released");
        }
       
    }
}