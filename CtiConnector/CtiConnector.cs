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
using Microsoft.Uii.Desktop.Cti.Controls;
using System.Threading;
// This is the CTI Hosted control namespace. 
using Microsoft.Uii.Common;
using Microsoft.Uii.Csr;

namespace CtiConnector
{
    /// <summary>
    /// This is the CTI “anchor” control for UII’s Cti Subsystem.
    /// This control is responsible for the point communications with the downstream Vendor provided CTI systems.
    /// </summary>
    public partial class CtiConnector : CtiHostedControl
    {
        #region Variables 

        /// <summary>
        /// Thread Manager for the CTI Event poller. 
        /// </summary>
        private ThreadStart CtiListenerThreadStart = null;

        /// <summary>
        /// Thread that the Poller runs on. 
        /// </summary>
        private Thread CtiListenerThread = null;

        /// <summary>
        /// This is a boolean used to track the state of the listner thread. 
        /// </summary>
        private bool _IsCtiListenerThreadActive = false;

        /// <summary>
        /// This is the Agent Ref ID that would be used by the CTI system to identifiy the agent on request to the CTI system 
        /// </summary>
        private string _AgentRefId;

        /// <summary>
        /// Local pointer to the Session Manager
        /// </summary>
        private Sessions localSessionManager;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public CtiConnector()
        {
            InitializeComponent();
        }

        #region UII Setup
        /// <summary>
        /// UII Constructor,  this is used by UII when creating this component. 
        /// </summary>
        /// <param name="appId">ID of the Hosted Control</param>
        /// <param name="appName">Name of the Hosted Control</param>
        /// <param name="appInitString">Initialization string for the Hosted Control</param>
        public CtiConnector(Guid appId, string appName, string appInitString)
            : base(appId, appName, appInitString)
        {
            InitializeComponent();
        }


        /// <summary>
        /// UII core property to determine if this control should be listed on the session's list. 
        /// </summary>
        public override bool IsListed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// UII Configuration Reader, provides access to the Uii Configuration System 
        /// </summary>
        public override ConfigurationValueReader ConfigurationReader
        {
            get
            {
                return base.ConfigurationReader;
            }
            set
            {
                base.ConfigurationReader = value;
            }
        }

        /// <summary>
        /// Set the local session manager object and connect the OnClose Event. 
        /// </summary>
        public override object SessionManager
        {
            set
            {
                if (localSessionManager == null)
                {
                    base.SessionManager = value;
                    localSessionManager = (Sessions)value;
                    localSessionManager.SessionCloseEvent += new SessionCloseHandler(localSessionManager_SessionCloseEvent);
                }
            }
        }

        /// <summary>
        /// Raised when a Session closes. 
        /// </summary>
        /// <param name="session">Session being closed.</param>
        /// <returns>if true, allow to close, if false, do not allow to close.</returns>
        private bool localSessionManager_SessionCloseEvent(Session session)
        {
            if (session.Global)
                return DeactivateConnection();
            else
                return true;
        }

        #endregion

        /* *****************************************************************************
         * This project type is configured for event polling from a service endpoint.
         *  for other types of connections, you will need to modify the activate and 
         *      deactivate methods as well as the threaded pollers.
         * *****************************************************************************
         */

        #region Activate / Deactivate CTI Connection

