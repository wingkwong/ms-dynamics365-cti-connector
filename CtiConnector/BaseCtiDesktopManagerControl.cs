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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// This is the base namespace for all CTI Subsystem components
using Microsoft.Uii.Desktop.Cti.Core;
// This is the CTI Hosted control namespace. 
using Microsoft.Uii.Desktop.Cti.Controls;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Uii.Common;
using System.Threading;
using Microsoft.Uii.Csr;
// This is the Reference to our customized state managers. 
using Microsoft.Uii.Desktop.Cti.Core.Properties;
using CtiConnector.StateManagers;


namespace CtiConnector
{
    public partial class BaseCtiDesktopManagerControl : CtiDesktopManager
    {
        #region Variables
        /// <summary>
        /// UII Session manager pointer. 
        /// </summary>
        private Sessions localSessionManager;

        /// <summary>
        /// Local variable for the custom UiiAgentStateManager
        /// </summary>
        private AgentStateManager localAgentStateManager;

        /// <summary>
        /// Local variable for the custom UiiCallStateManager
        /// </summary>
        private CallStateManager localCallStateManager;

        #endregion

        /// <summary>
        /// Default Constructor, not used by UII.
        /// </summary>
        public BaseCtiDesktopManagerControl()
        {
            InitializeComponent();
        }

        #region UII Interface

        /// <summary>
        /// UII Constructor, this is used by UII when creating this component. 
        /// </summary>
        /// <param name="appId">ID of the Hosted Control</param>
        /// <param name="appName">Name of the Hosted Control</param>
        /// <param name="appInitString">Initialization string for the Hosted Control</param>
        public BaseCtiDesktopManagerControl(Guid appId, string appName, string appInitString)
            : base(appId, appName, appInitString)
        {
            InitializeComponent();
        }

        /// <summary>
        /// UII Session Manager.
        /// </summary>
        public override object SessionManager
        {
            set
            {
                // Make sure we only set this once, 
                if (localSessionManager == null)
                {
                    localSessionManager = (Sessions)value;
                    // Wire Session close event.
                    localSessionManager.SessionCloseEvent += new SessionCloseHandler(localSessionManager_SessionCloseEvent);
                    base.SessionManager = value;
                }
            }
        }

        /// <summary>
        /// The action event handler for UII is used to receive actions from other hosted controls or adapters in  UII. 
        /// </summary>
        /// <param name="args"></param>
        protected override void DoAction(RequestActionEventArgs args)
        {
            //The action handler allows UII to accept actions from "non CTI aware" applications in the UII Space. 
            // In this case we will show an example of a reject call command from the search provider. 

            // Check to see if the requested action is "RejectCall" 
            if (args.Action.Equals("RejectCall", StringComparison.InvariantCultureIgnoreCase))
            {
                // The UII Call ID that we would like to reject. 
                if (!string.IsNullOrEmpty(args.Data))
                {
                    try
                    {
                        // Convert it to a Guid and call the local DoRejectCall Method. 
                        DoRejectCall(new Guid(args.Data));
                    }
                    catch (Exception ex)
                    {
                        // Error here. 
                    }
                }
            }

            // ... other handlers here as necessary  

            base.DoAction(args);
        }

        /// <summary>
        /// This event is raised when a session is closed. 
        /// in the case there is an Active call, the desktop manager can get in the way of the session close event 
        ///    and determine if any action needs to be taken. 
        /// For example, writing a Call Wrap Event to an Audit log
        /// </summary>
        /// <param name="session">Session that is being closed. </param>
        /// <returns>True allows the session to close,  false prevents the session from closing</returns>
        private bool localSessionManager_SessionCloseEvent(Session session)
        {
            try
            {
                if (session != null)
                {
                    // Blocking Command here
                    // Get a copy of the current session.
                    AgentDesktopSession aSess = (AgentDesktopSession)session;
                    // Get a handle to the Context that is in that session
                    Context lastSessionClosedContext = aSess.AppHost.GetContext();

                    // if there is a call on this session
                    if (aSess.CtiCallRefId != Guid.Empty)
                    {
                        // Check to see if there is a secondary call on the same session. 
                        if (aSess.CtiCallRefIdChat != Guid.Empty)
                        {
                            // if they are equivalent, they it is the same call. 
                            if (aSess.CtiCallRefIdChat != aSess.CtiCallRefId)
                            {
                                // if not,  
                                //   we may need to trigger a hang-up event for both the child and the parent Calls. 
                                // So here,  Issue a Hang-up event 
                                CallStateManager.HangUpCall(aSess.CtiCallRefIdChat);
                            }
                        }
                        // Now issue the Hang-up event to the primary call 
                        CallStateManager.HangUpCall(aSess.CtiCallRefId);
                    }
                }
            }
            catch (Exception ex)
            {
                // write a log event here.                 

            }
            return true;
        }

