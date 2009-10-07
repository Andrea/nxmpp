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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Common.Logging;
using NXmpp.Dns;

namespace NXmpp.Net
{
	internal static class XmppDns
	{
		/// <summary>
		/// Gets a array of XmppHosts for a given domain ordered by weight. First by looking up the srv records for the domain, then adding the domain as the host.
		/// Does not attempt to resolve IP addresses.
		/// </summary>
		/// <param name="dnsServerLookupFactory"></param>
		/// <param name="dnsQueryRequestFactory"></param>
		/// <param name="domain"></param>
		/// <param name="log"></param>
		/// <returns></returns>
		public static XmppHost[] GetHosts(IDnsServerLookupFactory dnsServerLookupFactory, IDnsQueryRequestFactory dnsQueryRequestFactory, string domain, ILog log)
		{
			IDnsServerLookup dnsServerLookup = dnsServerLookupFactory.Create();
			IPAddress[] nameServers = dnsServerLookup.GetDnsServers();

			var xmppHosts = new SortedList<int, XmppHost>();
			string srvRecord = "_xmpp-server._tcp." + domain;

			IDnsQueryRequest dnsQueryRequest = dnsQueryRequestFactory.Create();

			//loop through each dns server and attempt to resolve for xmpp server hosts. Loop stops when a dns server is reachable and returns records
			foreach (IPAddress nameServerAddress in nameServers)
			{
				XmppSrvRecord[] xmppSrvRecords;
				try
				{
					xmppSrvRecords = dnsQueryRequest.GetXmppSrvRecords(nameServerAddress.ToString(), srvRecord);
				}
				catch (SocketException ex)
				{
					log.Error("Dns server unreachable. " + ex);
					continue;
				}
				if (xmppSrvRecords == null || xmppSrvRecords.Length <= 0) continue;
				foreach (XmppSrvRecord xmppSrvRecord in xmppSrvRecords)
				{
					xmppHosts.Add(xmppSrvRecord.Weight, new XmppHost(xmppSrvRecord.HostName, xmppSrvRecord.Port));
				}
				break;
			}
			xmppHosts.Add(99999999, new XmppHost(domain, 5222));
			return xmppHosts.Values.ToArray();
		}
	}
}