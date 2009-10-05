using NXmpp.Dns;
using NXmpp.Net;

namespace NXmpp {
	internal interface IXmppContext {
		XmppClientSettings Settings { get; }
		IXmppConnection Connection { get; set; }
		IXmppConnectionFactory ConnectionFactory { get; }
		IDnsServerLookupFactory DnsServerLookupFactory { get; }
		JId JId { get; set; }
		void OnConnectionStateChanged(ConnectionStateType e);
		void StartAsync();
		void InitializeStanzaHandlers();
	}
}