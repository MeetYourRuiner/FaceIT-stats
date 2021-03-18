using FaceitStats.WPF.Properties;
using System.Collections.Specialized;

namespace FaceitStats.WPF.Classes
{
    public static class FavoritesWrapper
    {
        private static Settings Settings { get => Settings.Default; }

        public static void Add(string playerName)
        {
            StringCollection favorites = Settings.Favorites;
            favorites.Add(playerName);
            Settings.Default.Favorites = favorites;
            Save();
        }

        public static bool Contains(string playerName)
        {
            return Settings.Favorites.Contains(playerName);
        }

        public static string[] ListAll()
        {
            StringCollection favoritesCollection = Settings.Favorites;
            string[] favorites = new string[favoritesCollection.Count];
            favoritesCollection.CopyTo(favorites, 0);
            return favorites;
        }

        public static void Remove(string playerName)
        {
            StringCollection favorites = Settings.Favorites;
            favorites.Remove(playerName);
            Settings.Default.Favorites = favorites;
            Save();
        }

        public static void Save()
        {
            Settings.Default.Save();
        }
    }
}
