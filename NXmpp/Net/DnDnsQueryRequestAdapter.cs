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

using System.Collections.Generic;
using System.Net.Sockets;
using DnDns.Enums;
using DnDns.Query;
using DnDns.Records;

namespace NXmpp.Net
{
	internal class DnDnsQueryRequestAdapter : IDnsQueryRequest
	{
		#region IDnsQueryRequest Members

		public XmppSrvRecord[] GetXmppSrvRecords(string dnsServer, string host)
		{
			var request = new DnsQueryRequest();
			DnsQueryResponse response = request.Resolve(dnsServer, host, NsType.SRV, NsClass.INET, ProtocolType.Udp);

			var xmppSrvRecords = new List<XmppSrvRecord>();
			foreach (IDnsRecord answer in response.Answers)
			{
				var srvRecord = (SrvRecord) answer;
				xmppSrvRecords.Add(new XmppSrvRecord {HostEntry = System.Net.Dns.GetHostEntry(srvRecord.HostName), Port = srvRecord.Port});
			}
			return xmppSrvRecords.ToArray();
		}

		#endregion
	}
}