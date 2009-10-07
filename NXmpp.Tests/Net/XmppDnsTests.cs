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
using System.Net.Sockets;
using Common.Logging;
using Moq;
using NXmpp.Dns;
using NXmpp.Dns.Windows;
using NXmpp.Net;
using Xunit;

namespace NXmpp.Tests.Net
{
	public class XmppDnsTests
	{
		private readonly ILog _logger;

		public XmppDnsTests()
		{
			_logger = new Mock<ILog>(MockBehavior.Loose).Object;
		}

		[Fact]
		public void Get_hosts_should_call_dependency_factory_methods()
		{
			var dnsServerLookupFactoryMock = new Mock<IDnsServerLookupFactory>();
			dnsServerLookupFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsServerLookup>().Object);

			var dnsQueryRequestFactoryMock = new Mock<IDnsQueryRequestFactory>();
			dnsQueryRequestFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsQueryRequest>(MockBehavior.Loose).Object);

			try
			{
				XmppDns.GetHosts(dnsServerLookupFactoryMock.Object, dnsQueryRequestFactoryMock.Object, "domain", _logger);
			}
			catch {}
			dnsServerLookupFactoryMock.Verify(m => m.Create());
			dnsQueryRequestFactoryMock.Verify(m => m.Create());
		}

		[Fact]
		public void When_no_dns_servers_respond_should_return_domain_as_host()
		{
			var dnsServerLookupFactoryMock = new Mock<IDnsServerLookupFactory>();
			dnsServerLookupFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsServerLookup>().Object);

			var dnsQueryRequestFactoryMock = new Mock<IDnsQueryRequestFactory>();
			dnsQueryRequestFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsQueryRequest>(MockBehavior.Loose).Object);

			const string domain = "domain.com";
			XmppHost[] xmppHosts = XmppDns.GetHosts(dnsServerLookupFactoryMock.Object, dnsQueryRequestFactoryMock.Object, domain, _logger);

			Assert.NotNull(xmppHosts);
			Assert.Equal(1, xmppHosts.Length);
			Assert.Equal(domain, xmppHosts[0].HostName);
		}

		[Fact]
		public void When_srv_query_returns_response_should_return_XmppHosts() //test requires internet connection.
		{
			const string domain = "gmail.com";

			XmppHost[] xmppHosts = XmppDns.GetHosts(new WmiDnsServerLookupFactory(), new DnDnsQueryRequestFactory(), domain, _logger);
			Assert.NotNull(xmppHosts);
			Assert.True(xmppHosts.Length > 1);
			Assert.Equal(domain, xmppHosts[xmppHosts.Length - 1].HostName);
		}
	}
}