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
using Common.Logging;

namespace NXmpp.Net
{
	/// <summary>
	/// In order to support messaging between users through federated networks, the domains of those networks require SRV record.
	/// 
	/// For example: http://www.google.com/support/a/bin/answer.py?hl=en&answer=34143
	/// </summary>

	internal interface IXmppHostsLookup
	{
		/// <summary>
		/// Gets a array of XmppHosts for a given domain. First by looking up the srv records for the domain, and if not found, then assumes the domain is the host.
		/// </summary>
		/// <param name="dnsServers">Array of dns servers to query</param>
		/// <param name="domain">The domain.</param>
		/// <param name="log">Logger</param>
		/// <returns></returns>
		XmppHost[] GetXmppHosts(IPAddress[] dnsServers, string domain, ILog log);
	}
}