        #endregion

        #region CoreCTI Overrides

        /// <summary>
        /// This is a required override,
        /// This method is called by UII to Set the low level interface for CTI.
        /// This interface is also used to initialize the AgentState and CallState Managers
        /// </summary>
        /// <param name="ctiRoot">Pointer to the UII Cti Base Object</param>
        public override void SetRootCtiInterface(ICtiControl ctiRoot)
        {
            // Set up the AgentState Manager, and Set the internal UII Object Agent State object.
            localAgentStateManager = new AgentStateManager(ctiRoot);
            AgentStateManager = (ICtiAgentStateManager)localAgentStateManager;

            // Set up the CallState Manager, and Set the internal UII Object Call State object.
            localCallStateManager = new CallStateManager(ctiRoot);
            CallStateManager = (ICtiCallStateManager)localCallStateManager;

            // Wire base events for call management.
            if (CallStateManager != null)
            {
                CallStateManager.EnableAutoAnswer = true;
                CallStateManager.EnableOverrideAutoAnswerOnExistingCalls = true;
                CallStateManager.CallManagerStateNewCall += new EventHandler<NewCallEventArgs>(OnCallManagerStateNewCall);
                CallStateManager.CallManagerStateUpdate += new EventHandler<CtiCallEventArgs>(OnCallManagerStateUpdate);
            }
            base.SetRootCtiInterface(ctiRoot);
        }

        /// <summary>
        /// This method is called by UII to initialize and activate the CTI interface.  
        /// This can be used to do any sort of special handling or configuration to be passed in on the Start Command
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public override bool StartCtiInterface()
        {
            // set the internal CTI connected state. 
            IsCtiEnabled = false;

            // Check to see if the local UiiCtiHostedControl pointer has been set 
            //  ( this would be set by SetRootCtiInterface ) 
            if (CtiHostedControl != null)
            {
                // Now call Init and login on the CTI control in question. 
                if (CtiHostedControl.InitAndLogOn())
                {
                    IsCtiEnabled = true;
                }
                else
                {
                    // Display some error here... 
                    IsCtiEnabled = false;
                }
                return IsCtiEnabled;
            }
            else
            {
                // display some error here. 
                return IsCtiEnabled;
            }
        }

        #endregion

        #region Required CTI Call State Event Handlers

        /// <summary>
        /// This is the primary hander for “new” call event’s from the CIT system. 
        /// This event is raised by the call state manager after primary translation from the base CTI event.
        /// This event is used to create a Search Request to the system search provider or do other “pre-search” handling 
        /// </summary>
        /// <param name="sender">Call State Manager that raised this request.</param>
        /// <param name="e">New Event call Arguments</param>
        private void OnCallManagerStateNewCall(object sender, NewCallEventArgs e)
        {
            // This is how to determin if the current session has a call 
            if (((AgentDesktopSession)localSessionManager.ActiveSession).CtiCallRefId != Guid.Empty)
            {
                // I have a call. 
                // Do some alert or compenstation logic here.
                // maybe return ; 
            }

            // Form a request to the customer provider to launch a customer lookup with CTI data..
            CtiLookupRequest lookupRequest = new CtiLookupRequest(
                e.CallInfo.GetCtiCallRefId,         // Internal UII Call ID
                this.ApplicationName,   // Application Name that is raising the Lookup Request.
                e.CallInfo.CallType,    // Type of call
                e.CallInfo.Ani,         // Number that called me.
                e.CallInfo.Dnis);       // Number that they called. 

            //If you would like to add in custom properties that can be access by the search provider, use this construct
            // lookupRequest.AddLookupRequestItem("CUSTOMERID", GetParamFromCallState(e.UiiRefCallID, "custID"));

            // Now Serilize the Request and send it to the UII enviroment.
            string sData = GeneralFunctions.Serialize<CtiLookupRequest>(lookupRequest);

            // This is a custom implemented action sender that uses a threaded approach to send this request.
            // It is very important that you do this in a threaded manner because actions are blocking and 
            //      can slow down response to the Cti system.
            SendCommandParams cmd = new SendCommandParams("*", CtiLookupRequest.CTILOOKUPACTIONNAME, sData);
            new Thread(new ParameterizedThreadStart(SendAction)).Start(cmd);
        }


