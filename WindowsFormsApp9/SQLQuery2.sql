CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,  -- Otomatik artan birincil anahtar
    Username NVARCHAR(50) NOT NULL,        -- Kullanıcı adı
    Password NVARCHAR(50) NOT NULL         -- Şifre
);
