using System;

namespace FaceitStats.Core.Utils
{
    class EloCalculator
    {
        public static int CalculateElo(int diff, double winProbability)
        {
            double percentage = 1 / (1 + Math.Pow(10, (double)diff / 400));
            int winPoints = (int)(50 * (1 - percentage));
            return winPoints;
        }
    }
}
