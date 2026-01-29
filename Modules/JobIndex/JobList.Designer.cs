using SmartMedia.Controls;
using XS.WinFormsTools.XsListView;

namespace SmartMedia.Modules.Job
{
    partial class JobList
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
            components = new System.ComponentModel.Container();
            toolMainBar = new XsToolBar();
            btnPassAll = new ToolStripButton();
            btnRefesh = new ToolStripButton();
            lvData = new XsListViewBox();
            ucSplitLine_h1 = new XS.WinFormsTools.Forms.UCSplitLine_H();
            splitContainer1 = new SplitContainer();
            label1 = new Label();
            lbTags = new Label();
            txtDes = new RichTextBox();
            picCover = new XS.WinFormsTools.Controls.PictureBox();
            toolStrip2 = new XsToolBar();
            btnJobImg = new ToolStripButton();
            lbJobName = new ToolStripLabel();
            toolMainBar.SuspendLayout();
            lvData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCover).BeginInit();
            toolStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // toolMainBar
            // 
            toolMainBar.GripMargin = new Padding(3);
            toolMainBar.Items.AddRange(new ToolStripItem[] { btnPassAll, btnRefesh });
            toolMainBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolMainBar.Location = new Point(0, 0);
            toolMainBar.Name = "toolMainBar";
            toolMainBar.Size = new Size(1062, 39);
            toolMainBar.TabIndex = 2;
            toolMainBar.Text = "toolStrip1";
            // 
            // btnPassAll
            // 
            btnPassAll.Image = Resource.spand;
            btnPassAll.ImageScaling = ToolStripItemImageScaling.None;
            btnPassAll.ImageTransparentColor = Color.Magenta;
            btnPassAll.Name = "btnPassAll";
            btnPassAll.Size = new Size(92, 36);
            btnPassAll.Text = "暂停任务";
            btnPassAll.Click += btnPassAll_Click;
            // 
            // btnRefesh
            // 
            btnRefesh.Image = Resource.refesh;
            btnRefesh.ImageScaling = ToolStripItemImageScaling.None;
            btnRefesh.ImageTransparentColor = Color.Magenta;
            btnRefesh.Name = "btnRefesh";
            btnRefesh.Size = new Size(68, 36);
            btnRefesh.Text = "刷新";
            btnRefesh.Click += btnRefesh_Click;
            // 
            // lvData
            // 
            lvData.BorderStyle = BorderStyle.None;
            lvData.Controls.Add(ucSplitLine_h1);
            lvData.Dock = DockStyle.Fill;
            lvData.Font = new Font("Microsoft YaHei UI", 9F);
            lvData.FullRowSelect = true;
            lvData.GridLines = true;
            lvData.HeaderBackgroundColor = Color.AliceBlue;
            lvData.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvData.Location = new Point(0, 0);
            lvData.Name = "lvData";
            lvData.OwnerDraw = true;
            lvData.Size = new Size(827, 452);
            lvData.TabIndex = 3;
            lvData.UseCompatibleStateImageBehavior = false;
            lvData.View = View.Details;
            // 
            // ucSplitLine_h1
            // 
            ucSplitLine_h1.BackColor = Color.FromArgb(232, 232, 232);
            ucSplitLine_h1.Dock = DockStyle.Top;
            ucSplitLine_h1.Location = new Point(0, 0);
            ucSplitLine_h1.Name = "ucSplitLine_h1";
            ucSplitLine_h1.Size = new Size(827, 1);
            ucSplitLine_h1.TabIndex = 1;
            ucSplitLine_h1.TabStop = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 39);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvData);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(lbTags);
            splitContainer1.Panel2.Controls.Add(txtDes);
            splitContainer1.Panel2.Controls.Add(picCover);
            splitContainer1.Panel2.Controls.Add(toolStrip2);
            splitContainer1.Size = new Size(1062, 452);
            splitContainer1.SplitterDistance = 827;
            splitContainer1.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 207);
            label1.Name = "label1";
            label1.Size = new Size(32, 17);
            label1.TabIndex = 69;
            label1.Text = "简介";
            // 
            // lbTags
            // 
            lbTags.AutoSize = true;
            lbTags.Location = new Point(17, 169);
            lbTags.Name = "lbTags";
            lbTags.Size = new Size(44, 17);
            lbTags.TabIndex = 68;
            lbTags.Text = "标签：";
            // 
            // txtDes
            // 
            txtDes.BorderStyle = BorderStyle.None;
            txtDes.Location = new Point(17, 240);
            txtDes.Name = "txtDes";
            txtDes.Size = new Size(202, 89);
            txtDes.TabIndex = 67;
            txtDes.Text = "";
            // 
            // picCover
            // 
            picCover.BackColor = Color.WhiteSmoke;
            picCover.ErrorColor = Color.FromArgb(255, 240, 240);
            picCover.LoadedColor = Color.FromArgb(245, 245, 245);
            picCover.LoadingColor = Color.FromArgb(240, 240, 240);
            picCover.Location = new Point(26, 45);
            picCover.Name = "picCover";
            picCover.ShowLoadingIndicator = true;
            picCover.Size = new Size(155, 100);
            picCover.TabIndex = 66;
            picCover.TabStop = false;
            // 
            // toolStrip2
            // 
            toolStrip2.GripMargin = new Padding(3);
            toolStrip2.Items.AddRange(new ToolStripItem[] { btnJobImg, lbJobName });
            toolStrip2.Location = new Point(0, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.RenderMode = ToolStripRenderMode.System;
            toolStrip2.Size = new Size(231, 25);
            toolStrip2.TabIndex = 18;
            toolStrip2.Text = "toolStrip2";
            // 
            // btnJobImg
            // 
            btnJobImg.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnJobImg.Image = Resource.doc;
            btnJobImg.ImageTransparentColor = Color.Magenta;
            btnJobImg.Name = "btnJobImg";
            btnJobImg.Size = new Size(23, 22);
            btnJobImg.Text = "toolStripButton1";
            // 
            // lbJobName
            // 
            lbJobName.BackColor = Color.White;
            lbJobName.Name = "lbJobName";
            lbJobName.Size = new Size(56, 22);
            lbJobName.Text = "任务名称";
            // 
            // JobList
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1062, 491);
            Controls.Add(splitContainer1);
            Controls.Add(toolMainBar);
            Name = "JobList";
            Text = "定时发布任务";
            toolMainBar.ResumeLayout(false);
            toolMainBar.PerformLayout();
            lvData.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picCover).EndInit();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private XsToolBar toolMainBar;
        private XsListViewBox lvData;
        private ToolStripButton btnPassAll;
        private SplitContainer splitContainer1;
        private XsToolBar toolStrip2;
        private ToolStripLabel lbJobName;
        private ToolStripButton btnJobImg;
        private XS.WinFormsTools.Forms.UCSplitLine_H ucSplitLine_h1;
        private ToolStripButton btnRefesh;
        private XS.WinFormsTools.Controls.PictureBox picCover;
        private RichTextBox txtDes;
        private Label lbTags;
        private Label label1;
    }
}