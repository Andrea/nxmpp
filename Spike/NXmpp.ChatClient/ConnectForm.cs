using System;
using System.Windows.Forms;
using System.Threading;

namespace NXmpp.ChatClient {
	public partial class ConnectForm : Form {
		private XmppClient _xmppClient;

		internal XmppClientContainer XmppClientContainer { get; private set; }

		public ConnectForm() {
			InitializeComponent();
			CheckLoginButtonEnabled();
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void username_TextChanged(object sender, EventArgs e) {
			CheckLoginButtonEnabled();
		}

		private void CheckLoginButtonEnabled() {
			loginButton.Enabled = !string.IsNullOrEmpty(usernameTextBox.Text);
		}

		private void loginButton_Click(object sender, EventArgs e) {
			usernameTextBox.Enabled = passwordTextBox.Enabled = cancelButton.Enabled = loginButton.Enabled = false;
			loginProgressLabel.Visible = true;
			var xmppSettings = new XmppClientSettings(JId.Parse(usernameTextBox.Text), passwordTextBox.Text, (a, b, c) => true);
			_xmppClient = new XmppClient(xmppSettings);
			XmppClientContainer = new XmppClientContainer(_xmppClient, SynchronizationContext.Current);
			_xmppClient.ConnectionStateChanged += _xmppClient_ConnectionStateChanged;
			_xmppClient.Connect();
		}

		void _xmppClient_ConnectionStateChanged(object sender, EventArgs<ConnectionStateType> e) {
			if (InvokeRequired) {
				this.Invoke(() => _xmppClient_ConnectionStateChanged(this, e));
			}
			else {
				loginProgressLabel.Text = e.Value.ToString();
				if (e.Value == ConnectionStateType.Connected) {
					DialogResult = DialogResult.OK;
					Close();
				}
			}
		}
	}
}