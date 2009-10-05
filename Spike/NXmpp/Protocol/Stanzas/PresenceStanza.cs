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

using System.Xml;

namespace NXmpp.Protocol.Stanzas {

	internal enum PresenceType {
		None,
		Probe,
		Unavailable,
		Subscribe,
		Subscribed,
		Unsunscribe,
		Unsubscribed,
		Error
	}

	internal enum PresenceAvailabilityStatus {
		None,
		Away,
		Chat,
		DoNotDisturb,
		ExtendedAway
	}

	internal class PresenceStanza : DirectedStanza {
		private readonly PresenceType presenceType;

		private readonly PresenceAvailabilityStatus presenceAvailabilityStatus;

		private readonly string detailedStatus;

		private PresenceStanza(JId from, JId to, string id, PresenceType presenceType, PresenceAvailabilityStatus presenceAvailabilityStatus, string detailStatus)
			: base(from, to, id) {
			this.presenceType = presenceType;
			this.presenceAvailabilityStatus = presenceAvailabilityStatus;
		}

		internal static PresenceStanza CreateInitial(JId jid, string id) {
			return CreateBroadcastGeneral(jid, id, PresenceAvailabilityStatus.None);
		}

		internal static PresenceStanza CreateBroadcastGeneral(JId from, string id, PresenceAvailabilityStatus presenceAvailabilityStatus) {
			return new PresenceStanza(from, null, id, PresenceType.None, presenceAvailabilityStatus, string.Empty);
		}

		internal static PresenceStanza CreateAcceptSubscription(JId to,string id) {
			return new PresenceStanza(null, to, id, PresenceType.Subscribed, PresenceAvailabilityStatus.None, string.Empty);
		}

		internal static PresenceStanza CreateCancelSubscription(JId to, string id) {
			return new PresenceStanza(null, to, id, PresenceType.Unsubscribed, PresenceAvailabilityStatus.None, string.Empty);
		}

		internal static PresenceStanza CreateBroadcastGeneral(JId from,string id, PresenceAvailabilityStatus presenceAvailabilityStatus, string detailedStatus) {
			return new PresenceStanza(from, null, id, PresenceType.None, presenceAvailabilityStatus, detailedStatus);
		}

		internal static PresenceStanza CreateBroadcastUnavilable(JId from, string id) {
			return new PresenceStanza(from, null, id, PresenceType.Unavailable, PresenceAvailabilityStatus.None, string.Empty);
		}
	}
}