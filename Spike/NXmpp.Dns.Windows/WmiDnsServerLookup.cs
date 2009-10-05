using System;
using System.Collections.Generic;
using System.Management;
using System.Net;

namespace NXmpp.Dns.Windows {
	public class WmiDnsServerLookup : IDnsServerLookup {
		#region IDnsServerLookup Members

		public IPAddress[] GetDnsServers() {
			var mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection objCol = mgmt.GetInstances();

			//we may have more than one network adapter, each configured with dns servers.
			//we need to order the dns servers by the adapters IPConnectionMetric, which is the preferred order for querying against.
			var dnsHostsByConnectionMetric = new SortedList<UInt32, List<IPAddress>>(); 

			foreach (ManagementObject obj in objCol) {
				if ((bool) obj["IPEnabled"]) {
					var connectionMetric = ((UInt32) obj["IPConnectionMetric"]);
					var dnsItems = (string[]) obj["DNSServerSearchOrder"];
					if (dnsItems != null && dnsItems.Length > 0) {
						var ipAddresses = new List<IPAddress>();
						foreach (string dns in dnsItems) {
							ipAddresses.Add(IPAddress.Parse(dns));
						}
						dnsHostsByConnectionMetric.Add(connectionMetric, ipAddresses);
					}
				}
				obj.Dispose();
			}
			if(dnsHostsByConnectionMetric.Count == 0) {
				return null;
			}
			var dnsHosts = new List<IPAddress>();
			foreach (KeyValuePair<uint, List<IPAddress>> item in dnsHostsByConnectionMetric) {
				dnsHosts.AddRange(item.Value);
			}
			return dnsHosts.ToArray();
		}

		#endregion
	}
}