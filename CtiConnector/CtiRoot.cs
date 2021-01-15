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
using System.Windows.Forms;
using Microsoft.Uii.Desktop.Cti.Controls;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Csr.Cti.Providers;
using System.Diagnostics;
using Microsoft.Uii.Common;
using Microsoft.Uii.Common.Logging;
using Microsoft.Uii.Desktop.Cti.Core.Properties;
using Microsoft.Crm.Accelerator.Cca.Cti.Samples.CtiRoot.Properties;
using System.Globalization;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.UI.Core;
using System.Security.Permissions;

namespace Microsoft.Crm.Accelerator.Cca.Cti.Samples.CtiRoot
{
	/// <summary>
	/// Class to implement Cti Root functionality and CTI hosted control
	/// </summary>
	public partial class CtiRoot : CtiHostedControl, IDisposable
	{
		#region Variables
		private readonly CtiHandler cti;
		private LineClassProvider myLine;
		private TelephonyProvider ctiProvider;
		private AgentDesktopSessions sessionMgr;

		public override object SessionManager
		{
			set
			{
				if (sessionMgr == null)
				{
					sessionMgr = (AgentDesktopSessions)value;
					base.SessionManager = value;
				}
			}
		}

 

		#endregion

		#region Constructors
		/// <summary>
		/// default constructor
		/// </summary>
		public CtiRoot()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="appID"></param>
		/// <param name="appName"></param>
		/// <param name="appInitXML"></param>
		public CtiRoot(Guid appID, string appName, string appInitXML)
			: base(appID, appName, appInitXML)
		{
			InitializeComponent();

			cti = new CtiHandler();
		}
		#endregion

		#region UI bits

