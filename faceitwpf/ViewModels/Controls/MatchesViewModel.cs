using faceitwpf.Models;
using faceitwpf.ViewModels.Abstractions;
using System.Collections.Generic;

namespace faceitwpf.ViewModels.Controls
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

        public MatchesViewModel(List<Match> matches)
        {
            Matches = matches;
        }
    }
}
