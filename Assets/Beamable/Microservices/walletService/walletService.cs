using Beamable.Server;

namespace Beamable.Microservices
{
	[Microservice("walletService")]
	public class walletService : Microservice
	{
		[ClientCallable]
		public void ServerCall()
		{
			// This code executes on the server.
		}
	}
}