		/// <summary>
		/// Handle Display of Event Data into the display / trouble shooting UI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvEventList_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		void clearItemsToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Invoke((MethodInvoker)delegate
			{
				lvEventList.Items.Clear();
			});
		}


		#endregion

		#region TAPI/TSAPI Interface bits

		/// <summary>
		///  This is the event taht is called when a New call comes in. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Cti_CallChanged(object sender, CallEventArgs e)
		{
			switch (e.State)
			{
				case CallClassProvider.CallState.Special:
					// Ignoring the particular case.
					break;
				case CallClassProvider.CallState.Unknown:
					// Ignoring the particular case. 
					break;
				case CallClassProvider.CallState.None:
					// Ignoring the particular case 
					break;
				//case CallClassProvider.CallState.Disconnected
				case CallClassProvider.CallState.Idle:
					// As the Tapi Simulator doesn’t provide a disconnected event,and just goes to Idle,
					// so sending a change status event.

					if (CallStateChangeEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs(e.State.ToString(), e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallStateChangeEvent(this, args);
					}

					// Now flush the call. 
					if (CallDestructedEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs(e.State.ToString(), e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallDestructedEvent(this, args);
					}
					if (AgentLogMessageEvent != null && CallMediaConnectedEvent!=null && GeneralCtiEvent!=null && CallNewCallInfoEvent != null && CallItemEvent != null)
					{
					}
					break;
				case CallClassProvider.CallState.Incoming_Call:
					// As the Simulator does not provide a "ringing" event,sending a new call event.
					if (CallNewCallEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs("NewCALL", e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallNewCallEvent(this, args);
					}
					// Another workaround for the TAPI demo Code....
					e.State = CallClassProvider.CallState.Ringing;
					if (CallStateChangeEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs(e.State.ToString(), e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallStateChangeEvent(this, args);
					} break;
				case CallClassProvider.CallState.Ringing:
					// As the Simulator does not provide a "ringing" event,sending a new call event.
					if (CallNewCallEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs("NewCALL", e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallNewCallEvent(this, args);
					}
					// Another workaround for the TAPI demo Code....
					if (CallStateChangeEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs(e.State.ToString(), e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallStateChangeEvent(this, args);
					} break;
				case CallClassProvider.CallState.Connected:
					e.State = CallClassProvider.CallState.Connected;
					if (CallStateChangeEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs(e.State.ToString(), e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallStateChangeEvent(this, args);
					}
					break;
				default:
					if (CallStateChangeEvent != null)
					{
						CtiCoreEventArgs args = new CtiCoreEventArgs(e.State.ToString(), e, e.Call.CallID.ToString(CultureInfo.InvariantCulture));
						CallStateChangeEvent(this, args);
					}
					break;
			}
		}

		/// <summary>
		/// The Line for this instance
		/// </summary>
		public LineClassProvider CtiLine
		{
			get
			{
				return cti.CtiLine;
			}
			set
			{
				cti.CtiLine = value;
			}
		}

		/// <summary>
		/// Get or set Cti
		/// </summary>
		public TelephonyProvider Cti
		{
			get
			{
				return cti.Cti;
			}
			set
			{
				cti.Cti = value;
			}
		}

		/// <summary>
		/// Get RemoteTelephonyProcess
		/// </summary>
		public Process RemoteTelephonyProcess
		{
			get
			{
				return cti.RemoteTelephonyProcess;
			}
		}

		/// <summary>
		/// Creates an intances of the TelephonyProvider
		/// </summary>
		/// <returns></returns>
		public TelephonyProvider InitializeCti()
		{
			return cti.InitializeCti();
		}

		/// <summary>
		/// Intialize and return the line of the agentNumber passed in
		/// </summary>
		/// <returns></returns>
		public LineClassProvider InitializeLine(string agentNumber)
		{
			return cti.InitializeLine(agentNumber);
		}

		/// <summary>
		///	Need to start the .Net Remote process that does the telephony functions
		/// If its already started, it will detect so and exit and leave the running one
		/// alone.
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public Process InitializeRemoteTelephonyProcess(string app)
		{
			return cti.InitializeRemoteTelephonyProcess(app);
		}

		/// <summary>
		/// For backwards compatibility (used for TAPI and demos)
		/// </summary>
		/// <param name="type">The type of telephony API to use.</param>
		/// <returns></returns>
		public bool Init(TelephonyProvider.CtiType type)
		{
			return cti.Init(type);
		}

		/// <summary>
		/// For backwards compatibility (used for Avaya TSAPI)
		/// </summary>
		/// <param name="type">The type of telephony API to use.</param>
		/// <param name="user"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool Init(TelephonyProvider.CtiType type, string user, string password)
		{
			return cti.Init(type, user, password);
		}

		/// <summary>
		/// For use in installations using the Avaya SoftPhone product.
		/// </summary>
		/// <param name="type">The type of telephony API to use.</param>
		/// <param name="user"></param>
		/// <param name="password"></param>
		/// <param name="agentId"></param>
		/// <param name="agentPassword"></param>
		/// <param name="agentNumber"></param>
		/// <returns></returns>
		public bool Init(TelephonyProvider.CtiType type, string user, string password, string agentId, string agentPassword, string agentNumber)
		{
			return cti.Init(type, user, password, agentId, agentPassword, agentNumber);
		}

		/// <summary>
		/// Return the active call, null if no call is active.
		/// </summary>
		/// <returns>Returns the active call, or null.</returns>
		public CallClassProvider GetActiveCall()
		{
			return cti.GetActiveCall();
		}

		/// <summary>
		/// Returns the Call for a given callID
		/// </summary>
		/// <param name="callId"></param>
		/// <returns></returns>
		public CallClassProvider GetCall(int callId)
		{
			return cti.GetCall(callId);
		}

		/// <summary>
		/// Return all the calls 
		/// </summary>
		/// <returns></returns>
		public CallsClassProvider GetCalls()
		{
			return cti.GetCalls();
		}

		/// <summary>
		/// return the number of calls
		/// </summary>
		/// <returns></returns>
		public int GetNumberOfCalls()
		{
			return cti.GetNumberOfCalls();
		}

		/// <summary>
		/// Hang up the current call.
		/// </summary>
		public void HangUp(int callId)
		{
			cti.HangUp(callId);
		}

		/// <summary>
		/// Can we hang up the current call?
		/// </summary>
		/// <param name="callId"></param>
		/// <returns></returns>
		public bool CanHangUp(int callId)
		{
			return cti.CanHangUp(callId);
		}

		/// <summary>
		/// Can we take the current call off hold?
		/// </summary>
		/// <param name="callId"></param>
		/// <returns></returns>
		public bool CanUnhold(int callId)
		{
			return cti.CanUnhold(callId);
		}

		/// <summary>
		/// Hold the current call
		/// </summary>
		/// <param name="onHold"></param>
		/// <returns>Return true or false based on success of holding call.</returns>
		public bool Hold(bool onHold)
		{
			return cti.Hold(onHold);
		}

		/// <summary>
		/// find the longest call
		/// </summary>
		/// <returns></returns>
		public CallClassProvider FindTheLongestCall()
		{
			return cti.FindTheLongestCall();
		}

		/// <summary>
		/// Find the length of the call passed in
		/// </summary>
		/// <param name="call"></param>
		/// <returns></returns>
		public string LengthOfCall(CallClassProvider call)
		{
			return cti.LengthOfCall(call);
		}

		/// <summary>
		/// Return whether or not there are calls
		/// </summary>
		/// <returns></returns>
		public bool HaveCalls()
		{
			return cti.HaveCalls();
		}

		/// <summary>
		/// Return whether or not someone is on the phone.
		/// </summary>
		/// <returns>Returns true or false.</returns>
		public bool OnThePhone()
		{
			return cti.OnThePhone();
		}

		/// <summary>
		/// Take the current call off hold.
		/// </summary>
		/// <returns>Returns true or false based on success.</returns>
		public bool Unhold(int callId)
		{
			return cti.Unhold(callId);
		}

		/// <summary>
		/// Clean up resources.
		/// </summary>
		public void CleanUp()
		{
			cti.CleanUp();
		}


		/// <summary>
		/// Translate the String Call ID to an Init. 
		/// </summary>
		/// <param name="sCallID"></param>
		/// <returns></returns>
		private static int TranslateCallID(string sCallID)
		{
			// Turn the call Id into an int.
			try
			{
				return Convert.ToInt32(sCallID, CultureInfo.InvariantCulture);
			}
			catch
			{
				return -1;
			}

		}


		#endregion


		#region CTI DESKTOP Interface

		public override string GetAgentId()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Login Interface...
		/// </summary>
		/// <returns></returns>
		public override bool InitAndLogOn()
		{
			TelephonyProvider.CtiType ctiType = TelephonyProvider.CtiType.None;
			string agentNumber=string.Empty;
			try
			{
				ConfigurationValueReader configReader = ConfigurationValueReader.ConfigurationReader();
				ctiType = (TelephonyProvider.CtiType)Convert.ToInt32(configReader.ReadAppSettings("CtiType"), CultureInfo.InvariantCulture);
				agentNumber = configReader.ReadAppSettings("MachineName");
			}
			catch
			{
			}

			if (ctiType == TelephonyProvider.CtiType.None ||
					string.IsNullOrEmpty(agentNumber))
			{
				return false;
			}

			// Initialize the CTI class
			ctiProvider = InitializeCti();

			if (ctiProvider != null)
			{
				bool ctiLogin = Init(ctiType);

				// If agentNumber is null, then we aren't supporting CTI.
				if (ctiLogin && ! string.IsNullOrEmpty(agentNumber))
				{
					myLine = InitializeLine(agentNumber);
					if (myLine == null)
					{
						Logging.Error(Application.ProductName, ResourceStrings.UNABLETOCONFIGUREPHONE);
					}

					/// this is the new call event for TAPI/TSAPI
					cti.Cti.CallChanged += new CallEventHandler(Cti_CallChanged);

				}

			}

			if (ctiProvider != null)
			{
				//raise Agent ready Event. 
				if (AgentStateChangedEvent != null)
				{
					AgentStateChangedEvent(this, new CtiCoreEventArgs("AgentStateChange", CtiAgentStates.AVAILABLE, string.Empty));
				}
			}

			// True for success
			return ctiProvider != null;
		}

		public override bool CleanUpAndShutdown()
		{
			throw new NotImplementedException();
		}

		public override bool LogOffAgent()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the Agent State for any given situation... TAPI Demo does not support Direct Agent State.
		/// </summary>
		/// <param name="stateChangeRequest"></param>
		/// <returns></returns>
		public override bool SetAgentState(CtiStateRequest stateChangeRequest)
		{
			if (ctiProvider != null)
			{
				if (stateChangeRequest.StateType == StateRequestType.AGENT)
				{
					if (stateChangeRequest.StateName.Equals(CtiAgentStates.AVAILABLE, StringComparison.OrdinalIgnoreCase))
					{
						//raise Agent ready Event. 
						if (AgentStateChangedEvent != null)
						{
							AgentStateChangedEvent(this, new CtiCoreEventArgs("AgentStateChange",CtiAgentStates.AVAILABLE, string.Empty));
							return true;
						}
					}
					if (stateChangeRequest.StateName.Equals(CtiAgentStates.UNAVAILABLE, StringComparison.OrdinalIgnoreCase))
					{
						//raise Agent ready Event. 
						if (AgentStateChangedEvent != null)
						{
							AgentStateChangedEvent(this, new CtiCoreEventArgs("AgentStateChange", CtiAgentStates.UNAVAILABLE, string.Empty));
							return true;
						}
					}
				}
				if (stateChangeRequest.StateType == StateRequestType.CALL)
				{
				}
			}
			return true;

		}

		/// <summary>
		/// Not Used for TAPI
		/// </summary>
		/// <param name="mediaAddress"></param>
		/// <returns></returns>
		public override bool SetAgentMediaAddress(string mediaAddress)
		{
			// Not used by the TAPI adapter. 
			return true;
		}

		/// <summary>
		/// Answer a Call
		/// </summary>
		/// <param name="commandData"></param>
		/// <returns></returns>
		public override bool AnswerCall(CtiCommandRequest commandData)
		{
			if (myLine != null)
			{
				// Pick a call to answer
				using (SelectCallDlg dlg = new SelectCallDlg(myLine, "&Answer"))
				{
					if (dlg.MatchingCalls >1)
					{
						dlg.ShowDialog();
					}
					CallClassProvider call = dlg.SelectedCall;
					if (call != null)
					{
						// place any call on hold that can be held
						if(cti.GetActiveCall()!=null)
							cti.GetActiveCall().Hold();
						call.Answer();
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Handle Pick up Call... 
		/// this is also used for UnHold. 
		/// </summary>
		/// <param name="commandData"></param>
		/// <returns></returns>
		public override bool PickupCall(CtiCommandRequest commandData)
		{
			if (myLine != null)
			{
				int iCallId = TranslateCallID(commandData.CtiCallId);
				if (iCallId != -1)
				{
					// is the call on hold? 
					if (cti.GetCall(iCallId).State == CallClassProvider.CallState.Hold)
					{
						// Pick a call to retrieve
						using (SelectCallDlg dlg = new SelectCallDlg(myLine, "&Unhold"))
						{
							if (dlg.MatchingCalls > 1)
							{
								dlg.ShowDialog();
							}

							CallClassProvider call = dlg.SelectedCall;
							if (call != null)
							{
								// place any call on hold that can be held
								if (cti.GetActiveCall() != null)
									cti.GetActiveCall().Hold();
								call.Unhold();
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		///  Handle Hold event... 
		/// </summary>
		/// <param name="commandData"></param>
		/// <returns></returns>
		public override bool HoldCall(CtiCommandRequest commandData)
		{
			if (myLine != null)
			{
				// Pick a call to place on hold
				using (SelectCallDlg dlg = new SelectCallDlg(myLine, "&Hold"))
				{
					if (dlg.MatchingCalls >1)
					{
						dlg.ShowDialog();
					}
					CallClassProvider call = dlg.SelectedCall;
					if (call != null)
					{
						call.Hold();
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Handle Hang up call event.
		/// </summary>
		/// <param name="commandData"></param>
		/// <returns></returns>
		public override bool HangUpCall(CtiCommandRequest commandData)
		{
			if (myLine != null)
			{
				int iCallID = TranslateCallID(commandData.CtiCallId);
				if (iCallID != -1)
				{
					if (CanHangUp(iCallID))
					{
						using (SelectCallDlg dlg = new SelectCallDlg(myLine, "&Hangup"))
						{
							if (dlg.MatchingCalls > 1)
							{
								dlg.ShowDialog();
							}
							CallClassProvider call = dlg.SelectedCall;
							if (call != null)
							{
								call.Hangup();
								return true;
							}
						}
					}

				}
			}
			return false;
		}


		public override bool WrapCall(CtiCommandRequest commandData)
		{
			return false;
		}

		public override bool RejectCall(CtiCommandRequest commandData)
		{
			return false;
		}

		public override bool SendChatMessage(CtiCommandRequest commandData)
		{
			return false;
		}


		public override bool PushUrl(CtiCommandRequest commandData)
		{
			// Not used by the TAPI Example ADapter
			return true;
		}

		public override bool SetCallVariable(CtiCommandRequest commandData)
		{
			return false;
		}

		public override string ExecuteGenericCommand(string commandName, CtiCommandRequest commandData)
		{
			return string.Empty;
		}



		public override object GetGlobalAgentConfigVars()
		{
			return false;
		}

		public override event EventHandler<CtiCoreEventArgs> AgentStateChangedEvent;

		public override event EventHandler<CtiCoreEventArgs> AgentLogMessageEvent;

		public override event EventHandler<CtiCoreEventArgs> CallNewCallEvent;

		public override event EventHandler<CtiCoreEventArgs> CallDestructedEvent;

		public override event EventHandler<CtiCoreEventArgs> CallStateChangeEvent;

		public override event EventHandler<CtiCoreEventArgs> CallNewCallInfoEvent;

		public override event EventHandler<CtiCoreEventArgs> CallItemEvent;

		public override event EventHandler<CtiCoreEventArgs> CallMediaConnectedEvent;


		#endregion

		#region IDisposable Members

		/// <summary>
		///  Shutdown code. 
		/// </summary>
		public new void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
				
		public override event EventHandler<CtiCoreEventArgs> GeneralCtiEvent;
		public override bool ConferenceCall(CtiCommandRequest commandData)
		{
			return false;
		}
		public override bool MakeOutboundCall(CtiCommandRequest commandData)
		{
			return false;
		}

		[SecurityPermission(SecurityAction.Demand)]
		public override bool TransferCall(CtiCommandRequest commandData)
		{
			Guid transferToAgentID = Guid.Empty;
			try
			{
				if (myLine != null)
				{
					// Find call that can be transferred
					using (SelectCallDlg dlg = new SelectCallDlg(myLine, "&Transfer"))
					{
						if (dlg.MatchingCalls > 1)
						{
							dlg.ShowDialog();
						}

						CallClassProvider call = dlg.SelectedCall;
						var agentStateService = AifServiceContainer.Instance.GetService<IAgentStateService>();
						if (call != null)
						{
							using (GetAgentId numberDlg = new GetAgentId())
							{
								if (numberDlg.ShowDialog() == DialogResult.OK)
								{
									try
									{
										// See if this is being transferred to an agent

										if (agentStateService != null)
										{
											transferToAgentID = agentStateService.GetAgentId(numberDlg.AgentNumber);

											if (transferToAgentID != Guid.Empty && sessionMgr.ActiveSession != null)
											{
												string state = sessionMgr.ActiveSession.Save(true);
												if (!String.IsNullOrEmpty(state))
												{
													agentStateService.SetSessionTransferInformation(transferToAgentID, call.CallerNumber, state);
												}
											}
											// Make sure the transfer works
											if (call.Transfer(numberDlg.AgentNumber) < 0)
											{
												// if anything failed, clear the session transfer info
												if (transferToAgentID != Guid.Empty)
												{
													agentStateService.SetSessionTransferInformation(transferToAgentID, "", "");
												}
											}
											// Transfer is probably working
											else
											{
												// Ask in case the agent is expected to do wrap up or
												//   in case the call did not really transfer.
												// Don't do this if there is only a global session.
												if (sessionMgr.ActiveSession != null &&
													!sessionMgr.ActiveSession.Global &&
													TopMostMessageBox.Show(ResourceStrings.DESKTOP_CLOSE_AFTER_TRANSFER, Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
												{
													sessionMgr.CloseSession(sessionMgr.ActiveSession, false);
												}
											}
										}
									}
									catch (Exception exp)
									{
										Logging.Error(Application.ProductName, ResourceStrings.DESKTOP_ERR_TRANSFERRING_CALL, exp);

										// if anything failed, clear the session transfer info
										if (transferToAgentID != Guid.Empty)
										{
											if (agentStateService != null)
												agentStateService.SetSessionTransferInformation(transferToAgentID, "", "");
										}
										return false;
									} // try...catch
								}
								else
								{
									return false;
								}
							} 
						}
					}
				}
				else
				{
					return false;
				}
			}
			catch (Exception exp)
			{
				Logging.Error(Application.ProductName, ResourceStrings.DESKTOP_ERR_TRANSFERRING_CALL, exp);
				return false;
			}
			return true;
		}
		
	}
}
