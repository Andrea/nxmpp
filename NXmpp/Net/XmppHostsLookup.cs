using System;
using System.Collections.Generic;
using System.Net;

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
using Common.Logging;

namespace NXmpp.Net
{
	internal class XmppHostsLookup : IXmppHostsLookup
	{
		private readonly IDnsSrvResolver _dnsSrvResolver;

		internal XmppHostsLookup(IDnsSrvResolver dnsSrvResolver)
		{
			_dnsSrvResolver = dnsSrvResolver;
		}

		#region IXmppHostsLookup Members

		///<inheritdoc/>
		public XmppHost[] GetXmppHosts(IPAddress[] dnsServers, string domain, ILog log)
		{
			var xmppHosts = new List<XmppHost>();
			xmppHosts.AddRange(GetXmppHostsBySrvRecord(dnsServers, "_xmpp-server._tcp." + domain, log));
			xmppHosts.AddRange(GetXmppHostsBySrvRecord(dnsServers, "_jabber._tcp." + domain, log));
			return xmppHosts.ToArray();
		}

		#endregion

		private XmppHost[] GetXmppHostsBySrvRecord(IEnumerable<IPAddress> dnsServers, string srvRecord, ILog log)
		{
			return null;
		}
	}
}