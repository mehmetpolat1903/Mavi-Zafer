CREATE TABLE Scores (
    ID INT IDENTITY(1,1) PRIMARY KEY,   -- Otomatik artan bir ID
    Username NVARCHAR(100) NOT NULL,      -- Oyuncunun kullanıcı adı
    Score INT NOT NULL,                   -- Oyuncunun kazandığı puan
    GameDate DATETIME NOT NULL DEFAULT GETDATE() -- Oyunun oynandığı tarih
);
