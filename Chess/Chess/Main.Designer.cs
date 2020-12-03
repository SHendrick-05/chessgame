namespace Chess
{
    partial class Main
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
            this.top = new System.Windows.Forms.Panel();
            this.close = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.Label();
            this.play = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.source = new System.Windows.Forms.Button();
            this.music = new System.Windows.Forms.Button();
            this.top.SuspendLayout();
            this.SuspendLayout();
            // 
            // top
            // 
            this.top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(40)))));
            this.top.Controls.Add(this.close);
            this.top.Controls.Add(this.title);
            this.top.Dock = System.Windows.Forms.DockStyle.Top;
            this.top.Location = new System.Drawing.Point(0, 0);
            this.top.Name = "top";
            this.top.Size = new System.Drawing.Size(704, 78);
            this.top.TabIndex = 0;
            this.top.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Drag);
            // 
            // close
            // 
            this.close.Dock = System.Windows.Forms.DockStyle.Right;
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close.Font = new System.Drawing.Font("Century Gothic", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close.ForeColor = System.Drawing.SystemColors.Control;
            this.close.Location = new System.Drawing.Point(626, 0);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(78, 78);
            this.close.TabIndex = 1;
            this.close.Text = "X";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.CloseApp);
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Century Gothic", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.ForeColor = System.Drawing.SystemColors.Control;
            this.title.Location = new System.Drawing.Point(12, 23);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(300, 36);
            this.title.TabIndex = 0;
            this.title.Text = "Simple Chess Game";
            this.title.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Drag);
            // 
            // play
            // 
            this.play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.play.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.play.ForeColor = System.Drawing.SystemColors.Control;
            this.play.Location = new System.Drawing.Point(18, 318);
            this.play.Name = "play";
            this.play.Size = new System.Drawing.Size(178, 42);
            this.play.TabIndex = 1;
            this.play.Text = "Play";
            this.play.UseVisualStyleBackColor = true;
            this.play.Click += new System.EventHandler(this.Play);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(40)))));
            this.textBox1.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.Control;
            this.textBox1.Location = new System.Drawing.Point(18, 84);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(674, 228);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "-Finished some check edge cases\r\n-Added promotion choice\r\n\r\n-TODO:\r\nEdge cases\r\nD" +
    "raw offer\r\n\r\nBy Sean Hendrick 10P1";
            // 
            // source
            // 
            this.source.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.source.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.source.ForeColor = System.Drawing.SystemColors.Control;
            this.source.Location = new System.Drawing.Point(514, 318);
            this.source.Name = "source";
            this.source.Size = new System.Drawing.Size(178, 42);
            this.source.TabIndex = 3;
            this.source.Text = "View the source";
            this.source.UseVisualStyleBackColor = true;
            this.source.Click += new System.EventHandler(this.source_Click);
            // 
            // music
            // 
            this.music.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.music.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.music.ForeColor = System.Drawing.SystemColors.Control;
            this.music.Location = new System.Drawing.Point(268, 318);
            this.music.Name = "music";
            this.music.Size = new System.Drawing.Size(178, 42);
            this.music.TabIndex = 4;
            this.music.Text = "Music off";
            this.music.UseVisualStyleBackColor = true;
            this.music.Click += new System.EventHandler(this.music_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(704, 405);
            this.Controls.Add(this.music);
            this.Controls.Add(this.source);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.play);
            this.Controls.Add(this.top);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Main";
            this.Text = "Main";
            this.top.ResumeLayout(false);
            this.top.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel top;
        private System.Windows.Forms.Button play;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button source;
        private System.Windows.Forms.Button music;

    }
}

