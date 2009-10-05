using System.Net;
using Xunit;

namespace NXmpp.Dns.Windows.Tests {
	public class WmiDnsServerLookupTests {
		[Fact]
		public void CanRetrieveDnsServers() {
			//this test assumes the system has a working network connection with dns servers setup either manually or via DHCP
			var dnsServerLookup = new WmiDnsServerLookup();
			IPAddress[] dnsServers = dnsServerLookup.GetDnsServers();
			Assert.NotNull(dnsServers);
		}
	}
}