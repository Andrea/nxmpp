using NXmpp.Roster;
using System.Threading;

namespace NXmpp.ChatClient {
	internal class XmppClientContainer {
		private readonly XmppClient _xmppClient;
		private readonly SynchronizationContext _synchronizationContext;
		internal ContactsBindingList Contacts { get; private set; }

		internal XmppClientContainer(XmppClient xmppClient, SynchronizationContext synchronizationContext) {
			_xmppClient = xmppClient;
			_synchronizationContext = synchronizationContext;
			Contacts = new ContactsBindingList();
			_xmppClient.Roster.RosterChanged += Roster_RosterChanged;
		}

		private void Roster_RosterChanged(object sender, RosterChangedEventArgs e) {
			_synchronizationContext.Send(state => Contacts.HandleRosterItemChanged(e), null);
		}
	}
}