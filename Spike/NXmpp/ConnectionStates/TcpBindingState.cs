using System;
using System.Net;
using System.Net.Sockets;
using NXmpp.Dns;
using NXmpp.Net;
using NXmpp.Protocol.Stream;

namespace NXmpp.ConnectionStates {
	internal class TcpBindingState : ConnectionStateBase {
		private readonly IXmppHostsLookup _hostsLookup;
		private readonly IXmppConnectionFactory _xmppConnectionFactory;

		internal TcpBindingState(IXmppContext context, IXmppHostsLookup hostsLookup, IXmppConnectionFactory xmppConnectionFactory) : base(context) {
			_hostsLookup = hostsLookup;
			_xmppConnectionFactory = xmppConnectionFactory;
		}

		internal override void Handle() {
			Context.OnConnectionStateChanged(ConnectionStateType.Connecting);
			IDnsServerLookup dnsServerLookup = Context.DnsServerLookupFactory.Create();
			IPAddress[] dnsServers = dnsServerLookup.GetDnsServers();
			if (dnsServers == null) {
				throw new InvalidOperationException("Could not find a DNS server");
			}
			XmppHost[] xmppHosts = _hostsLookup.GetXmppHosts(dnsServers, Context.Settings.UserJId.Domain, Context.Settings.Log);
			IXmppConnection xmppConnection = null;
			foreach(XmppHost xmppHost in xmppHosts) {
				xmppConnection = _xmppConnectionFactory.Create(xmppHost, new Version(1, 0), Context.Settings.Log);
				try {
					xmppConnection.Connect();
					break;
				}
				catch(Exception ex) {
					Context.Settings.Log.Warn(ex);
					xmppConnection.Dispose();
					xmppConnection = null;
				}
			}
			if(xmppConnection == null) {
				throw new TcpBindingException("Unable to connect to xmpp host");
			}
			Context.Connection = xmppConnection;
			Context.Connection.InitializeStream();
			ServerFeatures serverFeatures = ServerFeatures.CreateFromXml(Context.Connection.Receive());
			NextState = new TlsState(Context, serverFeatures);
		}
	}

	internal class TcpBindingException : ApplicationException {
		internal TcpBindingException(string message) : base(message){}
	}
}