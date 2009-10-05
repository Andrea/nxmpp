using System;

namespace NXmpp.Roster {
	public class RosterChangedEventArgs : EventArgs {
		internal RosterChangedEventArgs(RosterItemChangedType changedType, RosterItem rosterItem) {
			RosterItem = rosterItem;
			ChangedType = changedType;
		}

		/// <summary>
		/// The RosterItem that has been added/updated/removed
		/// </summary>
		public RosterItem RosterItem { get; private set; }

		/// <summary>
		/// The RosterItem changed type.
		/// </summary>
		public RosterItemChangedType ChangedType { get; private set; }
	}
}