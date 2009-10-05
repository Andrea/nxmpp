namespace NXmpp.ConnectionStates {
	internal abstract class ConnectionStateBase {
		protected ConnectionStateBase(IXmppContext context) {
			Context = context;
		}

		protected IXmppContext Context { get; private set; }

		internal ConnectionStateBase NextState { get; set; }

		internal abstract void Handle();
	}
}