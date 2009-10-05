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
using System.Net;
using System.Net.Sockets;
using Common.Logging;
using NXmpp.Dns;

namespace NXmpp.Net
{
	internal static class XmppDns
	{
		/// <summary>
		/// Gets a array of XmppHosts for a given domain. First by looking up the srv records for the domain, and if not found, then assumes the domain is the host.
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
			var xmppHosts = new List<XmppHost>();
			xmppHosts.AddRange(GetHostsBySrvRecord(nameServers, dnsQueryRequestFactory, "_xmpp-server._tcp." + domain, log));
			xmppHosts.AddRange(GetHostsBySrvRecord(nameServers, dnsQueryRequestFactory, "_jabber._tcp." + domain, log));
			return xmppHosts.ToArray();
		}

		private static XmppHost[] GetHostsBySrvRecord(IEnumerable<IPAddress> nameServers, IDnsQueryRequestFactory dnsQueryRequestFactory, string srvRecord, ILog log)
		{
			bool dnsServerWasReached = false;
			IDnsQueryRequest dnsQueryRequest = dnsQueryRequestFactory.Create();
			//loop through each dns server and attemp
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
				dnsServerWasReached = true;
				if (xmppSrvRecords == null || xmppSrvRecords.Length <= 0) continue;
				var xmppHosts = new List<XmppHost>();
				foreach (XmppSrvRecord xmppSrvRecord in xmppSrvRecords)
				{
					try
					{
						foreach (IPAddress ipAddress in xmppSrvRecord.HostEntry.AddressList)
						{
							xmppHosts.Add(new XmppHost(xmppSrvRecord.HostEntry.HostName, ipAddress, xmppSrvRecord.Port));
						}
					}
					catch (SocketException ex)
					{
						if (ex.SocketErrorCode == SocketError.HostNotFound)
						{
							log.Warn(ex);
						}
						else
						{
							throw;
						}
					}
					catch (Exception ex)
					{
						log.Error(ex);
						throw;
					}
				}
				if (xmppHosts.Count == 0)
				{
					throw new SocketException(11001); //Host not found
				}
				return xmppHosts.ToArray();
			}
			if (!dnsServerWasReached)
			{
				throw new SocketException(10060);
			}
			return new XmppHost[] {};
		}
	}
}