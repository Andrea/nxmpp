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

namespace NXmpp {
	public class JId : IEquatable<JId> {

		internal JId(string user, string domain) : this(user, domain, null) {}

		internal JId(string user, string domain, string resource) {
			if (string.IsNullOrEmpty(user)) {
				throw new ArgumentNullException("user");
			}
			User = user;
			Domain = domain;
			Resource = resource;
		}

		public string User { get; private set; }

		public string Domain { get; private set; }

		public string Resource { get; private set; }

		public override string ToString() {
			string jid = User;
			if (!string.IsNullOrEmpty(Domain)) {
				jid += "@" + Domain;
			}
			if (!string.IsNullOrEmpty(Resource)) {
				jid += "/" + Resource;
			}
			return jid;
		}

		public static JId Parse(string jid) {
			string[] s = jid.Split('@');
			string user = s[0];
			string domain = null;
			string resource = null;
			if (s.Length == 2) {
				s = s[1].Split('/');
				domain = s[0];
				if (s.Length == 2) {
					resource = s[1];
				}
			}
			return new JId(user, domain, resource);
		}
		internal JId WithoutResource() {
			return new JId(User, Domain, null);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (JId)) return false;
			return Equals((JId) obj);
		}

		public bool Equals(JId other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.User, User) && Equals(other.Domain, Domain) && Equals(other.Resource, Resource);
		}

		public override int GetHashCode() {
			unchecked {
				int result = (User != null ? User.GetHashCode() : 0);
				result = (result*397) ^ (Domain != null ? Domain.GetHashCode() : 0);
				result = (result*397) ^ (Resource != null ? Resource.GetHashCode() : 0);
				return result;
			}
		}

		public static bool operator ==(JId left, JId right) {
			return Equals(left, right);
		}

		public static bool operator !=(JId left, JId right) {
			return !Equals(left, right);
		}
	}
}