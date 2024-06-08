using Beamable.Server;

namespace Beamable.Microservices
{
	[Microservice("LeaderboardService")]
	public class LeaderboardService : Microservice
	{
		[ClientCallable]
		public void ServerCall()
		{
			// This code executes on the server.
		}
	}
}
