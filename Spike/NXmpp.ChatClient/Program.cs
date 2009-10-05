using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NXmpp.ChatClient {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var connectForm = new ConnectForm();
			Application.Run(connectForm);
			if (connectForm.DialogResult == DialogResult.OK) {
				var mainForm = new Roster(connectForm.XmppClientContainer);
				Application.Run(mainForm);
			}
		}
	}
}
