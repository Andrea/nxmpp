using System.Net;
using Moq;
using NXmpp.ConnectionStates;
using NXmpp.Dns.Windows;
using Xunit;
using NXmpp.Net;
using Common.Logging;

namespace NXmpp.Tests.ConnectionStates {
	public class TcpBindingStateTests {
		[Fact]
		public void ahah() {
			var settings = new XmppClientSettings(new JId("user", "domain"), "password", (a, b, c) => true);
			var mockXmppContext = new Mock<IXmppContext>();
			var mockXmppHostLookup = new Mock<IXmppHostsLookup>();
			mockXmppHostLookup.Setup(x => x.GetXmppHosts(It.IsAny<IPAddress[]>(), It.IsAny<string>(), It.IsAny<ILog>())).Returns(new[] {new XmppHost("host", IPAddress.Parse("1.1.1.1"), 5269)});
			mockXmppContext.SetupGet(x => x.Settings).Returns(settings);
			mockXmppContext.SetupSet(x => x.Connection);
			var tcpBindingState = new TcpBindingState(mockXmppContext.Object, null, null);
			tcpBindingState.Handle();
			mockXmppHostLookup.VerifyAll();
			mockXmppContext.VerifyAll();
		}
	}
}