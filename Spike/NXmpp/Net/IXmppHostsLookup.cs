using System.Net;
using Common.Logging;

namespace NXmpp.Net {
	internal interface IXmppHostsLookup {
		XmppHost[] GetXmppHosts(IPAddress[] dnsServers, string domain, ILog log);
	}
}
