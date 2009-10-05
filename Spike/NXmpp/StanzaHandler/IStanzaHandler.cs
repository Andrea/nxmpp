using System.Xml.Linq;
namespace NXmpp.StanzaHandler {
	internal interface IStanzaHandler {
		string HandlerKey { get; }
		void Initialize();
		string Namespace { get; }
		void HandleIQStanza(XElement stanza);
	}
}