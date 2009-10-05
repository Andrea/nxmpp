using System;
using Common.Logging;

namespace NXmpp.Net {
	internal class XmppConnectionFactory : IXmppConnectionFactory {
		#region IXmppConnectionFactory Members

		public IXmppConnection Create(XmppHost xmppHost, Version protocolVersion, ILog log) {
			return new XmppConnection(xmppHost, protocolVersion, log);
		}

		#endregion
	}
}