        /// <summary>
        /// This event is raised by the CTI subsystem when an update to a current call is received. 
        /// Updates types are very vendor specific, though they generally include line state changes, and data payload changes.
        /// Due to the rapid nature of CTI Events, it’s very important to trigger a thread to handle the actual event.
        /// </summary>
        /// <param name="sender">Call State Manager that raised this request.</param>
        /// <param name="e">Updated Call Event Args</param>
        private void OnCallManagerStateUpdate(object sender, CtiCallEventArgs e)
        {
            // Fire Threaded Event Hander
            new Thread(new ParameterizedThreadStart(CallUpdatedEventHandler)).Start(e);
        }

        #endregion

        #region CTI Interaction Commands in the desktop manager. 

        /// <summary>
        /// Handle issuing a “Reject Call” command to the CTI Subsystem. 
        /// </summary>
        /// <param name="UiiRefCallID">UII Call Reference ID ( Originally from the New Call Event )</param>
        private void DoRejectCall(Guid UiiRefCallID)
        {
            // Check to see if the local state manager is connected. 
            if (localCallStateManager != null)
            {
                // issue command. 
                localCallStateManager.RejectCall(UiiRefCallID, null);
            }
        }


        #endregion

        #region Threaded Event Handler

        /// <summary>
        /// Update Call Event Handler Delegate
        /// </summary>
        /// <param name="oData">Call Data.</param>
        private delegate void CallUpdatedEventHandlerDel(object oData);

        /// <summary>
        /// This method is used to decompose and process a call update event.
        /// </summary>
        /// <param name="oData"></param>
        private void CallUpdatedEventHandler(object oData)
        {
            // Get on the right thread
            if (this.InvokeRequired)
            {
                this.Invoke(new CallUpdatedEventHandlerDel(CallUpdatedEventHandler), new object[] { oData });
            }
            else
            {
                if (oData != null)
                {
                    try
                    {
                        // Call Event Arguments. 
                        CtiCallEventArgs e;

                        // Safety Check here. 
                        if (oData is CtiCallEventArgs)
                            e = (CtiCallEventArgs)oData;
                        else
                            return;

                        // The key component of the Call Data Event is the “call state”. 
                        // This is used to split set up event specific handling.

                        // When dealing with Call State Updates here, consider you are operating at a “global” level.
                        // you will have access to the CallState Manager in hosted controls as well 
                        //   if you need to handle precise event management. 
                        if (e.CallState.Equals(CtiCallStates.WRAP, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Handel the “Wrap State”
                            // In the case of “wrap call” , you may need to wrap up and issue a wrap call command to the CTI adapter… 
                            //    here is an example of that:
                            Dictionary<string, string> ParamData = new Dictionary<string, string>();
                            ParamData.Add("WRAPCODE", "<WRAP CODE>");
                            ParamData.Add("WRAPDESC", "<WRAP COMMENT>");
                            localCallStateManager.WrapUpCall(e.CtiRefCallId, ParamData);

                        }

                        // Here we are handling the Pickup pending event.
                        if (e.CallState.Equals(CtiCallStates.PICKUPPENDING, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Do Call Pickup 
                            localCallStateManager.PickupCall(e.CtiRefCallId);
                        }
                    }
                    catch { }
                }

            }

        }

        #endregion

        #region Threaded Send Action Method and class

        /// <summary>
        /// Delegate for Thread Safe version of the Uii Send Action. 
        /// </summary>
        /// <param name="oData"></param>
        private delegate void SendActionDel(object oData);
        /// <summary>
        /// Thread Safe version of the Uii Send Action
        /// </summary>
        /// <param name="oData"></param>
        private void SendAction(object oData)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SendActionDel(SendAction), new object[] { oData });
            }
            else
            {
                SendCommandParams cmd = (SendCommandParams)oData;
                RequestActionEventArgs args = new RequestActionEventArgs(cmd.Target, cmd.Action, cmd.Data);
                FireRequestAction(args);
            }
        }



        /// <summary>
        /// Command Parameters for the Threaded Send Action to UII
        /// </summary>
        public class SendCommandParams
        {
            public string Target { get; set; }
            public string Action { get; set; }
            public string Data { get; set; }

            public SendCommandParams()
            {

            }

            public SendCommandParams(string target, string action, string data)
            {
                Target = target;
                Action = action;
                Data = data;
            }
        }

        #endregion

    }



}
