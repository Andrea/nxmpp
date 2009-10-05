using System;
using System.Net;

namespace NXmpp.Net {
	internal class XmppHost {
		internal XmppHost(string hostName, IPAddress ipAddress, UInt16 port) {
			HostName = hostName;
			IPAddress = ipAddress;
			Port = port;
		}
		internal string HostName { get; private set; }
		internal IPAddress IPAddress { get; private set; }
		internal ushort Port { get; private set; }
	}
}