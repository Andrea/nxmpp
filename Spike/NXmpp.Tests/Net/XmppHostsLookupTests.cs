using System.Net;
using System.Net.Sockets;
using Common.Logging;
using NXmpp.Dns.Windows;
using NXmpp.Net;
using Xunit;
using Common.Logging.Simple;

namespace NXmpp.Tests.Net {
	public class XmppHostsLookupTests {
		[Fact]
		public void When_lookup_gmail_xmpp_server_should_get_at_least_one_host() {
			//This test requires a working internet connection
			var xmppHostsLookup = new XmppHostsLookup();
			var dnsServerLookup = new WmiDnsServerLookup();
			var logger = new ConsoleOutLogger("XmppHostsLookupTests", LogLevel.All, true, true, true, "yyyy-MM-dd HH:MM:SS");
			XmppHost[] xmppHosts = xmppHostsLookup.GetXmppHosts(dnsServerLookup.GetDnsServers(), "gmail.com", logger);
			Assert.NotNull(xmppHosts);
			Assert.InRange(xmppHosts.Length, 1, int.MaxValue);
		}

		[Fact]
		public void When_lookup_internal_server_should_get_at_least_one_host() {
			//This test assumes a host on the internal network called winxmpp
			var xmppHostsLookup = new XmppHostsLookup();
			var dnsServerLookup = new WmiDnsServerLookup();
			var logger = new ConsoleOutLogger("XmppHostsLookupTests", LogLevel.All, true, true, true, "yyyy-MM-dd HH:MM:SS");
			XmppHost[] xmppHosts = xmppHostsLookup.GetXmppHosts(dnsServerLookup.GetDnsServers(), "winxmpp", logger);
			Assert.NotNull(xmppHosts);
			Assert.InRange(xmppHosts.Length, 1, int.MaxValue);
		}

		[Fact]
		public void When_dns_servers_unreachable_should_throw_SocketException() {
			var xmppHostsLookup = new XmppHostsLookup();
			var ipAddresses = new []{ IPAddress.Parse("1.1.1.1")}; //assuming there is no dns server at 1.1.1.1
			var logger = new ConsoleOutLogger("XmppHostsLookupTests", LogLevel.All, true, true, true, "yyyy-MM-dd HH:MM:SS");
			Assert.Throws<SocketException>( ()=> xmppHostsLookup.GetXmppHosts(ipAddresses, "host", logger));
		}
	}
}