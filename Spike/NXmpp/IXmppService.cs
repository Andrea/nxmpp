using System.Xml.Linq;
using NXmpp.StanzaHandler;

namespace NXmpp {
	internal interface IXmppService {
		JId JId { get; }
		void Send(IStanzaHandler sender, XElement element);
	}
}