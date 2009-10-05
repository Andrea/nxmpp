//#region License

//// Copyright 2009 Damian Hickey
//// 
//// Licensed under the Apache License, Version 2.0 (the "License"); you may
//// not use this file except in compliance with the License. You may obtain a
//// copy of the License at 
//// 
//// http://www.apache.org/licenses/LICENSE-2.0 
//// 
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
//// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License. 

//#endregion

//using System;
//using System.ComponentModel;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Xml;
//using Common.Logging;
//using Common.Logging.Simple;
//using NXmpp.Protocol.Stream;
//using Xunit;

//namespace NXmpp.Tests {
//    public class XmppClientTests {
//        private readonly ILog log;
//        private readonly Server server;
//        private readonly Socket socket;
//        private XmlReader xmlReader;

//        public XmppClientTests() {
//            log = new ConsoleOutLogger("XmppConnectionTests", LogLevel.All, true, true, true, "DDDD-MM-YY HH:mm:SS");
//            server = new Server(log);
//            server.Start();
//        }

//        public void Dispose() {
//            socket.Close();
//            server.Stop();
//        }

//        //[Fact]
//        //public void When_server_does_not_respond_within_timeout_should_throw() {
//        //    var mockXmppConnection = new Mock<IXmppConnection>(MockBehavior.Loose);
//        //    using (var xmppClient = new XmppClient("localhost", 5000, new NetworkCredential("test", "test"), (certificate, chain, sslPolicyErrors) => true, UseTLS.IfSupported, 100, log, new Version(1, 0), mockXmppConnection.Object)) {
//        //        Assert.Throws<IOException>(xmppClient.Connect);
//        //    }
//        //}

//        //[Fact]
//        //public void When_server_does_respond_within_timeout_should_not_throw() {
//        //    using (var xmppClient = new XmppClient("localhost", 5000, new NetworkCredential("test", "test"), (certificate, chain, sslPolicyErrors) => true, UseTLS.IfSupported, 1000, log)) {
//        //        server.MessageReceived += (sender, e) => {
//        //                                    log.Trace("S REC:" + e.Value);
//        //                                    server.WriteStream(1);
//        //                                    //server.WriteFeatures();
//        //                                  };
//        //        Assert.DoesNotThrow(xmppClient.Connect);
//        //    }
//        //}

//        //[Fact]
//        //public void When_server_version_major_is_greater_than_one_lower_should_throw_UnsupportedVersion() {
//        //    using (var xmppClient = new XmppClient("localhost", 5000, new NetworkCredential("test", "test"), (certificate, chain, sslPolicyErrors) => true, UseTLS.IfSupported, 1000, log)) {
//        //        server.MessageReceived += (sender, e) => {
//        //                                    log.Trace("S REC:" + e.Value);
//        //                                    server.WriteStream(3);
//        //                                  };
//        //        Assert.Throws<UnsupportedVersionException>(xmppClient.Connect);
//        //    }
//        //}


//        [Fact]
//        public void StupidTest2() {
//            using (var xmppClient = new XmppClient("winxmpp", 5222, new NetworkCredential("test", "test"), (certificate, chain, sslPolicyErrors) => true, UseTLS.IfSupported, log)) {
//                var manualResetEvent = new ManualResetEvent(false);
//                AsyncCompletedEventArgs asyncCompletedEventArgs;
//                xmppClient.ConnectCompleted += (sender, e) => {
//                                                asyncCompletedEventArgs = e;
//                                                manualResetEvent.Set();
//                                               };
//                xmppClient.ConnectAsync();
//                manualResetEvent.WaitOne(1000000);
//            }
//        }

//        [Fact]
//        public void When_incorrect_credentials_supplied_should_throw() {
//            using (var xmppClient = new XmppClient("winxmpp", 5222, new NetworkCredential("test", "test2"), (certificate, chain, sslPolicyErrors) => true, UseTLS.IfSupported, log)) {
//                Assert.Throws<NotAuthorizedException>(xmppClient.ConnectAsync);
//            }
//        }
//    }

//    internal class Server {
//        private readonly ManualResetEvent clientConnectedEvent = new ManualResetEvent(false);
//        private readonly ILog log;
//        private readonly byte[] readBuffer = new byte[65536];
//        private TcpClient client;
//        private TcpListener listener;

//        internal Server(ILog log) {
//            this.log = log;
//        }

//        internal NetworkStream NetworkStream { get; private set; }

//        internal WaitHandle Start() {
//            listener = new TcpListener(IPAddress.Loopback, 5000);
//            listener.Start();
//            listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
//            return clientConnectedEvent;
//        }

//        private void AcceptTcpClientCallback(IAsyncResult asyncResult) {
//            client = listener.EndAcceptTcpClient(asyncResult);
//            NetworkStream = client.GetStream();
//            listener.Stop();
//            clientConnectedEvent.Set();
//            NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, ReadCallback, null);
//        }

//        private void ReadCallback(IAsyncResult asyncResult) {
//            try {
//                int bytesRead = NetworkStream.EndRead(asyncResult);
//                if (bytesRead > 0) {
//                    string s = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
//                    OnMessageReceived(s);
//                    NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, ReadCallback, null);
//                }
//            }
//            catch {
//                return;
//            }
//        }

//        internal void Stop() {
//            client.Close();
//        }

//        internal void Write(string s) {
//            log.Trace("S SND:" + s);
//            NetworkStream.WriteStringUtf8(s);
//        }

//        internal string Read() {
//            return NetworkStream.ReadStringUtf8(65536);
//        }

//        internal event EventHandler<EventArgs<string>> MessageReceived;

//        private void OnMessageReceived(string message) {
//            if (MessageReceived != null) {
//                MessageReceived(this, new EventArgs<string>(message));
//            }
//        }

//        internal void WriteStream(int versionMajor) {
//            Write("<stream:stream xmlns='jabber:server' version='" + versionMajor + ".0' to='test' xmlns:stream='http://etherx.jabber.org/streams'>");
//        }
//    }
//}