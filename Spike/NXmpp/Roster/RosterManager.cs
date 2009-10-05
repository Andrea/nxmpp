#region License

// Copyright 2009 Damian Hickey
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may
// not use this file except in compliance with the License. You may obtain a
// copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License. 

#endregion

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NXmpp.Extensions;
using NXmpp.Protocol.Stanzas;
using NXmpp.StanzaHandler;

namespace NXmpp.Roster {
	public delegate void RosterChanged(object sender, RosterChangedEventArgs e);

	internal class RosterManager : StanzaHandlerBase, IRoster {
		public override string Namespace {
			get { return "jabber:iq:roster"; }
		}
		private readonly Dictionary<JId, RosterItem> _items = new Dictionary<JId, RosterItem>();
		private int id = 1;

		internal RosterManager(IXmppService xmppService)
			: base(xmppService) {}

		public override string HandlerKey {
			get { return "roster"; }
		}

		#region IRoster Members

		public event EventHandler<RosterChangedEventArgs> RosterChanged;

		#endregion

		public override void Initialize() {
			SendRosterInitialGet();
		}

		private void AddOrUpdate(RosterItem rosterItem) {
			if (_items.ContainsKey(rosterItem.JId)) {
				_items[rosterItem.JId] = rosterItem;
				RosterChanged.Fire(this, new RosterChangedEventArgs(RosterItemChangedType.Updated, rosterItem));
			}
			else {
				_items.Add(rosterItem.JId, rosterItem);
				RosterChanged.Fire(this, new RosterChangedEventArgs(RosterItemChangedType.Added, rosterItem));
			}
		}

		private void Remove(RosterItem rosterItem) {
			if (_items.ContainsKey(rosterItem.JId)) {
				_items.Remove(rosterItem.JId);
				RosterChanged.Fire(this, new RosterChangedEventArgs(RosterItemChangedType.Updated, rosterItem));
			}
			else {
				_items.Add(rosterItem.JId, rosterItem);
				RosterChanged.Fire(this, new RosterChangedEventArgs(RosterItemChangedType.Added, rosterItem));
			}
		}

		public void Add(JId jid, string name, string[] groups) {
			if (jid == JId) {
			    throw new InvalidOperationException("You cannot add yourself to your roster");
			}
			if (_items.ContainsKey(jid)) {
			    return; //TODO what should happen here? Exception?
			}
			var content = new XElement(XName.Get("item",Namespace) , new XAttribute("jid", jid), new XAttribute("name", name));
			if(groups != null && groups.Length > 0) {
				foreach (string group in groups) {
					content.Add(new XElement(XName.Get("group", Namespace), group));
				}
			}
			SendIQ(new IQRequest("query", new []{ content },  IQRequestType.Set, null));
		}

		private void SendRosterInitialGet() {
			SendIQ(new IQRequest("query", IQRequestType.Get, OnRosterInitialResult));
		}

		private void OnRosterInitialResult(IQResponse response) {
			if(response.ResponseType == IQResponseType.Result) {
				foreach (var itemElement in response.Content) {
					var jid = JId.Parse(itemElement.Attribute("jid").Value);
					string name = itemElement.Attribute("name") != null ? itemElement.Attribute("name").Value : null;
					var subscriptionType = itemElement.Attribute("subscription") != null ? (RosterItemSubscriptionType)Enum.Parse(typeof(RosterItemSubscriptionType), itemElement.Attribute("subscription").Value, true) : RosterItemSubscriptionType.None;
					var rosterItem = new RosterItem(jid, name, subscriptionType);
					foreach (var groupElement in itemElement.Descendants(XName.Get("group", Namespace))) {
						rosterItem.Groups.Add(groupElement.Value);
					}
					AddOrUpdate(rosterItem);
				}
				SendPresence(null, null);
			} //TODO: handle error
		}

		protected override void HandleIQStanzaPush(XElement iqStanza) {
			throw new NotImplementedException();
		}
	}
}