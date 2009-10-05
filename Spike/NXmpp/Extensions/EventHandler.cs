using System;

namespace NXmpp.Extensions {
	public static class EventHandlerExtensions {
		public static void Fire<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e) where TEventArgs : EventArgs {
			EventHandler<TEventArgs> eventHandlerTemp = eventHandler;
			if (eventHandlerTemp != null) {
				eventHandlerTemp(sender, e);
			}
		}
	}
}