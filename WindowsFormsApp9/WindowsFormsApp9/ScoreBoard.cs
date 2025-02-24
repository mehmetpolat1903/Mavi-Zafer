using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

public class ScoreBoard
{
    public List<PlayerScore> Scores { get; private set; } = new List<PlayerScore>();

    public void LoadScoresFromDatabase(int userId, string connectionString)
    {
        Scores.Clear();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "SELECT Username, TotalScore, GameDate FROM Scores WHERE UserID = @UserID ORDER BY GameDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string playerName = reader.GetString(0);
                        int score = reader.GetInt32(1);
                        DateTime gameDate = reader.GetDateTime(2);

                        Scores.Add(new PlayerScore(playerName, score, gameDate));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanından skorlar alınırken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public void SortScores()
    {
        Scores.Sort((x, y) => y.Score.CompareTo(x.Score));
    }

    internal void ShowScores()
    {
        throw new NotImplementedException();
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
