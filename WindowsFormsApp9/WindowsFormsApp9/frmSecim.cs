using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace MaviZafer
{
    public partial class frmSecim : Form
    {
        private Button btnPlayGame;
        private Button btnViewScores;
        private Button btnLogout;  // Logout button
        private Label lblWelcome;
        private int userId;
        private string username;
        private string connectionString = "Data Source=DESKTOP-1BSONIC\\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True;";

        public frmSecim(int userId, string username)
        {
            this.userId = userId;
            this.username = username;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Seçim Ekranı";
            this.WindowState = FormWindowState.Maximized; // Tam ekran modu
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Set background image
            this.BackgroundImage = Image.FromFile(@"C:\\Users\\MEHMET POLAT\\Downloads\\bs.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            lblWelcome = new Label
            {
                Text = $"Hoş geldiniz, {username}!",
                AutoSize = true,
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblWelcome);

            btnPlayGame = new Button
            {
                Text = "Oyun Oyna",
                Size = new Size(250, 60),
                BackColor = Color.FromArgb(0, 120, 215), // Blue color
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnPlayGame.Click += new EventHandler(BtnPlayGame_Click);
            this.Controls.Add(btnPlayGame);

            btnViewScores = new Button
            {
                Text = "Skorlar",
                Size = new Size(250, 60),
                BackColor = Color.FromArgb(30, 30, 30), // Dark Gray
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnViewScores.Click += new EventHandler(BtnViewScores_Click);
            this.Controls.Add(btnViewScores);

            // Logout button added
            btnLogout = new Button
            {
                Text = "Çıkış Yap",
                Size = new Size(250, 60),
                BackColor = Color.FromArgb(255, 69, 58), // Red color for logout button
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnLogout.Click += new EventHandler(BtnLogout_Click);  // Logout button click event
            this.Controls.Add(btnLogout);

            // Set rounded corners for buttons
            SetButtonRoundedCorners(btnPlayGame);
            SetButtonRoundedCorners(btnViewScores);
            SetButtonRoundedCorners(btnLogout);

            this.Load += FrmSecim_Load; // Form yüklendiğinde öğeleri hizala
        }

        private void FrmSecim_Load(object sender, EventArgs e)
        {
            // Formun ortasına göre öğeleri hizala
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            lblWelcome.Location = new Point(centerX - lblWelcome.Width / 2, centerY - 200);
            btnPlayGame.Location = new Point(centerX - btnPlayGame.Width / 2, centerY - 80);
            btnViewScores.Location = new Point(centerX - btnViewScores.Width / 2, centerY);
            btnLogout.Location = new Point(centerX - btnLogout.Width / 2, centerY + 80);
        }

        private void SetButtonRoundedCorners(Button button)
        {
            button.Region = new Region(CreateRoundRectangle(button.ClientRectangle, 20)); // Rounded corners with radius 20
        }

        private static GraphicsPath CreateRoundRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);  // Top-left corner
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);  // Top-right corner
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);  // Bottom-right corner
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);  // Bottom-left corner
            path.CloseFigure();
            return path;
        }

        private void BtnPlayGame_Click(object sender, EventArgs e)
        {
            frmDifficultySelect difficultyForm = new frmDifficultySelect(username, userId);
            difficultyForm.Show();
            this.Hide();
        }

        private void BtnViewScores_Click(object sender, EventArgs e)
        {
            ScoreBoard scoreBoard = new ScoreBoard();
            scoreBoard.LoadScoresFromDatabase(userId, connectionString);

            if (scoreBoard.Scores.Count > 0)
            {
                frmScores scoresForm = new frmScores(scoreBoard);
                scoresForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Henüz skor bulunmamaktadır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            frmLogin loginForm = new frmLogin();
            loginForm.Show();
            this.Close();  // Close the current form (frmSecim)
        }
    }

    public class ScoreBoard
    {
        public List<PlayerScore> Scores { get; private set; } = new List<PlayerScore>();

        public void LoadScoresFromDatabase(int userId, string connectionString)
        {
            Scores.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Username, TotalScore, DatePlayed FROM Scores WHERE UserID = @UserID ORDER BY DatePlayed DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Scores.Add(new PlayerScore(
                                    reader.GetString(0),     // Username
                                    reader.GetInt32(1),      // TotalScore
                                    reader.GetDateTime(2)    // DatePlayed
                                ));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Number} - {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class PlayerScore
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public DateTime GameDate { get; set; }

        public PlayerScore(string playerName, int score, DateTime gameDate)
        {
            PlayerName = playerName;
            Score = score;
            GameDate = gameDate;
        }
    }

    public partial class frmScores : Form
    {
        public frmScores(ScoreBoard scoreBoard)
        {
            InitializeComponent(scoreBoard);
        }

        private void InitializeComponent(ScoreBoard scoreBoard)
        {
            this.Text = "Skorlar";
            this.WindowState = FormWindowState.Maximized; // Uygulama tam ekran başlasın
            this.BackColor = Color.FromArgb(10, 25, 45);
            this.FormBorderStyle = FormBorderStyle.None; // Çerçeveyi kaldırarak tam ekran deneyimi sun

            int padding = 40;
            int listViewWidth = this.ClientSize.Width - padding * 2;
            int listViewHeight = this.ClientSize.Height - 200;

            ListView listView = new ListView
            {
                Location = new Point(padding, 100),
                Size = new Size(listViewWidth, listViewHeight),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(20, 30, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            listView.Columns.Add("Kullanıcı Adı", listViewWidth / 3, HorizontalAlignment.Left);
            listView.Columns.Add("Skor", listViewWidth / 6, HorizontalAlignment.Center);
            listView.Columns.Add("Tarih", listViewWidth / 3, HorizontalAlignment.Center);

            foreach (var score in scoreBoard.Scores)
            {
                ListViewItem item = new ListViewItem(new string[]
                {
            score.PlayerName,
            score.Score.ToString(),
            score.GameDate.ToString("dd/MM/yyyy")
                })
                {
                    BackColor = listView.Items.Count % 2 == 0 ? Color.FromArgb(0, 0, 128) : Color.Black
                };
                listView.Items.Add(item);
            }

            Button btnClose = new Button
            {
                Text = "Kapat",
                Width = 150,
                Height = 50,
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Location = new Point((this.ClientSize.Width - btnClose.Width) / 2, this.ClientSize.Height - btnClose.Height - padding);
            btnClose.Click += (sender, e) => this.Close();

            Label lblTitle = new Label
            {
                Text = "Skor Tablosu",
                ForeColor = Color.FromArgb(0, 120, 215),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(padding, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            Panel headerPanel = new Panel
            {
                BackColor = Color.FromArgb(15, 50, 100),
                Size = new Size(this.ClientSize.Width, 60),
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            headerPanel.Controls.Add(lblTitle);

            // Pencere yeniden boyutlandığında düzenlemenin doğru kalması için boyut değişikliği olayını bağla
            this.Resize += (sender, e) =>
            {
                listViewWidth = this.ClientSize.Width - padding * 2;
                listViewHeight = this.ClientSize.Height - 200;
                listView.Size = new Size(listViewWidth, listViewHeight);
                listView.Columns[0].Width = listViewWidth / 3;
                listView.Columns[1].Width = listViewWidth / 6;
                listView.Columns[2].Width = listViewWidth / 3;
                listView.Location = new Point(padding, 100);
                btnClose.Location = new Point((this.ClientSize.Width - btnClose.Width) / 2, this.ClientSize.Height - btnClose.Height - padding);
                headerPanel.Size = new Size(this.ClientSize.Width, 60);
            };

            this.Controls.Add(headerPanel);
            this.Controls.Add(listView);
            this.Controls.Add(btnClose);
        }
    }
}
