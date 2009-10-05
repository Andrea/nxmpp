using System;
using Common.Logging;

namespace NXmpp.Net {
	internal interface IXmppConnectionFactory {
		IXmppConnection Create(XmppHost xmppHost, Version protocolVersion, ILog log);
	}
}