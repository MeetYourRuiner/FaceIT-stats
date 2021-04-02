using FaceitStats.Core.Models;
using FaceitStats.WPF.ViewModels.Abstractions;
using System.Collections.Generic;

namespace FaceitStats.WPF.ViewModels.Controls
{
    class MatchesViewModel : BaseViewModel
    {
        private List<Match> _matches;
        public List<Match> Matches
        {
            get { return _matches; }
            set
            {
                _matches = value;
                OnPropertyChanged();
            }
        }

        private int _matchesOnPage;
        public int MatchesOnPage
        {
            get { return _matchesOnPage; }
            set
            {
                _matchesOnPage = value;
                OnPropertyChanged();
            }
        }

        public MatchesViewModel(List<Match> matches, int matchesOnPage)
        {
            Matches = matches;
            MatchesOnPage = matchesOnPage;
        }
    }
}
