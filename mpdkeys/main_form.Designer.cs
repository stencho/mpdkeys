namespace mpdkeys {
    partial class main_form {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.connect_button = new System.Windows.Forms.Button();
            this.host_text_box = new System.Windows.Forms.TextBox();
            this.host_label = new System.Windows.Forms.Label();
            this.port_label = new System.Windows.Forms.Label();
            this.port_nud = new System.Windows.Forms.NumericUpDown();
            this.mpd_connection_label = new System.Windows.Forms.Label();
            this.tray_icon = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.port_nud)).BeginInit();
            this.SuspendLayout();
            // 
            // connect_button
            // 
            this.connect_button.Location = new System.Drawing.Point(119, 38);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(157, 23);
            this.connect_button.TabIndex = 2;
            this.connect_button.Text = "Connect to server";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // host_text_box
            // 
            this.host_text_box.Location = new System.Drawing.Point(42, 12);
            this.host_text_box.Name = "host_text_box";
            this.host_text_box.Size = new System.Drawing.Size(127, 20);
            this.host_text_box.TabIndex = 0;
            this.host_text_box.Text = "127.0.0.1";
            this.host_text_box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.host_text_box_KeyDown);
            // 
            // host_label
            // 
            this.host_label.AutoSize = true;
            this.host_label.Location = new System.Drawing.Point(13, 15);
            this.host_label.Name = "host_label";
            this.host_label.Size = new System.Drawing.Size(29, 13);
            this.host_label.TabIndex = 6;
            this.host_label.Text = "Host";
            // 
            // port_label
            // 
            this.port_label.AutoSize = true;
            this.port_label.Location = new System.Drawing.Point(183, 15);
            this.port_label.Name = "port_label";
            this.port_label.Size = new System.Drawing.Size(26, 13);
            this.port_label.TabIndex = 7;
            this.port_label.Text = "Port";
            // 
            // port_nud
            // 
            this.port_nud.Location = new System.Drawing.Point(209, 13);
            this.port_nud.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.port_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.port_nud.Name = "port_nud";
            this.port_nud.Size = new System.Drawing.Size(66, 20);
            this.port_nud.TabIndex = 1;
            this.port_nud.Value = new decimal(new int[] {
            6600,
            0,
            0,
            0});
            this.port_nud.KeyDown += new System.Windows.Forms.KeyEventHandler(this.port_nud_KeyDown);
            // 
            // mpd_connection_label
            // 
            this.mpd_connection_label.AutoSize = true;
            this.mpd_connection_label.ForeColor = System.Drawing.Color.DarkRed;
            this.mpd_connection_label.Location = new System.Drawing.Point(12, 43);
            this.mpd_connection_label.Name = "mpd_connection_label";
            this.mpd_connection_label.Size = new System.Drawing.Size(94, 13);
            this.mpd_connection_label.TabIndex = 9;
            this.mpd_connection_label.Text = "mpd disconnected";
            this.mpd_connection_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tray_icon
            // 
            this.tray_icon.Text = "MPDKeys";
            // 
            // main_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 70);
            this.Controls.Add(this.port_nud);
            this.Controls.Add(this.host_text_box);
            this.Controls.Add(this.mpd_connection_label);
            this.Controls.Add(this.port_label);
            this.Controls.Add(this.host_label);
            this.Controls.Add(this.connect_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "main_form";
            this.Text = "mpdkeys";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.config_form_FormClosing);
            this.Load += new System.EventHandler(this.config_form_Load);
            this.Resize += new System.EventHandler(this.main_form_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.port_nud)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.TextBox host_text_box;
        private System.Windows.Forms.Label host_label;
        private System.Windows.Forms.Label port_label;
        private System.Windows.Forms.NumericUpDown port_nud;
        private System.Windows.Forms.Label mpd_connection_label;
        private System.Windows.Forms.NotifyIcon tray_icon;
    }
}

