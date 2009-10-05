using System.Linq;
using NXmpp.Protocol.Sasl;
using NXmpp.Protocol.Stream;
using NXmpp.Sasl;

namespace NXmpp.ConnectionStates {
	internal class SaslAuthState : ConnectionStateBase {
		private ServerFeatures _serverFeatures;

		internal SaslAuthState(IXmppContext context, ServerFeatures serverFeatures)
			: base(context) {
			_serverFeatures = serverFeatures;
		}

		internal override void Handle() {
			if (!_serverFeatures.Mechanisms.Contains("DIGEST-MD5")) {
				//throw new NotSupportedException("No supported auth mechanisms");
			}
			Context.OnConnectionStateChanged(ConnectionStateType.Authenticating);
			Context.Connection.Send(new DigestMD5SaslAuth());
			SaslChallenge saslChallenge = SaslChallenge.CreateFromXml(Context.Connection.Receive());
			var digestMD5Authentication = new DigestMD5Authentication(saslChallenge.ChallangeString);
			string response = digestMD5Authentication.GetDigestResponse(Context.Settings.UserJId.User, Context.Settings.Password, digestMD5Authentication.Realms[0], Context.Settings.UserJId.Domain);
			Context.Connection.Send(new SaslResponse(response));
			SaslAuthResult saslAuthResult = SaslAuthResult.CreateFromXml(Context.Connection.Receive());
			if (!saslAuthResult.Success) {
				StreamException.ThrowStreamException(saslAuthResult.FailReason);
			}
			Context.Connection.InitializeStream();
			_serverFeatures = ServerFeatures.CreateFromXml(Context.Connection.Receive());
			NextState = new BindResourceState(Context, _serverFeatures);
		}
	}
}