using System;

namespace faceitwpf.Classes
{
    class Utils
    {
        public static int CalculateElo(int diff)
        {
            double percentage = 1 / (1 + Math.Pow(10, (double)diff / 400));
            int winPoints = (int)(50 * (1 - percentage));
            return winPoints;
        }
    }
}
