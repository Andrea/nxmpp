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

using System.Net;

namespace NXmpp.Net
{
	internal class XmppHost
	{
		internal XmppHost(string hostName, IPAddress ipAddress, UInt16 port)
		{
			HostName = hostName;
			IPAddress = ipAddress;
			Port = port;
		}

		internal string HostName { get; private set; }
		internal IPAddress IPAddress { get; private set; }
		internal ushort Port { get; private set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (XmppHost)) return false;
			return Equals((XmppHost) obj);
		}

		public bool Equals(XmppHost other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.HostName, HostName) && Equals(other.IPAddress, IPAddress) && other.Port == Port;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = (HostName != null ? HostName.GetHashCode() : 0);
				result = (result*397) ^ (IPAddress != null ? IPAddress.GetHashCode() : 0);
				result = (result*397) ^ Port.GetHashCode();
				return result;
			}
		}

		public static bool operator ==(XmppHost left, XmppHost right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(XmppHost left, XmppHost right)
		{
			return !Equals(left, right);
		}
	}
}