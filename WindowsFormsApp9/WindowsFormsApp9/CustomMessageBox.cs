using System;
using System.Drawing;
using System.Windows.Forms;

public class CustomMessageBox : Form
{
    public CustomMessageBox(string title, string message)
    {
        // Form ayarları
        this.Text = title;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(400, 250);
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.FromArgb(40, 44, 52);
        this.Padding = new Padding(10);

        // Panel arka plan
        Panel panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(50, 54, 62)
        };
        this.Controls.Add(panel);

        // Başlık etiketi
        Label lblTitle = new Label
        {
            Text = title,
            Font = new Font("Arial", 18, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter
        };
        panel.Controls.Add(lblTitle);

        // Mesaj etiketi
        Label lblMessage = new Label
        {
            Text = message,
            Font = new Font("Arial", 12),
            ForeColor = Color.WhiteSmoke,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(10)
        };
        panel.Controls.Add(lblMessage);

        // Kapat düğmesi
        Button btnClose = new Button
        {
            Text = "Tamam",
            Font = new Font("Arial", 12, FontStyle.Bold),
            BackColor = Color.FromArgb(70, 130, 180),
            ForeColor = Color.White,
            Dock = DockStyle.Bottom,
            Height = 40,
            FlatStyle = FlatStyle.Flat
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => this.Close();
        panel.Controls.Add(btnClose);
    }

    public static void Show(string title, string message)
    {
        CustomMessageBox msgBox = new CustomMessageBox(title, message);
        msgBox.ShowDialog();
    }
}
