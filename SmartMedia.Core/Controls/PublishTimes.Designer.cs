namespace SmartMedia.Core.Controls
{
    partial class PublishTimes
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
            gbTimer = new GroupBox();
            label5 = new Label();
            label2 = new Label();
            label1 = new Label();
            cbEndHour = new ComboBox();
            cbStartHour = new ComboBox();
            cbPushNumber = new ComboBox();
            label4 = new Label();
            cbWeek = new ComboBox();
            dtPushTime = new DateTimePicker();
            cbIsOpenTimer = new XS.WinFormsTools.Controls.CheckBox();
            gbTimer.SuspendLayout();
            SuspendLayout();
            // 
            // gbTimer
            // 
            gbTimer.Controls.Add(label5);
            gbTimer.Controls.Add(label2);
            gbTimer.Controls.Add(label1);
            gbTimer.Controls.Add(cbEndHour);
            gbTimer.Controls.Add(cbStartHour);
            gbTimer.Controls.Add(cbPushNumber);
            gbTimer.Controls.Add(label4);
            gbTimer.Controls.Add(cbWeek);
            gbTimer.Controls.Add(dtPushTime);
            gbTimer.Location = new Point(2, 39);
            gbTimer.Name = "gbTimer";
            gbTimer.Size = new Size(391, 158);
            gbTimer.TabIndex = 51;
            gbTimer.TabStop = false;
            gbTimer.Text = "这里是批量设置，导入数据你还可单独修改";
            // 
            // label5
            // 
            label5.ForeColor = SystemColors.ControlDark;
            label5.Location = new Point(11, 82);
            label5.Name = "label5";
            label5.Size = new Size(145, 57);
            label5.TabIndex = 82;
            label5.Text = "起始与结束时间一样为准时发布，否则在时点之间随机发布";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(165, 76);
            label2.Name = "label2";
            label2.Size = new Size(20, 17);
            label2.TabIndex = 81;
            label2.Text = "从";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(265, 76);
            label1.Name = "label1";
            label1.Size = new Size(20, 17);
            label1.TabIndex = 80;
            label1.Text = "到";
            // 
            // cbEndHour
            // 
            cbEndHour.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEndHour.FormattingEnabled = true;
            cbEndHour.Location = new Point(265, 96);
            cbEndHour.Name = "cbEndHour";
            cbEndHour.Size = new Size(94, 25);
            cbEndHour.TabIndex = 79;
            // 
            // cbStartHour
            // 
            cbStartHour.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStartHour.FormattingEnabled = true;
            cbStartHour.Location = new Point(165, 96);
            cbStartHour.Name = "cbStartHour";
            cbStartHour.Size = new Size(94, 25);
            cbStartHour.TabIndex = 78;
            // 
            // cbPushNumber
            // 
            cbPushNumber.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPushNumber.FormattingEnabled = true;
            cbPushNumber.Location = new Point(265, 38);
            cbPushNumber.Name = "cbPushNumber";
            cbPushNumber.Size = new Size(94, 25);
            cbPushNumber.TabIndex = 77;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(138, 41);
            label4.Name = "label4";
            label4.Size = new Size(20, 17);
            label4.TabIndex = 76;
            label4.Text = "起";
            // 
            // cbWeek
            // 
            cbWeek.DropDownStyle = ComboBoxStyle.DropDownList;
            cbWeek.FormattingEnabled = true;
            cbWeek.Location = new Point(163, 37);
            cbWeek.Name = "cbWeek";
            cbWeek.Size = new Size(94, 25);
            cbWeek.TabIndex = 75;
            // 
            // dtPushTime
            // 
            dtPushTime.Location = new Point(13, 38);
            dtPushTime.Name = "dtPushTime";
            dtPushTime.Size = new Size(120, 23);
            dtPushTime.TabIndex = 74;
            // 
            // cbIsOpenTimer
            // 
            cbIsOpenTimer.AutoSize = true;
            cbIsOpenTimer.Font = new Font("Segoe UI", 12F);
            cbIsOpenTimer.Location = new Point(3, 6);
            cbIsOpenTimer.Name = "cbIsOpenTimer";
            cbIsOpenTimer.Size = new Size(133, 20);
            cbIsOpenTimer.TabIndex = 50;
            cbIsOpenTimer.Text = "是否定时发布";
            cbIsOpenTimer.UseVisualStyleBackColor = true;
            cbIsOpenTimer.CheckedChanged += cbIsOpenTimer_CheckedChanged;
            // 
            // PublishTimes
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(gbTimer);
            Controls.Add(cbIsOpenTimer);
            Name = "PublishTimes";
            Size = new Size(397, 200);
            gbTimer.ResumeLayout(false);
            gbTimer.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox gbTimer;
        private Label label5;
        private Label label2;
        private Label label1;
        private ComboBox cbEndHour;
        private ComboBox cbStartHour;
        private ComboBox cbPushNumber;
        private Label label4;
        private ComboBox cbWeek;
        private DateTimePicker dtPushTime;
        private XS.WinFormsTools.Controls.CheckBox cbIsOpenTimer;
    }
}
