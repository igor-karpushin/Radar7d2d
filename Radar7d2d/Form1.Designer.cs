namespace Radar7d2d
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.right_panel = new System.Windows.Forms.Panel();
            this.playersList = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.right_panel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // right_panel
            // 
            this.right_panel.Controls.Add(this.playersList);
            this.right_panel.Dock = System.Windows.Forms.DockStyle.Right;
            this.right_panel.Location = new System.Drawing.Point(643, 0);
            this.right_panel.Name = "right_panel";
            this.right_panel.Size = new System.Drawing.Size(151, 450);
            this.right_panel.TabIndex = 4;
            // 
            // playersList
            // 
            this.playersList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.playersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playersList.FormattingEnabled = true;
            this.playersList.Location = new System.Drawing.Point(0, 0);
            this.playersList.Name = "playersList";
            this.playersList.Size = new System.Drawing.Size(151, 450);
            this.playersList.TabIndex = 2;
            this.playersList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.playersList_MouseDoubleClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.webView21);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(643, 450);
            this.panel3.TabIndex = 5;
            // 
            // webView21
            // 
            this.webView21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView21.Location = new System.Drawing.Point(0, 0);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(643, 450);
            this.webView21.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            this.webView21.TabIndex = 0;
            this.webView21.Text = "webView21";
            this.webView21.ZoomFactor = 1D;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 450);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.right_panel);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Radar7d2d v2.1";
            this.right_panel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel right_panel;
        private System.Windows.Forms.Panel panel3;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.ListBox playersList;
    }
}

