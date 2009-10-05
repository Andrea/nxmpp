using System;
using System.ComponentModel;
using NXmpp.Roster;
using System.Collections.Generic;

namespace NXmpp.ChatClient {
	internal class ContactsBindingList : BindingList<Contact> {

		private readonly Dictionary<JId, Contact> _contactsDictionary = new Dictionary<JId, Contact>();

		internal void HandleRosterItemChanged(RosterChangedEventArgs args) {
			if (args.ChangedType == RosterItemChangedType.Added) {
				{
					var contact = new Contact {JId = args.RosterItem.JId, Name = args.RosterItem.Name};
					_contactsDictionary.Add(contact.JId, contact);
					Add(contact);
				}
			}
			else if (args.ChangedType == RosterItemChangedType.Removed) {
				{
					var contact = _contactsDictionary[args.RosterItem.JId];
					_contactsDictionary.Remove(contact.JId);
					Remove(contact);
				}
			}
			else if (args.ChangedType == RosterItemChangedType.Updated) {
				{
					var contact = _contactsDictionary[args.RosterItem.JId];
					contact.Name = args.RosterItem.Name;
				}
			}
			else {
				throw new InvalidOperationException("Unknown RosterItemChangedType");
			}
		}
	}
}