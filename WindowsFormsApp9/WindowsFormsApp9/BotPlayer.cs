using System;
using System.Collections.Generic;
using System.Drawing;

public class BotPlayer
{
    private List<Point> shipPositions; // Botun gemi pozisyonları
    private List<Point> previousMoves; // Botun yaptığı hamleler
    private Random random;

    public BotPlayer()
    {
        shipPositions = new List<Point>();
        previousMoves = new List<Point>();
        random = new Random();
    }

    public void SetDifficultyLevel(string difficulty)
    {
        // Zorluk seviyesine göre farklı stratejiler belirlenebilir (şimdilik boş)
    }

    public void PlaceShipsRandomly(int gridSize)
    {
        int shipCount = 5; // Örnek olarak 5 gemi yerleştiriliyor

        while (shipPositions.Count < shipCount)
        {
            int x = random.Next(0, gridSize);
            int y = random.Next(0, gridSize);

            Point position = new Point(x, y);
            if (!shipPositions.Contains(position)) // Aynı pozisyona gemi yerleştirme
            {
                shipPositions.Add(position);
            }
        }
    }

    public List<Point> GetShipPositions()
    {
        return new List<Point>(shipPositions); // Gemi pozisyonlarını döndür
    }

    public Point MakeMove()
    {
        Point move;
        do
        {
            move = new Point(random.Next(0, 10), random.Next(0, 10));
        } while (previousMoves.Contains(move));

        previousMoves.Add(move);
        return move;
    }

    public void RegisterHit(Point hitPosition)
    {
        // Gemi vurulmuşsa, burada bir işlem yapılabilir (örneğin, strateji geliştirme)
    }

    public void RegisterSink()
    {
        // Gemi batırıldığında işlem yapılabilir
    }
}
