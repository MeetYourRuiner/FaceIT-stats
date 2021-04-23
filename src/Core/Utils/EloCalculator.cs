using System;

namespace FaceitStats.Core.Utils
{
    public static class EloCalculator
    {
        //public static int CalculateElo(int diff)
        //{
        //    double percentage = 1 / (1 + Math.Pow(10, (double)diff / 400));
        //    int winPoints = (int)(50 * (1 - percentage));
        //    return winPoints;
        //}

        public static (int gain, int loss) CalculateEloChange(double winProbability)
        {
            int gain = Convert.ToInt32(Math.Round(50 - winProbability * 50));
            return (
                gain,
                -(50 - gain)
            );
        }
    }
}
