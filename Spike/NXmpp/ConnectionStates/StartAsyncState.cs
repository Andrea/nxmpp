using System;

namespace NXmpp.ConnectionStates {
	internal class StartAsyncState : ConnectionStateBase {
		public StartAsyncState(IXmppContext context) : base(context) {}

		internal override void Handle() {
			Context.StartAsync();
			Context.InitializeStanzaHandlers();
			NextState = new ConnectedState(Context);
		}
	}
}