namespace SmartMedia
{
    partial class frmSplash
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
            picStart = new PictureBox();
            lbStatus = new Label();
            lvVerson = new Label();
            ((System.ComponentModel.ISupportInitialize)picStart).BeginInit();
            SuspendLayout();
            // 
            // picStart
            // 
            picStart.BackColor = Color.Transparent;
            picStart.Dock = DockStyle.Fill;
            picStart.Image = Resource.启动图片;
            picStart.Location = new Point(0, 0);
            picStart.Name = "picStart";
            picStart.Size = new Size(502, 305);
            picStart.TabIndex = 2;
            picStart.TabStop = false;
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            lbStatus.BackColor = Color.Transparent;
            lbStatus.ForeColor = SystemColors.ActiveCaptionText;
            lbStatus.Location = new Point(127, 203);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new Size(53, 17);
            lbStatus.TabIndex = 3;
            lbStatus.Text = "加载中...";
            // 
            // lvVerson
            // 
            lvVerson.BackColor = Color.Transparent;
            lvVerson.FlatStyle = FlatStyle.Flat;
            lvVerson.Font = new Font("Microsoft YaHei UI", 26.25F);
            lvVerson.ForeColor = Color.White;
            lvVerson.Location = new Point(359, 112);
            lvVerson.Name = "lvVerson";
            lvVerson.Size = new Size(70, 46);
            lvVerson.TabIndex = 4;
            lvVerson.Text = "1.0";
            // 
            // frmSplash
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(502, 305);
            Controls.Add(lvVerson);
            Controls.Add(lbStatus);
            Controls.Add(picStart);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmSplash";
            Text = "frmSplash";
            TransparencyKey = Color.Transparent;
            ((System.ComponentModel.ISupportInitialize)picStart).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox picStart;
        private Label lbStatus;
        private Label lvVerson;
    }
}