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
		public void When_no_dns_servers_found_should_throw()
		{
			var dnsServerLookupFactoryMock = new Mock<IDnsServerLookupFactory>();
			dnsServerLookupFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsServerLookup>().Object);

			var dnsQueryRequestFactoryMock = new Mock<IDnsQueryRequestFactory>();
			dnsQueryRequestFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsQueryRequest>(MockBehavior.Loose).Object);

			Assert.Throws<SocketException>(() => XmppDns.GetHosts(dnsServerLookupFactoryMock.Object, dnsQueryRequestFactoryMock.Object, "domain", _logger));
		}

		[Fact]
		public void When_all_dns_servers_unreachable_should_throw()
		{
			var dnsServerLookupFactoryMock = new Mock<IDnsServerLookupFactory>();
			dnsServerLookupFactoryMock.Setup(m => m.Create()).Returns(() =>
			                                                          {
			                                                          	var dnsServerLookupMock = new Mock<IDnsServerLookup>();
			                                                          	dnsServerLookupMock.Setup(m => m.GetDnsServers()).Returns(new[] {IPAddress.Parse("127.0.0.1")});
			                                                          	return dnsServerLookupMock.Object;
			                                                          });

			var dnsQueryRequestFactoryMock = new Mock<IDnsQueryRequestFactory>();
			dnsQueryRequestFactoryMock.Setup(m => m.Create()).Returns(() =>
			                                                          {
			                                                          	var dnsQueryRequestMock = new Mock<IDnsQueryRequest>();
			                                                          	dnsQueryRequestMock.Setup(m => m.GetXmppSrvRecords(It.IsAny<string>(), It.IsAny<string>()))
			                                                          		.Throws<SocketException>();
			                                                          	return dnsQueryRequestMock.Object;
			                                                          });

			Assert.Throws<SocketException>(() => XmppDns.GetHosts(dnsServerLookupFactoryMock.Object, dnsQueryRequestFactoryMock.Object, "domain", _logger));
		}

		[Fact]
		public void When_srv_query_returns_response_should_return_XmppHosts()
		{
			const string dnsServerAddress = "127.0.0.1";
			const string domain = "domain.com";
			var xmppHost = new XmppHost("xmpp.domain.com", IPAddress.Parse("127.0.0.2"), 5129);

			var dnsServerLookupFactoryMock = new Mock<IDnsServerLookupFactory>();
			dnsServerLookupFactoryMock.Setup(m => m.Create()).Returns(() =>
			                                                          {
			                                                          	var dnsServerLookupMock = new Mock<IDnsServerLookup>();
			                                                          	dnsServerLookupMock.Setup(m => m.GetDnsServers()).Returns(new[] {IPAddress.Parse(dnsServerAddress)});
			                                                          	return dnsServerLookupMock.Object;
			                                                          });

			var dnsQueryRequestFactoryMock = new Mock<IDnsQueryRequestFactory>();
			var xmppSrvRecord = new XmppSrvRecord {HostEntry = new IPHostEntry {AddressList = new[] {xmppHost.IPAddress}, HostName = xmppHost.HostName}, Port = xmppHost.Port};

			var dnsQueryRequestMock = new Mock<IDnsQueryRequest>();
			dnsQueryRequestMock.Setup(m => m.GetXmppSrvRecords(It.IsAny<string>(), "_xmpp-server._tcp." + domain)).Returns(() => new[] {xmppSrvRecord});

			dnsQueryRequestFactoryMock.Setup(m => m.Create()).Returns(() => dnsQueryRequestMock.Object);
			XmppHost[] xmppHosts = XmppDns.GetHosts(dnsServerLookupFactoryMock.Object, dnsQueryRequestFactoryMock.Object, domain, _logger);
			Assert.NotNull(xmppHosts);
			Assert.Equal(1, xmppHosts.Length);
			Assert.Equal(xmppHost, xmppHosts[0]);
		}

		[Fact]
		public void When_query_for_gmail_with_should_return_XmppHosts() //test requires internet connection.
		{
			//Todo: operation is too slow, investigate.
			const string domain = "gmail.com";
			XmppHost[] xmppHosts = XmppDns.GetHosts(new WmiDnsServerLookupFactory(), new DnDnsQueryRequestFactory(), domain, _logger);
			Assert.NotNull(xmppHosts);
			Assert.InRange(xmppHosts.Length, 1, int.MaxValue);
		}
	}
}