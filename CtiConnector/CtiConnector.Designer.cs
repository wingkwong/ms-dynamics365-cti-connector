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

namespace CtiConnector
{
    partial class CtiConnector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Microsoft.Uii.Csr.Context context1 = new Microsoft.Uii.Csr.Context();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvEventList = new System.Windows.Forms.ListView();
            this.TimeStamp = new System.Windows.Forms.ColumnHeader();
            this.EventName = new System.Windows.Forms.ColumnHeader();
            this.eventDetail = new System.Windows.Forms.RichTextBox();
            this.EventActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.EventActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(856, 546);
            this.splitContainer1.SplitterDistance = 317;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvEventList);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 546);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Events";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.eventDetail);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(535, 546);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Event Detail";
            // 
            // lvEventList
            // 
            this.lvEventList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TimeStamp,
            this.EventName});
            this.lvEventList.ContextMenuStrip = this.EventActions;
            this.lvEventList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvEventList.FullRowSelect = true;
            this.lvEventList.GridLines = true;
            this.lvEventList.Location = new System.Drawing.Point(3, 16);
            this.lvEventList.MultiSelect = false;
            this.lvEventList.Name = "lvEventList";
            this.lvEventList.Size = new System.Drawing.Size(311, 527);
            this.lvEventList.TabIndex = 0;
            this.lvEventList.UseCompatibleStateImageBehavior = false;
            this.lvEventList.View = System.Windows.Forms.View.Details;
            this.lvEventList.SelectedIndexChanged += new System.EventHandler(lvEventList_SelectedIndexChanged);
            // 
            // TimeStamp
            // 
            this.TimeStamp.Text = "Time Stamp";
            this.TimeStamp.Width = 100;
            // 
            // EventName
            // 
            this.EventName.Text = "Event Name";
            this.EventName.Width = 206;
            // 
            // eventDetail
            // 
            this.eventDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventDetail.Location = new System.Drawing.Point(3, 16);
            this.eventDetail.Name = "eventDetail";
            this.eventDetail.Size = new System.Drawing.Size(529, 527);
            this.eventDetail.TabIndex = 0;
            this.eventDetail.Text = "";
            // 
            // EventActions
            // 
            this.EventActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearItemsToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveEventsToolStripMenuItem});
            this.EventActions.Name = "EventActions";
            this.EventActions.Size = new System.Drawing.Size(146, 54);
            this.EventActions.Text = "Diag Event";
            // 
            // clearItemsToolStripMenuItem
            // 
            this.clearItemsToolStripMenuItem.Name = "clearItemsToolStripMenuItem";
            this.clearItemsToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.clearItemsToolStripMenuItem.Text = "Clear Items";
            this.clearItemsToolStripMenuItem.Click += new System.EventHandler(clearItemsToolStripMenuItem_Click);

            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(142, 6);
            // 
            // saveEventsToolStripMenuItem
            // 
            this.saveEventsToolStripMenuItem.Enabled = false;
            this.saveEventsToolStripMenuItem.Name = "saveEventsToolStripMenuItem";
            this.saveEventsToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.saveEventsToolStripMenuItem.Text = "Save Events";
            // 
            // CosmoCtiRoot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            context1.ContextInformation = "<UiiContext />";
            this.Context = context1;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CosmoCtiRoot";
            this.Size = new System.Drawing.Size(856, 546);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.EventActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvEventList;
        private System.Windows.Forms.ColumnHeader TimeStamp;
        private System.Windows.Forms.ColumnHeader EventName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox eventDetail;
        private System.Windows.Forms.ContextMenuStrip EventActions;
        private System.Windows.Forms.ToolStripMenuItem clearItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveEventsToolStripMenuItem;
    }
}
