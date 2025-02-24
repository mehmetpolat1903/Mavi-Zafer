using System;
using System.Data.SqlClient;
using System.Windows.Forms;

public class GameManager
{
    private readonly Player currentPlayer;
    private readonly ScoreBoard scoreBoard;
    private readonly string connectionString;

    public GameManager(Player player, string dbConnectionString)
    {
        this.currentPlayer = player;
        this.connectionString = dbConnectionString;
        this.scoreBoard = new ScoreBoard();
    }

    // Oyunun bitişini kontrol et ve sonucu kaydet
    public void EndGame(int playerScore)
    {
        SaveScoreToDatabase(playerScore);

        // Skorları sıralayıp göster
        scoreBoard.SortScores();
        scoreBoard.ShowScores();
    }

    private void SaveScoreToDatabase(int score)
    {
        string query = "INSERT INTO Scores (UserName, TotalScore, GameDate) VALUES (@UserName, @Score, @Date)";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserName", currentPlayer.UserName);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Skor kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public int CalculatePlayerScore(int hits)
    {
        return hits * 10;
    }
}
