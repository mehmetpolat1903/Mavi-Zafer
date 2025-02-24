using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MaviZafer
{
    public class frmSignIn : Form
    {
        private TextBox txtPlayerName;
        private TextBox txtPassword;
        private Button btnStart;
        private Button btnBack;
        private Label lblPlayerName;
        private Label lblPassword;

        private string connectionString = "Data Source=DESKTOP-1BSONIC\\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True;";
        private int loggedInUserId = 0; // Kullanıcı ID'si
        private string loggedInUsername = string.Empty; // Kullanıcı adı

        public frmSignIn()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form ayarları
            this.Text = "Giriş Yap";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Arka plan resmi
            this.BackgroundImage = Image.FromFile("C:\\Users\\MEHMET POLAT\\Downloads\\bs.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Dinamik merkezi konumlandırma
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            // Oyuncu Adı TextBox
            txtPlayerName = new TextBox
            {
                Width = 300,
                ForeColor = Color.Gray,
                Text = "Kullanıcı Adı",
                Font = new Font("Arial", 12),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(centerX - 150, centerY - 100)
            };
            txtPlayerName.GotFocus += RemoveText;
            txtPlayerName.LostFocus += AddText;
            this.Controls.Add(txtPlayerName);

            // Oyuncu Adı Label
            lblPlayerName = new Label
            {
                Text = "Kullanıcı Adı:",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(centerX - 150, centerY - 130)
            };
            this.Controls.Add(lblPlayerName);

            // Şifre TextBox
            txtPassword = new TextBox
            {
                Width = 300,
                ForeColor = Color.Gray,
                Text = "Şifre",
                Font = new Font("Arial", 12),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '\0',
                Location = new Point(centerX - 150, centerY - 30)
            };
            txtPassword.GotFocus += RemoveText;
            txtPassword.LostFocus += AddText;
            this.Controls.Add(txtPassword);

            // Şifre Label
            lblPassword = new Label
            {
                Text = "Şifre:",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(centerX - 150, centerY - 60)
            };
            this.Controls.Add(lblPassword);

            // Başla Butonu
            btnStart = new Button
            {
                Text = "Başla",
                BackColor = Color.Blue,
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Width = 300,
                Height = 50,
                Location = new Point(centerX - 150, centerY + 50)
            };
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            // Geri Butonu
            btnBack = new Button
            {
                Text = "Geri",
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                FlatStyle = FlatStyle.Flat,
                Width = 300,
                Height = 40,
                Location = new Point(centerX - 150, centerY + 120)
            };
            btnBack.Click += BtnBack_Click;
            btnBack.MouseEnter += BtnBack_MouseEnter;
            btnBack.MouseLeave += BtnBack_MouseLeave;
            this.Controls.Add(btnBack);

            // Formun yeniden boyutlanma olayı
            this.Resize += FrmSignIn_Resize;
        }

        private void FrmSignIn_Resize(object sender, EventArgs e)
        {
            // Form yeniden boyutlandığında öğeleri merkezle
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            lblPlayerName.Location = new Point(centerX - 150, centerY - 130);
            txtPlayerName.Location = new Point(centerX - 150, centerY - 100);
            lblPassword.Location = new Point(centerX - 150, centerY - 60);
            txtPassword.Location = new Point(centerX - 150, centerY - 30);
            btnStart.Location = new Point(centerX - 150, centerY + 50);
            btnBack.Location = new Point(centerX - 150, centerY + 120);
        }

        private void RemoveText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.ForeColor == Color.Gray)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;

                // Şifre alanında karakter gizleme
                if (textBox == txtPassword)
                    textBox.PasswordChar = '*';
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.ForeColor = Color.Gray;

                if (textBox == txtPlayerName)
                    textBox.Text = "Oyuncu Adı";
                else if (textBox == txtPassword)
                {
                    textBox.Text = "Şifre";
                    textBox.PasswordChar = '\0';
                }
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            string playerName = txtPlayerName.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(playerName) || playerName == "Oyuncu Adı" ||
                string.IsNullOrWhiteSpace(password) || password == "Şifre")
            {
                MessageBox.Show("Lütfen oyuncu adınızı ve şifrenizi girin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateCredentials(playerName, password))
            {
                frmSecim secimForm = new frmSecim(loggedInUserId, loggedInUsername);
                secimForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateCredentials(string playerName, string password)
        {
            bool isValid = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT UserId, UserName FROM Users WHERE UserName = @UserName AND Password = @Password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserName", playerName);
                    command.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        loggedInUserId = reader.GetInt32(0);
                        loggedInUsername = reader.GetString(1);
                        isValid = true;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return isValid;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            frmLogin loginForm = new frmLogin();
            loginForm.Show();
            this.Close();
        }

        private void BtnBack_MouseEnter(object sender, EventArgs e)
        {
            btnBack.BackColor = Color.Red;
        }

        private void BtnBack_MouseLeave(object sender, EventArgs e)
        {
            btnBack.BackColor = Color.Gray;
        }
    }
}
