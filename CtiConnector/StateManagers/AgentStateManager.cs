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
using Microsoft.Uii.Desktop.Cti.Core; // This is the base namespace for all CTI Subsystem components

namespace CtiConnector.StateManagers
{
    /// <summary>
    /// The Agent State Manager is responsible for translating and raising state alerts to UII 
    ///     and taking any basic action necessary to support the Agent State.  Also this class is usually 
    ///     responsible for managing data or variables concerning the agent him or her self. 
    /// Please refer to the Agent State Manager documentation prior to implementing this class
    /// </summary>
    public class AgentStateManager : CtiAgentStateManager
    {
        /// <summary>
        /// Added the Default constructor here. 
        /// Handling the AgentStateManager Setup 
        /// </summary>
        /// <param name="rootCti">Initialized Cti Root Control</param>
        public AgentStateManager(ICtiControl rootCti) : base(rootCti) { }



        /// <summary>
        /// This event works with the raw CTI event and decomposes it into something 
        ///     that the UII system can use and react too. 
        /// </summary>
        /// <param name="e">CTI Event transport</param>
        public override void OnUpdateAgentStatusEvent(CtiCoreEventArgs e)
        {

            // Here you will need to cast the event from the CTI system into the vendor system type
            // The Cit Vendor Types would ideally be defined in Vendor provided lib’s and stored in the “support folder”
            // Provided here is an example of what you can do with this area
            /*
            // Here we are making some determination as the agent state based on the state provided by the CTI system
            CtiVendor_AgentStateChange stChangeEvent = (CtiVendor_AgentStateChange)e.EventInfo;
            if (stChangeEvent != null)
            {
                
                if (stChangeEvent.State == AgentState.Undefined)
                {
                    SendLoggedOut();
                }
                
                if (stChangeEvent.State == AgentState.Initialized)
                {
                    // This is a customized command that is vendor specific, in this case it would be used to load the agent configuration variables. 
                    LoadGlobalAgentConfig();  
             
                    // This is a command raised by UII to send an agent logged out event to the UII system
                    SendLoggedOut(); 
                }
                // Raise Logged out event. 
                if (stChangeEvent.State == AgentState.LoggedOut)
                {
                    // This is a command raised by UII to send an agent logged out event to the UII system
                    SendLoggedOut();
                }
                // Raised Agent Away event
                if (stChangeEvent.State == AgentState.Released)
                {
                    // This is a command raised by UII to send an agent unavailable event to the UII system
                    SendUnavailable();
                }

                // Raise Available Event. 
                if (stChangeEvent.State == AgentState.Available)
                {
                    // This is a command raised by UII to send an agent available out event to the UII system
                    SendAvailable();
                }
                // Raise Unavailable event. 
                if (stChangeEvent.State == AgentState.ReleasePending)
                {
                    // This is a command raised by UII to send an agent unavailable event to the UII system
                    SendUnavailable();
                }
            }
             */
        }

    }
}
