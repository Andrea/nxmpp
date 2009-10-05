using System;

namespace NXmpp.Roster {
	public interface IRoster {
		event EventHandler<RosterChangedEventArgs> RosterChanged;
	}
}