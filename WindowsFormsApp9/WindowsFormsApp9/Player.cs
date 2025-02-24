public class Player
{
    public string UserName { get; set; }  // UserName özelliği eklendi
    public string Password { get; set; }  // Şifre özelliği
    public int Score { get; set; }        // Puan özelliği eklendi

    // Player sınıfının constructor'ı
    public Player(string userName, string password)
    {
        UserName = userName;
        Password = password;
        Score = 0; // Başlangıçta puan 0
    }

    // Puanı güncelleyen metod
    public void UpdateScore(int score)
    {
        Score += score; // Yeni skor ekleniyor
    }

    // Puanı sıfırlayan metod (örneğin oyun bittiğinde)
    public void ResetScore()
    {
        Score = 0;
    }

    // Kullanıcı adı ve şifreyi kontrol eden metod (login işlemi için)
    public bool ValidateLogin(string username, string password)
    {
        return UserName.Equals(username) && Password.Equals(password);
    }
}
