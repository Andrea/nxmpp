using System.Collections;
using System.Collections.Generic;
using NXmpp.Net;

namespace NXmpp.ConnectionStates {
	internal class ConnectionStatesEnumerable : IEnumerable<ConnectionStateBase> {
		private readonly IXmppContext _context;

		internal ConnectionStatesEnumerable(IXmppContext context) {
			_context = context;
		}

		#region IEnumerable<ConnectionStateBase> Members

		public IEnumerator<ConnectionStateBase> GetEnumerator() {
			ConnectionStateBase connectionState = new TcpBindingState(_context, new XmppHostsLookup(), new XmppConnectionFactory());
			yield return connectionState;
			while (connectionState.NextState != null) {
				connectionState = connectionState.NextState;
				yield return connectionState;
			}
			//yield return new DisconnectedState(_context);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion
	}
}