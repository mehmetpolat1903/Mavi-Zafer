using System;
using System.Drawing;

namespace MaviZafer
{
    public class Ship
    {
        public int Size { get; private set; }
        public Point[] Coordinates { get; private set; } // Gemi koordinatları
        public int Hits { get; private set; } // Alınan hasar

        public bool IsSunk => Hits >= Size; // Gemi tamamen batmış mı?

        public event Action<Ship> Sunk; // Gemi batma olayı

        public Ship(int size)
        {
            Size = size;
            Coordinates = new Point[size];
            Hits = 0;
        }

        /// <summary>
        /// Gemi koordinatlarını manuel olarak ayarlama.
        /// </summary>
        /// <param name="coordinates">Gemiye atanacak koordinatlar</param>
        public void SetCoordinates(Point[] coordinates)
        {
            if (coordinates.Length != Size)
                throw new ArgumentException("Koordinat sayısı gemi boyutuyla eşleşmeli.");
            Coordinates = coordinates;
        }

        /// <summary>
        /// Gemi başlangıçta oyun tahtasının ortasına yerleştirilir.
        /// </summary>
        /// <param name="gridSize">Oyun tahtası boyutu</param>
        /// <param name="isHorizontal">Geminin yatay mı yoksa dikey mi yerleştirileceği</param>
        public void SetCoordinatesToCenter(int gridSize, bool isHorizontal)
        {
            int startX = (gridSize - Size) / 2; // X ekseninde başlangıç noktası
            int startY = gridSize / 2;          // Y ekseninde orta nokta

            Coordinates = new Point[Size];

            for (int i = 0; i < Size; i++)
            {
                Coordinates[i] = isHorizontal
                    ? new Point(startX + i, startY)  // Yatay yerleştirme
                    : new Point(startX, startY + i); // Dikey yerleştirme
            }
        }

        /// <summary>
        /// Geminin hedeflenen noktada vurulup vurulmadığını kontrol eder.
        /// </summary>
        /// <param name="target">Hedeflenen koordinat</param>
        /// <returns>Vurulma durumu (true/false)</returns>
        public bool CheckHit(Point target)
        {
            foreach (var coord in Coordinates)
            {
                if (coord == target)
                {
                    Hit();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Geminin aldığı hasarı arttırır ve batma durumunu kontrol eder.
        /// </summary>
        public void Hit()
        {
            Hits++;
            if (IsSunk)
                Sunk?.Invoke(this); // Gemi batma olayını tetikler
        }
    }
}
