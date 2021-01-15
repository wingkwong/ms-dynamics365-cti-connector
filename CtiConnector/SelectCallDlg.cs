#region
// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================
#endregion

using System.Windows.Forms;
using Microsoft.Uii.Csr.Cti.Providers;
using Microsoft.Crm.Accelerator.Cca.Cti.Samples.CtiRoot.Properties;
using Microsoft.Uii.Desktop.UI.Core;
using System.Security.Permissions;

namespace Microsoft.Crm.Accelerator.Cca.Cti.Samples.CtiRoot
{
	/// <summary>
	/// Summary description for SelectCallDlg.
	/// </summary>
	public class SelectCallDlg : System.Windows.Forms.Form
	{
		private CallClassProvider selectedCall;

		private System.Windows.Forms.ListView callsList;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ColumnHeader startedCol;
		private System.Windows.Forms.ColumnHeader partiesCol;
		private System.Windows.Forms.ColumnHeader stateCol;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectCallDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		[SecurityPermission(SecurityAction.Demand)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",  MessageId = "", Justification="As this is a command maintaining it in lower case is required ")]
		public SelectCallDlg(LineClassProvider myLine, string command)
		{
			bool         showThisCall;
			ListViewItem item;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			this.Text = ResourceStrings.SELECT_CALL_DLG_TEXT;

			if ( command != null )
			{
				btnOk.Text = command;
				command = command.Replace( "&", "" );
				command = command.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			}
			foreach ( CallClassProvider call in myLine.Calls )
			{
				showThisCall = false;
				if ( command == "unhold" && call.CanUnhold() )
				{
					showThisCall = true;
				}
				else if ( command == "hold" && call.CanHold() )
				{
					showThisCall = true;
				}
				else if ( command == "answer" && call.CanAnswer() )
				{                
					showThisCall = true;
				}
				else if ( command == "hangup" && call.CanHangup() )
				{
					showThisCall = true;
				}
				else if ( command == "transfer" && call.CanTransfer() )
				{
					showThisCall = true;
				}
				else if ( command == null )  // use as default for all
				{
					showThisCall = true;
				}

				if ( showThisCall )
				{
					item = callsList.Items.Add( call.Started.ToShortTimeString() );
					item.SubItems.Add( CallClassProvider.CallStateText(call.State) );

					if ( call.UserTag != null )
					{
						item.SubItems.Add( call.UserTag + "  " + call.Parties );
					}
					else
					{
						item.SubItems.Add( call.Parties );
					}
					item.Tag = call;
				}
			}
			if ( callsList.Items.Count > 0 )
			{
				callsList.Items[ 0 ].Selected = true;
			}
			if ( callsList.Items.Count == 1 )
			{
				selectedCall = (CallClassProvider)callsList.Items[ 0 ].Tag;
			}
		}

		/// <summary>
		/// The number of calls which match the request made when this dialog
		/// was created, such as which calls can be unheld or answered.
		/// </summary>
		public int MatchingCalls
		{
			get
			{
				return callsList.Items.Count;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectCallDlg));
			this.callsList = new System.Windows.Forms.ListView();
			this.startedCol = new System.Windows.Forms.ColumnHeader();
			this.stateCol = new System.Windows.Forms.ColumnHeader();
			this.partiesCol = new System.Windows.Forms.ColumnHeader();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// callsList
			// 
			this.callsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.startedCol,
			this.stateCol,
			this.partiesCol});
			this.callsList.FullRowSelect = true;
			resources.ApplyResources(this.callsList, "callsList");
			this.callsList.MultiSelect = false;
			this.callsList.Name = "callsList";
			this.callsList.UseCompatibleStateImageBehavior = false;
			this.callsList.View = System.Windows.Forms.View.Details;
			this.callsList.DoubleClick += new System.EventHandler(this.btnOk_Click);
			// 
			// startedCol
			// 
			resources.ApplyResources(this.startedCol, "startedCol");
			// 
			// stateCol
			// 
			resources.ApplyResources(this.stateCol, "stateCol");
			// 
			// partiesCol
			// 
			resources.ApplyResources(this.partiesCol, "partiesCol");
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// SelectCallDlg
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.callsList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectCallDlg";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOk_Click(object sender, System.EventArgs e)
		{

			int sel = 0;
			// see if a call is selected
			if ( callsList.SelectedItems.Count == 0 )
			{
				TopMostMessageBox.Show(ResourceStrings.SELECT_CALL_DLG_SELECT_A_CALL, this.Text, MessageBoxButtons.OK);
				DialogResult = DialogResult.None;
			}
			else
			{
				sel = callsList.SelectedIndices[0];
				selectedCall = (CallClassProvider)callsList.Items[ sel ].Tag;
				DialogResult = DialogResult.OK;
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			// setting the selected call to null prevents code using this dialog
			// from taking action on a call.
			selectedCall = null;
			DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// The call which a user has selected to be acted upon.
		/// </summary>
		public CallClassProvider SelectedCall
		{
			get
			{
				return selectedCall;
			}
		}

	}
}