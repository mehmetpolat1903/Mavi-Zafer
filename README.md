Projenin Genel Yapısı ve Kodların İşlevleri
Mavi Zafer, C# Windows Forms ve .NET Framework kullanılarak geliştirilmiş bir Battle Ship tarzı deniz savaşı oyunu.

1. Giriş ve Kullanıcı İşlemleri
frmLogin ve frmRegister: Kullanıcı giriş ve kayıt ekranları. Veritabanına bağlanarak kullanıcıları doğrular.
frmSignIn: Kullanıcı giriş yaptıktan sonra ana seçim ekranına yönlendirir (frmSecim).
2. Oyun Seçenekleri ve Hazırlık
frmSecim: Kullanıcı, "Oyun Oyna" veya "Skorlar" butonlarını seçer.
frmDifficultySelect: Oyun zorluk seviyesi belirlenir (Kolay, Orta, Zor).
frmGameBoard: Kullanıcı gemilerini yerleştirir.
3. Oyun Mekanizması
frmBattle: Asıl oyun ekranı. Oyuncu ve bot, sırayla atış yapar.
GameManager: Oyun durumunu yönetir, sıralı atışları kontrol eder.
Player ve BotPlayer: Oyuncu ve botun davranışlarını tanımlar.
Ship: Gemi sınıfı, her geminin koordinatlarını ve durumunu saklar.
4. Skor Yönetimi
ScoreBoard: Kullanıcının kazandığı puanları gösterir.
Veritabanı (Users ve Scores tabloları): Oyuncuların bilgilerini ve skorlarını saklar.

AYRICA resimler dosya yolu ile eklenmiştir. Düzeltmek için dosya yolunu değiştirmeniz yeterlidir.
