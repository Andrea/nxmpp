using System.Net;

namespace NXmpp.Dns {
	public interface IDnsServerLookup {
		/// <summary>
		/// Gets the dns servers registered to all network adaptors in the system.
		/// </summary>
		/// <returns>An array of dns server ip addresses. NULL if no dns servers found.</returns>
		IPAddress[] GetDnsServers();
	}
}