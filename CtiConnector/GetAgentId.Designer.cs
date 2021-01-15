namespace Microsoft.Crm.Accelerator.Cca.Cti.Samples.CtiRoot
{
	partial class GetAgentId
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetAgentId));
			this.lbAgentId = new System.Windows.Forms.Label();
			this.txtAgentId = new System.Windows.Forms.TextBox();
			this.Submit = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbAgentId
			// 
			resources.ApplyResources(this.lbAgentId, "lbAgentId");
			this.lbAgentId.Name = "lbAgentId";
			// 
			// txtAgentId
			// 
			resources.ApplyResources(this.txtAgentId, "txtAgentId");
			this.txtAgentId.Name = "txtAgentId";
			// 
			// Submit
			// 
			resources.ApplyResources(this.Submit, "Submit");
			this.Submit.Name = "Submit";
			this.Submit.UseVisualStyleBackColor = true;
			this.Submit.Click += new System.EventHandler(this.Submit_Click);
			// 
			// Cancel
			// 
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.Cancel, "Cancel");
			this.Cancel.Name = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			// 
			// GetAgentId
			// 
			this.AcceptButton = this.Submit;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.Submit);
			this.Controls.Add(this.txtAgentId);
			this.Controls.Add(this.lbAgentId);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GetAgentId";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.TopMost = true;
			this.Load += new System.EventHandler(this.GetAgentId_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbAgentId;
		private System.Windows.Forms.TextBox txtAgentId;
		private System.Windows.Forms.Button Submit;
		private System.Windows.Forms.Button Cancel;
	}
}