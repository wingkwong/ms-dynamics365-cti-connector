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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Crm.Accelerator.Cca.Cti.Samples.CtiRoot.Properties;

namespace Microsoft.Crm.Accelerator.Cca.Cti.Samples.CtiRoot
{
	public partial class GetAgentId : Form
	{
		public GetAgentId()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Returns the Agent ID
		/// </summary>
		public string AgentNumber
		{
			get
			{
				return agentNumber;
			}
			set
			{
				agentNumber = value;
			}
		}
		private string agentNumber;
		private void GetAgentId_Load(object sender, EventArgs e)
		{
			lbAgentId.Text = ResourceStrings.AGENT_NUMBER;
			Submit.Text = ResourceStrings.SELECT_CALL_DLG_BTN_OK;
			txtAgentId.Focus();
		}

		private void Submit_Click(object sender, EventArgs e)
		{
			AgentNumber = txtAgentId.Text;
			if (string.IsNullOrEmpty(AgentNumber))
			{
				Microsoft.Uii.Csr.Win32API.MessageBeep(0);
				txtAgentId.Text = "";
			}
			else
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}
	}
}
