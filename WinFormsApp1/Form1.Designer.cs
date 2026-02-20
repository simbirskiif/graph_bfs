namespace WinFormsApp1;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        picture = new System.Windows.Forms.PictureBox();
        ((System.ComponentModel.ISupportInitialize)picture).BeginInit();
        SuspendLayout();
        // 
        // picture
        // 
        picture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        picture.BackColor = System.Drawing.SystemColors.ActiveCaption;
        picture.Location = new System.Drawing.Point(0, 0);
        picture.Name = "picture";
        picture.Size = new System.Drawing.Size(600, 450);
        picture.TabIndex = 0;
        picture.TabStop = false;
        picture.Paint += picture_Paint;
        picture.MouseDown += picture_MouseDown;
        picture.MouseMove += picture_MouseMove;
        picture.MouseUp += picture_MouseUp;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(picture);
        DoubleBuffered = true;
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)picture).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.PictureBox picture;

    #endregion
}