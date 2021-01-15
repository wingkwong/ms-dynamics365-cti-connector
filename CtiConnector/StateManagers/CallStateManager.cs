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
using System.Linq;
using System.Text;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.Cti.Core.Properties; // This is the base namespace for all CTI Subsystem components

namespace CtiConnector.StateManagers
{
    /// <summary>
    /// The CallStateManager is responsible for translating and raising Call centric events to UII. 
    /// This class is also responsible for completing the call record for a CTI event as part of the new call event. 
    /// </summary>
    public class CallStateManager : CtiCallStateManager
    {
        /// <summary>
        /// Added the Default constructor here. 
        /// Handling the CallStateManager Setup 
        /// </summary>
        /// <param name="rootCti">Initialized Cti Root Control</param>
        public CallStateManager(ICtiControl rootCti) : base(rootCti) { }

        #region Required Overrides. 


        /// <summary>
        /// This event is raised by the CTI SubSystem when a new call event is raised by the CTI Adapter. 
        /// This event is used to complete populating the UII Cti Call Record and notifying the desktop manager. 
        /// </summary>
        /// <param name="calldata">This is the Uii Call Data object used to store information about this call, this is built by the CTI Subsystem on a new call event.</param>
        /// <param name="e">This is the core CTI Object data being sent up from the CTI Interface.</param>
        //public override void OnNewCallEvent(CallInfoData calldata, CtiCoreEventArgs e)
        protected override void OnNewCallEvent(NewCallEventData callEventData)
        {

            // Here,  you would decode the vendor specific call event  and map  additional data to the Uii Call Info Record. 

            /* 
             // Try to Map the call object to the vendor call record. 
            NewCall NewCallEvnt = null;
            if (callEventData.EventData.EventInfo is NewCall)
            {
                NewCallEvnt = (NewCall)callEventData.EventData.EventInfo;
            }
            else
            {
                // if the record type was incorrect, abort the new call event and throw it away
                // RemoveCall will remove the call from the Uii Call List. 
                RemoveCall(callEventData.CallInfo);
                return;
            }

            // Assign the vendor specific call id to the call object 
            callEventData.CallInfo.CallId = NewCallEvnt.CallId;

            // Disassemble call data from the vendor call record and place it in the Call Info Record.
            object[] oCallInfoItmes = NewCallEvnt.CallInfo.Items;
            foreach (ConfigItem infoItem in oCallInfoItmes)
            {
                switch (infoItem.Id)
                {
                    case "Caller":
                        // Get the bits outa Caller
                        foreach (ConfigItem callerInfo in infoItem.Items)
                        {
                            switch (callerInfo.Id)
                            {
                                case "Name":
                                    callEventData.CallInfo.CallerName = callerInfo.Value;
                                    break;
                                case "ANI":
                                    callEventData.CallInfo.Ani = callerInfo.Value;
                                    break;
                                case "DNIS":
                                    callEventData.CallInfo.Dnis = callerInfo.Value;
                                    break;
                                case "IPAddress":
                                    callEventData.CallInfo.IPAddress = callerInfo.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case "Type":
                        callEventData.CallInfo.CallType = infoItem.Value;
                        break;
                    default:
                        break;
                }
            }

            // This does a thread-safe update to the UII Call Record 
            UpdateCallInfoItemEntry(callEventData.CallInfo);

            // Create the New Call event args and Raise the event to UII,  
            //   this event will then be processed by the desktop manager. 
            NewCallEventArgs args = new NewCallEventArgs(callEventData.CallInfo.GetCtiCallRefId, callEventData.CallInfo);
            RaiseNewCallEvent(args);

            // If AutoAnswer is enabled, answer the call
            if (EnableAutoAnswer)
                AnswerCall(callEventData.CallInfo.GetCtiCallRefId);
             */
        }

