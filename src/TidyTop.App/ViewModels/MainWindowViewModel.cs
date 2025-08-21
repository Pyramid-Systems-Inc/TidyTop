using ReactiveUI;

namespace TidyTop.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _title = "TidyTop Desktop Organizer";

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
    }
}