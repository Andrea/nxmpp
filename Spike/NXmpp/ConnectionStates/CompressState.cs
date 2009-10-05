using NXmpp.Protocol.Stream;

namespace NXmpp.ConnectionStates {
	internal class CompressState : ConnectionStateBase {
		private readonly ServerFeatures _serverFeatures;

		internal CompressState(IXmppContext context, ServerFeatures serverFeatures)
			: base(context) {
			_serverFeatures = serverFeatures;
		}

		internal override void Handle() {
			NextState = new SaslAuthState(Context, _serverFeatures);
		}
	}
}