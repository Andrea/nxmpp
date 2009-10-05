using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using DnDns.Enums;
using DnDns.Query;
using DnDns.Records;
using Common.Logging;

namespace NXmpp.Net {
	internal class XmppHostsLookup : IXmppHostsLookup {
		#region IXmppHostsLookup Members


		public XmppHost[] GetXmppHosts(IPAddress[] dnsServers, string domain, ILog log) {
			var dnsQueryRequest = new DnsQueryRequest();
			string record = "_xmpp-server._tcp." + domain;
			bool dnsServerWasReached = false;
			foreach (IPAddress address in dnsServers) {
				DnsQueryResponse queryResponse = null;
				try {
					queryResponse = dnsQueryRequest.Resolve(address.ToString(), record, NsType.SRV, NsClass.INET, ProtocolType.Udp);
				}
				catch(SocketException ex) {
					log.Error("Dns server unreachable. " + ex);
					continue;
				}
				dnsServerWasReached = true;
				if (queryResponse == null || queryResponse.Answers == null || queryResponse.Answers.Length <= 0) continue;
				var hosts = new List<XmppHost>();
				foreach (IDnsRecord answer in queryResponse.Answers) {
					var srvRecord = (SrvRecord) answer;
					try {
						IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(srvRecord.HostName);
						foreach (IPAddress ipAddress in ipHostEntry.AddressList) {
							hosts.Add(new XmppHost(ipHostEntry.HostName, ipAddress, srvRecord.Port));
						}
					}
					catch (SocketException ex) {
						if (ex.SocketErrorCode == SocketError.HostNotFound) {
							log.Warn(ex);
						}
						else {
							throw;
						}
					}
					catch (Exception ex) {
						log.Error(ex);
						throw;
					}
				}
				if(hosts.Count == 0) {
					throw new SocketException(11001); //Host not found
				}
				return hosts.ToArray();
			}
			if (!dnsServerWasReached) {
				throw new SocketException(10060);
			}
			//couldn't locate the service via dns, assume the domain is the host.
			try {
				IPHostEntry defaultHostEntry = System.Net.Dns.GetHostEntry(domain);
				var hosts = new List<XmppHost>();
				foreach (IPAddress ipAddress in defaultHostEntry.AddressList) {
					hosts.Add(new XmppHost(defaultHostEntry.HostName, ipAddress, 5222)); //5222 default XmppPort
				}
				return hosts.ToArray();
			}
			catch(SocketException ex) {
				log.Error(ex);
				throw;
			}
		}

		#endregion
	}
}