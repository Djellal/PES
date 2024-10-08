using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Pes
{
    public class CustomCircuitHandler : CircuitHandler
    {
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Circuit closed for {circuit.Id}");
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Circuit opened for {circuit.Id}");
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Connection up for {circuit.Id}");
            return base.OnConnectionUpAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Connection down for {circuit.Id}");
            return base.OnConnectionDownAsync(circuit, cancellationToken);
        }
    }

}
