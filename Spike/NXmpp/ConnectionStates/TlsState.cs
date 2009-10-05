using NXmpp.Protocol.Stream;
using NXmpp.Protocol.Tls;

namespace NXmpp.ConnectionStates {
	internal class TlsState : ConnectionStateBase {
		private ServerFeatures _serverFeatures;

		internal TlsState(IXmppContext context, ServerFeatures serverFeatures)
			: base(context) {
			_serverFeatures = serverFeatures;
		}

		internal override void Handle() {
			if (_serverFeatures.TLSSupport == ServerFeatures.TLSSupportType.Required ||
			    (_serverFeatures.TLSSupport == ServerFeatures.TLSSupportType.SupportedNotRequired && (Context.Settings.UseTLS == UseTLS.Always || Context.Settings.UseTLS == UseTLS.IfSupported))) {
				Context.OnConnectionStateChanged(ConnectionStateType.Securing);
				Context.Connection.Send(new StartTlsRequest());
				StartTlsResponse startTlsResponse = StartTlsResponse.CreateFromXml(Context.Connection.Receive());
				if (!startTlsResponse.Proceed) {
					//throw new InvalidOperationException("Attempt to start TLS negotiation failed");
				}
				Context.Connection.Secure(Context.Settings.TlsCertificateCallback);
				Context.Connection.InitializeStream();
				_serverFeatures = ServerFeatures.CreateFromXml(Context.Connection.Receive());
			}
			NextState = new CompressState(Context, _serverFeatures);
		}
	}
}