        /// <summary>
        /// This method would be used by upstream components to request specific call data from or about a call.
        /// </summary>
        /// <param name="ccfCallRefCallID">The UII Call ID that data is being requested from</param>
        /// <param name="key">The key field identifying data to be returned.</param>
        /// <returns>String version of the requested variable or null/empty</returns>
        public override string GetCallVariable(Guid ccfCallRefCallID, string key)
        {
            // Example : 
            try
            {
                CallInfoData calldata = GetCallInfoData(ccfCallRefCallID);
                if (calldata != null)
                    if (!string.IsNullOrEmpty(calldata.CallId))
                    {
                        switch (key.ToUpper())
                        {
                            case "CALLERNAME":
                                return calldata.CallerName;
                            case "ANI":
                                return calldata.Ani;
                            case "CALLTYPE":
                                return calldata.CallType;
                            case "DNIS":
                                return calldata.Dnis;
                            case "IPADDRESS":
                                return calldata.IPAddress;
                            case "STARTTIME":
                                return calldata.CallReceived.ToUniversalTime().ToString();
                            case "CURRENTCALLSTATE":
                                return calldata.CurrentCallState.ToString();
                            default:
                                if (calldata.AdditionalParameters.ContainsKey(key.ToUpper()))
                                {
                                    return calldata.AdditionalParameters[key.ToUpper()];
                                }
                                else
                                {
                                    if (calldata.NewCallEventObject != null)
                                    {
                                        // Decode the object and dig though the object data for the requested key. 
                                    }
                                    return string.Empty;
                                }
                        }
                    }

            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// The call Info event is raised by the CTI connector when an update to data associated with a call changes. 
        /// </summary>
        /// <param name="e">This is the core CTI Object data being sent up from the CTI Interface.</param>
        public override void OnCallInfoEvent(CtiCoreEventArgs e)
        {
            // Example ... 
            // In this case we are looking at data on the call and decoding the vender specific data. 
            //  To make a determination about how to deal with a second call incoming on the same line.

            //CallInfoNewItem infoItem = (CallInfoNewItem)e.EventInfo;
            //bool bPickUp = false;
            //if (string.Compare("wasconnected", infoItem.CallInfoItem.Id, true) == 0)
            //{
            //    // the Call was just connected... Determine what to do. 
            //    int iCallCount = GetCurrentActiveCallCount;

            //    if (EnableAutoAnswer && iCallCount > 1)  // Auto Answer Enabled..
            //    {
            //        if (EnableOverrideAutoAnswerOnExistingCalls)
            //        {
            //            bPickUp = true;
            //        }
            //        else
            //            bPickUp = false;
            //    }
            //    else
            //        if (EnableAutoAnswer)
            //            bPickUp = true;

            //    if (bPickUp)
            //    {
            //        CallInfoData callData = GetCallInfoData(infoItem.CallId);
            //        PickupCall(callData.GetCtiCallRefId);
            //    }

            //}

        }


        /// <summary>
        /// This event is raised when the state of the connection changes. 
        /// These events can include, but are not limited to things like: 
        ///     Line Off Hook
        ///     Line Wrap Up
        ///     Line On Hold… 
        /// </summary>
        /// <param name="e">This is the core CTI Object data being sent up from the CTI Interface.</param>
        public override void OnCallStateChanged(CtiCoreEventArgs e)
        {
            //// Example ... 
            //// In this case we are looking at data on the call and decoding the vender specific data. 
            ////  To make a determination about how to deal with a the state of the line.

            //// Get the Vendor Data for the CallState Change Event. 
            //CallStateChange callStateCng = (CallStateChange)e.EventInfo;

            //// Set the state of the call in the call List. 
            //CallInfoData calldata = GetCallInfoData(callStateCng.CallId);
            //calldata.CurrentCallState = callStateCng.State.ToString();

            //// Update the UiiCall Record. 
            //UpdateCallInfoItemEntry(calldata);


            //// Here we are now assembling the event we are going to raise to the rest of the UII CTI Subsystem’s upstream listeners.
            ////UiiCallEventArgs args = null;
            //CtiCallEventArgs args = null; 
            //switch (callStateCng.State)
            //{
            //    case CallState.Undefined:  // this should set the phone to an inOpe State
            //        args = new CtiCallEventArgs(calldata.GetCtiCallRefId, callStateCng.State.ToString(),
            //            new CtiCallActionOptions()
            //            {
            //                EnableAnswer = true,
            //                EnableDial = true
            //            });
            //        args.CallState = "Ready";
            //        break;
            //    case CallState.Ringing:  // this should enable Answer
            //        args = new CtiCallEventArgs(calldata.GetCtiCallRefId, CtiCallStates.RINGING,
            //            new CtiCallActionOptions()
            //            {
            //                EnableAnswer = true,
            //                EnablePickup = true
            //            });

            //        break;
            //    case CallState.InCall: // this should enable Hold , Transfer , Conference 
            //        args = new CtiCallEventArgs(calldata.GetCtiCallRefId, CtiCallStates.OFFHOOK,
            //            new CtiCallActionOptions()
            //            {
            //                EnableHold = true,
            //                EnableTransfer = true,
            //                EnableConference = true
            //            });
            //        break;
            //    case CallState.OnHold: // this should enable unHold , Transfer , Conference 
            //        args = new CtiCallEventArgs(calldata.GetCtiCallRefId, CtiCallStates.ONHOLD,
            //            new CtiCallActionOptions()
            //            {
            //                EnableUNHold = true,
            //                EnableTransfer = true,
            //                EnableConference = true,
            //                EnableDial = true
            //            });
            //        break;
            //    case CallState.PickUpPending:
            //        args = new CtiCallEventArgs(calldata.GetCtiCallRefId, CtiCallStates.PICKUPPENDING,
            //            new CtiCallActionOptions()
            //            {
            //                EnableAnswer = true,
            //            });
            //        break;
            //    case CallState.WrapUp: // this should disable all
            //        args = new CtiCallEventArgs(calldata.GetCtiCallRefId, CtiCallStates.WRAP, new CtiCallActionOptions());
            //        break;
            //    case CallState.Destructed: // this should enable dial /  
            //        args = new CtiCallEventArgs(calldata.GetCtiCallRefId, CtiCallStates.DISCONNECTED,
            //            new CtiCallActionOptions()
            //            {
            //                EnableDial = true
            //            });
            //        break;
            //    default:
            //        System.Diagnostics.Trace.WriteLine("Unsupported event type detected : " + callStateCng.State);
            //        return;
            //}

            ////Raise Status change Event. 
            //RaiseCallStateChangeEvent(args); 
        }

        #endregion

    }
}
