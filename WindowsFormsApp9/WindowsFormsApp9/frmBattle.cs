using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace MaviZafer
{
    public partial class frmBattle : Form
    {
        private readonly int gridSize;
        private readonly int[,] botState;
        private readonly Dictionary<int, int> botShipsHealth;
        private readonly bool[,] playerState;
        private readonly Button[,] playerBoard;
        private readonly Button[,] botBoard;
        private int CellSize;
        private Random random = new Random();
        private int playerHits = 0;
        private int botHits = 0;

        private readonly int userId;
        private readonly string username;
        private readonly string connectionString = "Data Source=DESKTOP-1BSONIC\\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True;";

        private readonly int[][] directions = new int[][]
        {
            new int[] { -1, 0 },
            new int[] { 1, 0 },
            new int[] { 0, -1 },
            new int[] { 0, 1 }
        };

        private ListBox lstStatus;

        public frmBattle(bool[,] playerState, int gridSize, int userId, string username)
        {
            this.gridSize = gridSize;
            this.playerState = playerState;
            this.botState = new int[gridSize, gridSize];
            this.botShipsHealth = new Dictionary<int, int>();
            this.playerBoard = new Button[gridSize, gridSize];
            this.botBoard = new Button[gridSize, gridSize];
            this.userId = userId;
            this.username = username;

            InitializeComponent();
            GenerateBotBoard();
        }

        private void InitializeComponent()
        {
            // Dinamik form boyutları
            int formWidth = Screen.PrimaryScreen.Bounds.Width;
            int formHeight = Screen.PrimaryScreen.Bounds.Height;

            // Dinamik hücre boyutu (ekran boyutuna göre)
            int dynamicCellSize = Math.Min(formWidth, formHeight) / (gridSize * 2 + 4);
            CellSize = Math.Max(dynamicCellSize, 40); // Minimum hücre boyutu 40 olarak ayarlandı

            this.Text = "MAVİ ZAFER";
            this.ClientSize = new Size(formWidth, formHeight);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(18, 32, 47);
            this.Font = new Font("Arial", 10, FontStyle.Bold);

            // Bot tahtası konumu (grid oluşturma sırasında kullanılıyor)
            int botBoardStartX = formWidth / 2 + 20;
            int botBoardStartY = formHeight / 2 - (gridSize * CellSize) / 2;

            // Oyuncu tahtası
            int playerBoardStartX = 20;
            int playerBoardStartY = formHeight / 2 - (gridSize * CellSize) / 2;
            CreateGrid(playerBoard, new Point(playerBoardStartX, playerBoardStartY), "Kahraman Bölgesi");

            // Bot tahtası
            CreateGrid(botBoard, new Point(botBoardStartX, botBoardStartY), "Korsan Bölgesi", isBot: true);

            // "Oyunu Bitir" butonu, bot tahtasının hemen altına yerleştiriliyor
            Button btnEndGame = new Button
            {
                Text = "Teslim Ol",
                Size = new Size(120, 40),
                Location = new Point(botBoardStartX + (gridSize * CellSize) / 2 - 60, botBoardStartY + gridSize * CellSize + 20), // Bot tahtasının altına ortalanmış
                BackColor = Color.FromArgb(255, 69, 0), // Kırmızımsı bir renk
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnEndGame.FlatAppearance.BorderSize = 0;
            btnEndGame.Click += BtnEndGame_Click;
            this.Controls.Add(btnEndGame);


            // Arka plan görselini ayarlıyoruz
            string backgroundImagePath = @"C:\Users\MEHMET POLAT\Downloads\bs.png";
            if (File.Exists(backgroundImagePath))
            {
                this.BackgroundImage = Image.FromFile(backgroundImagePath);
                this.BackgroundImageLayout = ImageLayout.Stretch; // Görseli ekran boyutuna göre sığdırıyoruz
            }
            else
            {
                // Eğer görsel bulunmazsa, arka plan rengini belirliyoruz
                this.BackColor = Color.FromArgb(18, 32, 47);
            }

            this.Font = new Font("Arial", 10, FontStyle.Bold);

            // Bilgilendirme Label'ı
            Label lblInfo = new Label
            {
                Text = "Kırmızı isabet, Mavi ıska.",
                AutoSize = true,
                ForeColor = Color.Red,
                Location = new Point(20, Math.Min(formHeight - 60, playerBoardStartY + gridSize * CellSize + 20)),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            this.Controls.Add(lblInfo);

            // Durum Listesi
            lstStatus = new ListBox
            {
                Location = new Point(formWidth - 240, 20),
                Width = 200,
                Height = formHeight - 40,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(28, 40, 51),
                Font = new Font("Arial", 12, FontStyle.Bold),
                DrawMode = DrawMode.OwnerDrawFixed, // Satırları kendimiz çizmemizi sağlar
                ItemHeight = 40 // Satır yüksekliğini artırıyoruz
            };
            this.Controls.Add(lstStatus);

            // Öğeleri ekrandan taşmasını engellemek için kaydırma çubuğu ekleme (isteğe bağlı)
            if (playerBoardStartY + gridSize * CellSize + 60 > formHeight)
            {
                AddScrollBars();
            }

            // DrawItem olayını tanımlayarak her satırın rengini değiştirme
            lstStatus.DrawItem += (sender, e) =>
            {
                if (e.Index >= 0)
                {
                    // Alternatif olarak mavi ve kırmızı renkler arasında geçiş yapıyoruz
                    if (e.Index % 2 == 0)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 255)), e.Bounds); // Mavi
                    }
                    else
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 0)), e.Bounds); // Kırmızı
                    }

                    // Yazıyı beyaz renkte yazdırıyoruz
                    e.Graphics.DrawString(lstStatus.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds);
                }
            };
        }

        private void AddScrollBars()
        {
            // Scrollable Panel
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = this.BackColor
            };

            // Tüm öğeleri panele taşı
            foreach (Control control in this.Controls)
            {
                control.Parent = panel;
            }

            // Paneli forma ekle
            this.Controls.Add(panel);
        }




        private void CreateGrid(Button[,] board, Point startLocation, string title, bool isBot = false)
        {
            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(startLocation.X, startLocation.Y - 30),
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(lblTitle);

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Button cellButton = new Button
                    {
                        Size = new Size(CellSize, CellSize),
                        Location = new Point(startLocation.X + j * CellSize, startLocation.Y + i * CellSize),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(28, 40, 51),
                        ForeColor = Color.White,
                        Tag = new Point(i, j),
                        FlatAppearance = { BorderSize = 1, BorderColor = Color.FromArgb(70, 120, 140) }
                    };

                    if (isBot)
                    {
                        cellButton.Click += BotBoardCell_Click;
                    }

                    board[i, j] = cellButton;
                    this.Controls.Add(cellButton);
                }
            }
        }

        private void BotBoardCell_Click(object sender, EventArgs e)
        {
            Button cellButton = sender as Button;
            if (cellButton != null && cellButton.Tag is Point coord)
            {
                if (cellButton.Text == "🔥" || cellButton.Text == "💧")
                {
                    lstStatus.Items.Add("Kullanıcı: Bu hücreye zaten saldırdınız!");
                    return;
                }

                int shipId = botState[coord.X, coord.Y];

                if (shipId > 0)
                {
                    cellButton.Text = "🔥";
                    cellButton.BackColor = Color.Red;
                    lstStatus.Items.Add($"Kullanıcı: [{coord.X}, {coord.Y}] isabet!");
                    playerHits++;

                    botShipsHealth[shipId]--;
                    if (botShipsHealth[shipId] == 0)
                    {
                        lstStatus.Items.Add("Kullanıcı: Bir gemi battı!");
                        ShowDestroyedShip(shipId);
                    }

                    if (IsGameOver())
                    {
                        return;
                    }
                }
                else
                {
                    cellButton.Text = "💧";
                    cellButton.BackColor = Color.Blue;
                    lstStatus.Items.Add($"Kullanıcı: [{coord.X}, {coord.Y}] ıska!");
                    BotAttack();
                }
            }
        }

        private void BotAttack()
        {
            while (true)
            {
                int x = random.Next(gridSize);
                int y = random.Next(gridSize);

                if (playerBoard[x, y].Text == "🔥" || playerBoard[x, y].Text == "💧")
                {
                    continue;
                }

                if (playerState[x, y])
                {
                    playerBoard[x, y].Text = "🔥";
                    playerBoard[x, y].BackColor = Color.Red;
                    lstStatus.Items.Add($"Bot: [{x}, {y}] isabet!");
                    botHits++;

                    if (IsGameOver())
                    {
                        return;
                    }
                }
                else
                {
                    playerBoard[x, y].Text = "💧";
                    playerBoard[x, y].BackColor = Color.Blue;
                    lstStatus.Items.Add($"Bot: [{x}, {y}] ıska!");
                    break;
                }
            }
        }

        private bool IsGameOver()
        {
            bool playerLost = botShipsHealth.Values.All(v => v == 0);
            bool botLost = playerBoard.Cast<Button>().Count(b => b.BackColor == Color.Red) >= 10;

            // Tahta boyutuna göre zorluk seviyesini belirleme
            int boardSize = playerBoard.GetLength(0); // Tahta boyutunu doğru şekilde alıyoruz
            double difficultyMultiplier = GetDifficultyMultiplier(boardSize); // Zorluk çarpanını alıyoruz

            if (playerLost)
            {
                // Skor hesaplama: Zorluk çarpanı uygulanarak doğru hesaplama yapılıyor
                int totalScore = (int)((playerHits * 50 + 300) * difficultyMultiplier);
                SaveScore(playerHits, totalScore, "Kazandı"); // Skor kaydediliyor
                MessageBox.Show($"Tebrikler KAPTAN! Deniz'i Kurtardınız. Toplam Skor: {totalScore}", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NavigateToSelectionForm();
                return true;
            }
            else if (botLost)
            {
                // Skor hesaplama: Zorluk çarpanı uygulanarak doğru hesaplama yapılıyor
                int totalScore = (int)((playerHits * 30) * difficultyMultiplier);
                SaveScore(playerHits, totalScore, "Kaybetti"); // Skor kaydediliyor
                MessageBox.Show($"Maalesef kaybettiniz. Toplam Skor: {totalScore}", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NavigateToSelectionForm();
                return true;
            }

            return false;
        }

        // Zorluk seviyesini tahta boyutuna göre belirleme fonksiyonu
        private double GetDifficultyMultiplier(int boardSize)
        {
            if (boardSize == 7) // Kolay (7x7)
                return 0.5;
            else if (boardSize == 10) // Orta (10x10)
                return 1.0;
            else if (boardSize == 15) // Zor (15x15)
                return 2.0;
            else
                return 1.0; // Varsayılan (Eğer başka bir boyut varsa 1.0 kabul ediyoruz)
        }


        private void NavigateToSelectionForm()
        {
            // Close the current game form
            this.Close();

            // Open the frmSecim form (assuming it's already implemented)
            frmSecim selectionForm = new frmSecim(userId, username);
            selectionForm.Show();
        }

        private void ShowDestroyedShip(int shipId)
        {
            // Gemi görsellerini belirleyin
            string horizontalShipImage = @"C:\\Users\\MEHMET POLAT\\Downloads\\bspb.png";
            string verticalShipImage = @"C:\\Users\\MEHMET POLAT\\Downloads\\bspbd.png";

            // Geminin yatay mı dikey mi olduğunu tespit edin
            bool isVertical = IsShipVertical(shipId);
            string shipDestroyedImage = isVertical ? verticalShipImage : horizontalShipImage;

            using (Bitmap fullImage = new Bitmap(shipDestroyedImage))
            {
                // Geminin uzunluğunu belirleyin
                int shipLength = GetShipLength(shipId);

                // Hücre boyutlarını gemi uzunluğuna göre ayarlayın
                int cellWidth = isVertical ? fullImage.Width : fullImage.Width / shipLength; // Bir hücre genişliği
                int cellHeight = isVertical ? fullImage.Height / shipLength : fullImage.Height; // Bir hücre yüksekliği


                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        // Eğer bu hücrede batmış geminin parçası varsa
                        if (botState[i, j] == shipId)
                        {
                            // Gemi parçasının sıra numarasını hesaplayın
                            int partIndex = GetShipPartIndex(shipId, i, j, isVertical);

                            // Hücre görseli oluşturun
                            Bitmap cellImage = isVertical ? new Bitmap(cellHeight, cellWidth) : new Bitmap(cellWidth, cellHeight);

                            using (Graphics g = Graphics.FromImage(cellImage))
                            {
                                if (isVertical)
                                {
                                    // Dikey kesme alanı
                                    int cropY = partIndex * cellHeight;
                                    g.DrawImage(fullImage, new Rectangle(0, 0, cellHeight, cellWidth), 0, cropY, cellWidth, cellHeight, GraphicsUnit.Pixel);
                                }
                                else
                                {
                                    // Yatay kesme alanı
                                    int cropX = partIndex * cellWidth;
                                    g.DrawImage(fullImage, new Rectangle(0, 0, cellWidth, cellHeight), cropX, 0, cellWidth, cellHeight, GraphicsUnit.Pixel);
                                }
                            }

                            // Hücreye görseli yerleştirin
                            botBoard[i, j].BackgroundImage = cellImage;
                            botBoard[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                            botBoard[i, j].FlatStyle = FlatStyle.Flat;
                            botBoard[i, j].FlatAppearance.BorderSize = 0;
                        }
                    }
                }
            }
        }

        // Geminin uzunluğunu belirleyen yardımcı metot
        private int GetShipLength(int shipId)
        {
            int count = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (botState[i, j] == shipId)
                    {
                        count++;
                    }
                }
            }
            return count; // Gemiye ait hücre sayısı
        }

        // Geminin yatay mı dikey mi yerleştirildiğini kontrol eden yardımcı metot
        private bool IsShipVertical(int shipId)
        {
            for (int i = 0; i < gridSize - 1; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (botState[i, j] == shipId && botState[i + 1, j] == shipId)
                    {
                        return true; // Gemi dikey olarak yerleştirilmiş
                    }
                }
            }
            return false; // Gemi yatay olarak yerleştirilmiş
        }

        // Geminin parça sırasını hesaplayan yardımcı metot (hem yatay hem dikey için)
        private int GetShipPartIndex(int shipId, int row, int col, bool isVertical)
        {
            int index = 0;
            if (isVertical)
            {
                for (int i = 0; i < gridSize; i++)
                {
                    if (botState[i, col] == shipId)
                    {
                        if (i == row) return index;
                        index++;
                    }
                }
            }
            else
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (botState[row, j] == shipId)
                    {
                        if (j == col) return index;
                        index++;
                    }
                }
            }
            return index;
        }



        private void BtnEndGame_Click(object sender, EventArgs e)
        {
            // Tahta boyutunu hesapla
            int boardSize = playerBoard.GetLength(0); // Oyuncu tahtasının boyutunu al

            // Zorluk çarpanını belirle
            double difficultyMultiplier = GetDifficultyMultiplier(boardSize);

            // Skor hesapla
            int totalScore = (int)(playerHits * 30 * difficultyMultiplier); // Zorluk çarpanını uygula

            // Skoru kaydet
            SaveScore(playerHits, totalScore, "Kaybetti");

            // Kullanıcıya bilgi ver
            MessageBox.Show($"Maalesef oyunu bitirdiniz. Toplam Skor: {totalScore}", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // frmSecim formuna yönlendir
            NavigateToSelectionForm();
        }



        private void GenerateBotBoard()
        {
            PlaceBotShip(5, 1);
            PlaceBotShip(4, 2);
            PlaceBotShip(3, 3);
            PlaceBotShip(2, 4);
        }

        private void PlaceBotShip(int size, int shipId)
        {
            bool placed = false;
            while (!placed)
            {
                int x = random.Next(gridSize);
                int y = random.Next(gridSize);
                bool horizontal = random.Next(2) == 0;

                if (CanPlaceShip(botState, x, y, size, horizontal))
                {
                    for (int i = 0; i < size; i++)
                    {
                        int newX = horizontal ? x : x + i;
                        int newY = horizontal ? y + i : y;

                        botState[newX, newY] = shipId;
                    }
                    botShipsHealth[shipId] = size;
                    placed = true;
                }
            }
        }

        private bool CanPlaceShip(int[,] state, int x, int y, int size, bool horizontal)
        {
            for (int i = 0; i < size; i++)
            {
                int newX = horizontal ? x : x + i;
                int newY = horizontal ? y + i : y;

                if (newX >= gridSize || newY >= gridSize || state[newX, newY] != 0)
                {
                    return false;
                }
            }
            return true;
        }


        private void SaveScore(int totalHits, int totalScore, string gameType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Scores (UserID, Username, TotalHits, TotalScore, GameType, DatePlayed)
                        VALUES (@UserID, @Username, @TotalHits, @TotalScore, @GameType, @DatePlayed)";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@TotalHits", totalHits);
                        cmd.Parameters.AddWithValue("@TotalScore", totalScore);
                        cmd.Parameters.AddWithValue("@GameType", gameType);
                        cmd.Parameters.AddWithValue("@DatePlayed", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                lstStatus.Items.Add($"Skor kaydedildi: {totalScore}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Skor Kaydedilemedi");
            }
        }
    }
}