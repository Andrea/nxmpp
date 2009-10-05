using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NXmpp.Protocol;
using NXmpp.Protocol.Iq;
using NXmpp.Protocol.Stream;

namespace NXmpp.Roster {
	internal class RosterInitializeIqResponse : IqResponse {
		internal List<RosterItem> Items { get; private set; }

		protected override void ParseInner(XElement element) {
			if (element.Name != XName.Get("query", NamespaceStrings.XmppRoster)) {
				throw new XmlNotWellFormedException("query element expected");
			}
			Items = new List<RosterItem>();
			foreach (XElement item in element.Descendants(XName.Get("item", NamespaceStrings.XmppRoster))) {
				var rosterItem = new RosterItem(
					JId.Parse(item.Attribute("jid").Value),
					item.Attribute("name").Value,
					(RosterItemSubscriptionType) Enum.Parse(typeof (RosterItemSubscriptionType), item.Attribute("subscription").Value, true)
					);
				foreach (XElement group in item.Descendants(XName.Get("group", NamespaceStrings.XmppRoster))) {
					rosterItem.Groups.Add(group.Value);
				}
				Items.Add(rosterItem);
			}
		}
	}
}