using System;
using System.Xml.Linq;
using Moq;
using NXmpp.Roster;
using Xunit;
using NXmpp.StanzaHandler;

namespace NXmpp.Tests.Roster {
	public class RosterManagerTests {
		private const string Path = "Roster";
		private readonly JId jid = new JId("user", "domain", "resource");
		readonly Mock<IXmppService> mockXmppService = new Mock<IXmppService>(MockBehavior.Loose);

		public RosterManagerTests() {
			mockXmppService.SetupGet(x => x.JId).Returns(jid);
		}

		[Fact]
		public void When_initializing_should_send_RosterGet() {
			XElement rosterGetExpected = XElement.Load(System.IO.Path.Combine(Path, "RosterGet.xml"));
			XElement rosterGetActual = null;
			mockXmppService.Setup(x => x.Send(It.IsAny<IStanzaHandler>(), It.IsAny<XElement>())).Callback((IStanzaHandler handler, XElement x) => rosterGetActual = x);
			var rosterManager = new RosterManager(mockXmppService.Object);
			rosterManager.Initialize();
			mockXmppService.VerifyAll();
			Assert.Equal(rosterGetExpected.ToString(SaveOptions.DisableFormatting), rosterGetActual.ToString(SaveOptions.DisableFormatting));
		}

		[Fact]
		public void When_initializing_should_fire_RosterItemChanged_event() {
			XElement responseElement = XElement.Load(System.IO.Path.Combine(Path, "RosterIqResult.xml"));
			var rosterManager = new RosterManager(mockXmppService.Object);
			bool rosterChangedFired = false;
			rosterManager.RosterChanged += (sender, e) => rosterChangedFired = true;
			rosterManager.Initialize();
			rosterManager.HandleIQStanza(responseElement);
			Assert.True(rosterChangedFired);
		}

		[Fact]
		public void After_initialization_should_send_initial_presence() {;
			XElement initialPresenceExpected = XElement.Load(System.IO.Path.Combine(Path, "InitialPresence.xml"));
			XElement initialPresenceActual = null;
			XElement responseElement = XElement.Load(System.IO.Path.Combine(Path, "RosterIqResult.xml"));
			mockXmppService.Setup(x => x.Send(It.IsAny<IStanzaHandler>(), It.IsAny<XElement>())).Callback((IStanzaHandler handler, XElement e) => initialPresenceActual = e);
			var rosterManager = new RosterManager(mockXmppService.Object);
			rosterManager.Initialize();
			rosterManager.HandleIQStanza(responseElement);
			Assert.Equal(initialPresenceExpected.ToString(SaveOptions.DisableFormatting), initialPresenceActual.ToString(SaveOptions.DisableFormatting));
		}

		[Fact]
		public void When_add_self_to_roster_should_throw() {
			XElement responseElement = XElement.Load("Roster\\RosterIqResult.xml");
			var rosterManager = new RosterManager(mockXmppService.Object);
			rosterManager.Initialize();
			rosterManager.HandleIQStanza(responseElement);
			Assert.Throws<InvalidOperationException>(() => rosterManager.Add(jid, "user", new[] {"none"}));
		}

		[Fact]
		public void When_add_to_roster_should_send_RosterSet() {
			XElement rosterSetExpected = XElement.Load("Roster\\RosterSet.xml");
			XElement rosterSetActual = null;
			mockXmppService.Setup(x => x.Send(It.IsAny<IStanzaHandler>(), It.IsAny<XElement>())).Callback((IStanzaHandler handler, XElement e) => rosterSetActual = e);
			var rosterManager = new RosterManager(mockXmppService.Object);
			rosterManager.Initialize();
			rosterManager.Add(new JId("user2", "domain"), "buddy", new[] {"Friends"});
			Assert.Equal(rosterSetExpected.ToString(SaveOptions.DisableFormatting), rosterSetActual.ToString(SaveOptions.DisableFormatting));
		}

		public void When_roster_push_received_should_fire_fire_RosterItemChanged_event() {
		}
	}
}