using SqliteExample.Models;
using SqliteExample.Services;

namespace SqliteExample.Views
{
    [QueryProperty(nameof(Pc), "pc")]
    public partial class PcDetailPage : ContentPage
    {
        private readonly DatabaseService _database;
        private Person? _pc;

        public Person? Pc
        {
            get => _pc;
            set
            {
                if (_pc == value) return;
                _pc = value;
                OnPropertyChanged();
            }
        }

        public PcDetailPage(DatabaseService database)
        {
            InitializeComponent();
            _database = database;
            BindingContext = this;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (Pc == null) return;
            await _database.UpdatePersonAsync(Pc);
            await Shell.Current.GoToAsync("..");
        }
    }
}