        /// <summary>
        /// Activate the Listener and login 
        /// </summary>
        private bool ActivateConnection()
        {
            //Initialize Connection to CTI Service
            InitializeCtiConnection();

            // Initialize Threaded Listener for CTI events
            if (CreateCtiListener())
            {
                // Login to the CTI system 
                if (!AgentLogon(0))
                {
                    LogErrorEvent(0, "General Error, unable to Login Agent.");
                    return false;
                }
                if (!ActivateCtiListener())
                {
                    LogErrorEvent(0, "General Error, unable to start listening to CTI events.");
                }
            }
            else
            {
                LogErrorEvent(0, "General Error, unable to create Listener Thread");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initialize Cti Objects and services here.
        /// </summary>
        private void InitializeCtiConnection()
        {
            /*
             *  Your code.
             */

        }

        /// <summary>
        /// Create the Listener Thread that will poll for CTI events and actions.  
        /// </summary>
        private bool CreateCtiListener()
        {
            try
            {
                CtiListenerThreadStart = new ThreadStart(this.EventInterfaceThread);
            }
            catch (Exception ex)
            {
                LogErrorEvent(0, ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method activates the Listen thread and starts listening for CIT events, 
        ///     At this point the system is live.  
        /// </summary>
        /// <returns> true on successful activation.</returns>
        private bool ActivateCtiListener()
        {
            if (CtiListenerThreadStart != null)
            {
                CtiListenerThread = new Thread(CtiListenerThreadStart);
                CtiListenerThread.Priority = ThreadPriority.BelowNormal;
                CtiListenerThread.Name = "CTIInterfaceThread";
                CtiListenerThread.Start();
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method is responsible for logging the agent into the CTI system and handling issues with login
        /// </summary>
        /// <returns>true on Good login, false if not.</returns>
        private bool AgentLogon(int iLoginCount)
        {
            /*
             * your code to login here.. 
             */

            // Set the Agent Identifier to be used by the system requesting information from the Downstream CTI system   
            _AgentRefId = "";

            return false;
        }

        /// <summary>
        /// Deactivate the listener and logout
        /// </summary>
        private bool DeactivateConnection()
        {
            return AgentLogout();
        }

        /// <summary>
        /// Attempt to Log the Agent out of the CTI system 
        /// </summary>
        /// <returns>true on good logout, false if not</returns>
        private bool AgentLogout()
        {
            System.Diagnostics.Trace.WriteLine(string.Format("Logout attempt for {0}", GetAgentRefId()));
            try
            {
                if (!string.IsNullOrEmpty(GetAgentRefId()))
                {
                    // disable the Running Poller
                    _IsCtiListenerThreadActive = false;
                    /*
                     * your code here
                     */
                    // if good..
                    try
                    {
                        // Force the processing thread to terminate. 
                        if (CtiListenerThread != null)
                            CtiListenerThread.Abort();
                    }
                    catch (Exception Ex)
                    {
                        // eat this error.
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogErrorEvent(0, ex.Message);
            }
            return false;
        }

        #endregion

        #region Threaded Methods

        /// <summary>
        /// Delegate for the IsCtiListenerThreadActive method.
        /// </summary>
        /// <returns></returns>
        private delegate bool IsCtiListenerThreadActiveDelegate();
        /// <summary>
        /// Returns the running state of the CTI Listener 
        /// Thread Safe
        /// </summary>
        /// <returns>True if Listener is running, False if not</returns>
        private bool IsCtiListenerThreadActive()
        {
            if (this.InvokeRequired)
            {
                return (bool)this.Invoke(new IsCtiListenerThreadActiveDelegate(IsCtiListenerThreadActive));
            }
            else
            {
                return _IsCtiListenerThreadActive;
            }
        }

        /// <summary>
        /// Delegate for the GetAgentRefId method
        /// </summary>
        /// <returns></returns>
        private delegate string GetAgentRefIdDelegate();
        /// <summary>
        /// Returns the Agent Reference id to be used by Uii when sending requests to the downstream CTI solution
        /// Thread Safe.
        /// </summary>
        /// <returns>Agent ID</returns>
        private string GetAgentRefId()
        {
            if (this.InvokeRequired)
            {
                return (string)this.Invoke(new GetAgentRefIdDelegate(GetAgentRefId));
            }
            else
            {
                return _AgentRefId;
            }
        }

        /// <summary>
        /// This method will process the events from the CTI system and raise the appropriate events to the UII Cti SubSystem
        /// </summary>
        /// <param name="oData">Object array of prams. </param>
        private void ParseDownStreamEvents(object oData)
        {
            // Example ... 

            object[] oEventList = null;
            if (oData is object[])
            {
                oEventList = (object[])oData;
            }

            if (oEventList != null)
            {
                // Base event would be the base event type for the vendor specific CTI events class. 
                foreach (/*BaseEvent*/ object Evnt in oEventList)
                {
                    string[] nameSpacePath = Evnt.ToString().Split('.');
                    string eventName = nameSpacePath[nameSpacePath.Length - 1];

                    // Add the event to the diag list
                    AddItemToEventList(eventName, Evnt);

                    switch (eventName)
                    {
                        case "AgentStateChange":
                            if (AgentStateChangedEvent != null)
                            {
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, string.Empty);
                                AgentStateChangedEvent(this, args);
                            }
                            break;
                        case "AgentLogMessage":
                            if (AgentLogMessageEvent != null)
                            {
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, string.Empty);
                                AgentLogMessageEvent(this, args);
                            }
                            break;
                        case "NewCall":
                            if (CallNewCallEvent != null)
                            {
                                // extract vendor the call id from the event...
                                string CallId = string.Empty;
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, CallId);
                                CallNewCallEvent(this, args);
                            }
                            break;
                        case "CallDestructed":
                            if (CallDestructedEvent != null)
                            {
                                // extract vendor the call id from the event...
                                string CallId = string.Empty;
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, CallId);
                                CallDestructedEvent(this, args);
                            }
                            break;
                        case "CallStateChange":
                            if (CallStateChangeEvent != null)
                            {
                                // extract vendor the call id from the event...
                                string CallId = string.Empty;
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, CallId);
                                CallStateChangeEvent(this, args);
                            }
                            break;
                        case "CallInfoNewItem":
                            if (CallNewCallInfoEvent != null)
                            {
                                // extract vendor the call id from the event...
                                string CallId = string.Empty;
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, CallId);
                                CallNewCallInfoEvent(this, args);
                            }
                            break;
                        case "NewChatItem":
                            if (CallItemEvent != null)
                            {
                                // extract vendor the call id from the event...
                                string CallId = string.Empty;
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, CallId);
                                CallItemEvent(this, args);
                            }
                            break;
                        case "CallMediaConnected":
                            if (CallMediaConnectedEvent != null)
                            {
                                CtiCoreEventArgs args = new CtiCoreEventArgs(eventName, Evnt, string.Empty);
                                CallMediaConnectedEvent(this, args);
                            }
                            break;
                        default:
                            break;
                    }

                }

            }
        }


        #region EventPoller

        /// <summary>
        ///  This method is run inside a thread and will raise threads out as events arrive from the downstream CTI solution 
        /// </summary>
        private void EventInterfaceThread()
        {
            // this provides a delay between pulses. 

            int idelaytime = 10000; // MS ( 1 second == 1000 ) 
            string sAgentRefId = GetAgentRefId();
            // This is the CTI event return record,  Vendor specific
            //CtiEventsRequestResult evntRslt = null;
            do
            {
                if (!String.IsNullOrEmpty(sAgentRefId))
                {
                    try
                    {
                        // Here,  you would make a sync web request to the CTI Event’s Service and pull down any events queued up for this client. 
                        // Example : 
                        //    evntRslt = evntSvc.GetEvents(sAgentRefId, idelaytime);

                    }
                    catch (Exception ex)
                    {
                        // this will happen is something goes wrong with the connection..
                        LogErrorTS(-1, string.Format("Error on Listener : {0} ", ex.Message));
                    }

                    //// Here you are checking for a good result from the event request,
                    //if (evntRslt != null)
                    //{
                    //    if (evntRslt.Events != null)
                    //    {
                    //        try
                    //        {
                    //           new Thread(new ParameterizedThreadStart(ParseDownStreamEvents)).Start(evntRslt.Events);
                    // 
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            // Thread Crashing... get out.. 
                    //            //LogErrorTS(-900, string.Format("Error on Listener : {0} ", ex.Message));
                    //            tWorker.Abort();
                    //        }
                    //    }
                    //}
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            while (IsCtiListenerThreadActive());
            // Kill Thread? 

        }



        #endregion

        #endregion 

        #region Utilites

        /// <summary>
        /// Delegate for the Thread Safe Version of the Log Error
        /// </summary>
        /// <param name="iErrorCode"></param>
        /// <param name="sErrorMessage"></param>
        private delegate void LogErrorDelegate(int iErrorCode, string sErrorMessage);
        /// <summary>
        /// Thread Safe Version of the Log Error
        /// </summary>
        /// <param name="iErrorCode">ID of the Error code</param>
        /// <param name="sErrorMessage">Error Message / Description </param>
        private void LogErrorTS(int iErrorCode, string sErrorMessage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new LogErrorDelegate(LogErrorEvent), iErrorCode, new string(sErrorMessage.ToCharArray()));
            }
            else
            {
                LogErrorEvent(iErrorCode, sErrorMessage);
            }
        }

        /// <summary>
        /// Logs an Error to the Error Provider.
        /// </summary>
        /// <param name="sErrorID"></param>
        /// <param name="sErrorName"></param>
        /// <param name="sErrorDescription"></param>
        private void LogErrorEvent(int iErrorID, string sErrorDescription)
        {
            // Wire up point with UII's Error Logger here.. 
            System.Diagnostics.Trace.WriteLine(sErrorDescription);
            // MessageBox.Show(sErrorDescription, iErrorID.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Diagnostic / Tracing UI

        /// <summary>
        /// Base control has a UI to allow for Troubleshooting, 
        /// this method adds an item to the troubleshooting list.
        /// </summary>
        /// <param name="sEventName">Name of the Event</param>
        /// <param name="baseEvent">Event itself.</param>
        private void AddItemToEventList(string sEventName, object baseEvent)
        {
            // do a check to see if your visiable.. if not skip..
            ListViewItem lvItem = new ListViewItem(DateTime.Now.ToLongTimeString());
            if (lvItem != null)
            {
                /// Only keep the top 100 events. 
                if (lvEventList.Items.Count > 100)
                    lvEventList.Items.RemoveAt(0);

                lvItem.SubItems.Add(sEventName);
                lvItem.Tag = baseEvent;
                if (!lvEventList.Items.Contains(lvItem))
                    lvEventList.Items.Add(lvItem);
                //else
                //System.Diagnostics.Trace.WriteLine(string.Format(">>>>Duplicate EVENT FROM CTI >>> {0} - {1} ", sEventName, baseEvent.Id));
            }
        }

        /// <summary>
        /// Clears items from the right click menu on the CTI Diagnostic UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearItemsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            lvEventList.Items.Clear();
        }


        /// <summary>
        /// Handle Display of Event Data into the Display / Trouble shooting UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvEventList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection col = lvEventList.SelectedItems;
            if (col.Count > 0)
            {
                // get the event object off the list item that was selected and serialize it. 
                //BaseEvent be = (BaseEvent)col[0].Tag;
                // Show result in the details panel. 
                //eventDetail.Text = GeneralFunctions.Serialize(be);
            }

        }

        #endregion

        #region Uii CTI Interface Methods and Events


        #region Events

        /// <summary>
        /// Used to Raise an Agent “log” event.. this can be used for populating local agent variables or other aspects of the agent data
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> AgentLogMessageEvent;
        /// <summary>
        /// Used to Raise an Agent State Changed Event to the Uii Cti Subsystem
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> AgentStateChangedEvent;
        /// <summary>
        /// Used to raise a notification that the call has been terminated and cleaned up
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> CallDestructedEvent;
        /// <summary>
        /// Used to raise a notification that some data on the call has changed
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> CallItemEvent;
        /// <summary>
        /// Use to raise an event to indicating that the media segment of the call has been connected
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> CallMediaConnectedEvent;
        /// <summary>
        /// Use to raise an event to indicating that a new call has been received
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> CallNewCallEvent;
        /// <summary>
        /// Used to raise an event indicating that information on the newly created call has been updated
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> CallNewCallInfoEvent;
        /// <summary>
        /// Used to raise an event indicating that the state of the referred to line has updated.
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> CallStateChangeEvent;
        /// <summary>
        /// Catch all event that can be raised by the CTI adapter. 
        ///     This is primarily an extensibility point to allow for “new”  functions to be implemented beyond the supplied types
        /// </summary>
        public override event EventHandler<CtiCoreEventArgs> GeneralCtiEvent;

        #endregion

        /// <summary>
        /// Called by UII’s Cti Subsystem, Requests adapter to answer a call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool AnswerCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Requests adapter to shutdown
        /// </summary>
        /// <returns>true on success</returns>
        public override bool CleanUpAndShutdown()
        {
            return DeactivateConnection();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Requests adapter set up a conference call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool ConferenceCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, User defined request to the CTI adapter, 
        ///     This is primarily an extensibility point to allow for “new”  functions to be implemented beyond the supplied types
        /// </summary>
        /// <param name="commandName">User defined Command name</param>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override string ExecuteGenericCommand(string commandName, CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Requests adapter return the current agent reference id
        /// </summary>
        /// <returns>Agent reference id</returns>
        public override string GetAgentId()
        {
            return GetAgentRefId();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to get Agent Configuration or Profile Information 
        /// </summary>
        /// <returns></returns>
        public override object GetGlobalAgentConfigVars()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to hang up a call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool HangUpCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to hold up a call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool HoldCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter initialize the connection to the CTI subsystem and login the agent
        /// </summary>
        /// <returns></returns>
        public override bool InitAndLogOn()
        {
            // note:  this should block until complete. 
            return ActivateConnection();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter shutdown, logout the agent, and release any stored objects.
        /// </summary>
        /// <returns></returns>
        public override bool LogOffAgent()
        {
            return DeactivateConnection();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to make a new outbound call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool MakeOutboundCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to Pick up a call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool PickupCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to Push a URL to the caller
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool PushUrl(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to Reject a call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool RejectCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to send a chat message to the caller
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool SendChatMessage(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to set the agent media address
        /// </summary>
        /// <param name="mediaAddress"></param>
        /// <returns></returns>
        public override bool SetAgentMediaAddress(string mediaAddress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to set the state of an Agent
        /// </summary>
        /// <param name="stateChangeRequest"></param>
        /// <returns></returns>
        public override bool SetAgentState(CtiStateRequest stateChangeRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to set one or more call variables on an call
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool SetCallVariable(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to Transfer a call to another address
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool TransferCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by UII’s Cti Subsystem, Request the adapter to wrap the call up
        /// </summary>
        /// <param name="commandData">CallID and additional Command Data</param>
        /// <returns></returns>
        public override bool WrapCall(CtiCommandRequest commandData)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
