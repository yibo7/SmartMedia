namespace SmartMedia.Core.Controls
{
    partial class PublishTimer
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            dtPushTime = new DateTimePicker();
            cbPushFromTime = new XS.WinFormsTools.Controls.CheckBox();
            SuspendLayout();
            // 
            // dtPushTime
            // 
            dtPushTime.Enabled = false;
            dtPushTime.Location = new Point(12, 40);
            dtPushTime.Name = "dtPushTime";
            dtPushTime.Size = new Size(200, 23);
            dtPushTime.TabIndex = 73;
            // 
            // cbPushFromTime
            // 
            cbPushFromTime.AutoSize = true;
            cbPushFromTime.Font = new Font("Segoe UI", 12F);
            cbPushFromTime.Location = new Point(12, 12);
            cbPushFromTime.Name = "cbPushFromTime";
            cbPushFromTime.Size = new Size(133, 20);
            cbPushFromTime.TabIndex = 72;
            cbPushFromTime.Text = "是否定时发布";
            cbPushFromTime.UseVisualStyleBackColor = true;
            // 
            // PublishTimer
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dtPushTime);
            Controls.Add(cbPushFromTime);
            Name = "PublishTimer";
            Size = new Size(233, 84);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DateTimePicker dtPushTime;
        private XS.WinFormsTools.Controls.CheckBox cbPushFromTime;
    }
}
