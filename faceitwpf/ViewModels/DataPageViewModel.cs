using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace faceitwpf.ViewModels
{
    class DataPageViewModel : INotifyPropertyChanged
    {
        public DataPageViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
