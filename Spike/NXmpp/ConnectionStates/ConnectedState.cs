namespace NXmpp.ConnectionStates {
	internal class ConnectedState : ConnectionStateBase {
		internal ConnectedState(IXmppContext connectionContext) : base(connectionContext) {}

		internal override void Handle() {
			Context.OnConnectionStateChanged(ConnectionStateType.Connected);
			return;
		}
	}
}