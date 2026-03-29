using SqliteExample.Models;
using SqliteExample.Services;
using System.Collections.ObjectModel;

namespace SqliteExample
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _dbService;
        private Person? _selectedPC;
        private bool _isEditing;

        public ObservableCollection<Person> PeopleList { get; set; } = new();
        public string? CurrentGPU { get; set; }
        public string? CurrentCPU { get; set; }
        public int CurrentRAM { get; set; }
        public string? CurrentColor { get; set; }
        public List<int> RAMOptions { get; set; } = new() { 16, 32, 64 };
        public List<string> ColorOptions { get; set; } = new() { "Czarny", "Biały", "Szary", "Niebieski" };
        
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(nameof(IsEditing)); OnPropertyChanged(nameof(ButtonText)); }
        }

        public string ButtonText => IsEditing ? "Zapisz" : "Dodaj";

        public MainPage(DatabaseService dbService)
        {
            _dbService = dbService;
            var people = _dbService.GetPeopleAsync().Result;

            if (people?.Count == 0)
            {
                var defaultPeople = new List<Person>
                {
                    new Person { GPU = "RTX 4060", CPU = "i5-13600K", RAM = 16, CaseColor = "Czarny" },
                    new Person { GPU = "RTX 4070", CPU = "i7-13700K", RAM = 32, CaseColor = "Biały" },
                    new Person { GPU = "RTX 4090", CPU = "i9-14900K", RAM = 64, CaseColor = "Niebieski" }
                };

                foreach (var p in defaultPeople)
                    _dbService.AddPersonAsync(p).Wait();
                
                people = defaultPeople;
            }

            PeopleList = new ObservableCollection<Person>(people ?? new());
            CurrentColor = "Czarny";
            CurrentRAM = 16;
            
            InitializeComponent();
            BindingContext = this;
        }

        private void OnAddPC_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentGPU) || string.IsNullOrWhiteSpace(CurrentCPU)) return;

            if (IsEditing && _selectedPC != null)
            {
                _selectedPC.GPU = CurrentGPU;
                _selectedPC.CPU = CurrentCPU;
                _selectedPC.RAM = CurrentRAM;
                _selectedPC.CaseColor = CurrentColor;
                _dbService.UpdatePersonAsync(_selectedPC).Wait();
                PeopleList[PeopleList.IndexOf(_selectedPC)] = _selectedPC;
                IsEditing = false;
                _selectedPC = null;
            }
            else
            {
                var pc = new Person { GPU = CurrentGPU, CPU = CurrentCPU, RAM = CurrentRAM, CaseColor = CurrentColor };
                _dbService.AddPersonAsync(pc).Wait();
                PeopleList.Add(pc);
            }

            CurrentGPU = "";
            CurrentCPU = "";
            OnPropertyChanged(nameof(CurrentGPU));
            OnPropertyChanged(nameof(CurrentCPU));
        }

        private void OnEdit_Clicked(object sender, EventArgs e)
        {
            var pc = (sender as Button)?.BindingContext as Person;
            if (pc == null) return;

            _selectedPC = pc;
            CurrentGPU = pc.GPU;
            CurrentCPU = pc.CPU;
            CurrentRAM = pc.RAM;
            CurrentColor = pc.CaseColor;
            IsEditing = true;
            OnPropertyChanged(nameof(CurrentGPU));
            OnPropertyChanged(nameof(CurrentCPU));
        }

        private void OnCancel_Clicked(object sender, EventArgs e)
        {
            IsEditing = false;
            CurrentGPU = "";
            CurrentCPU = "";
            CurrentRAM = 16;
            CurrentColor = "Czarny";
            OnPropertyChanged(nameof(CurrentGPU));
            OnPropertyChanged(nameof(CurrentCPU));
        }

        private void OnDelete_Clicked(object sender, EventArgs e)
        {
            var pc = (sender as Button)?.BindingContext as Person;
            if (pc == null) return;
            _dbService.DeletePersonAsync(pc).Wait();
            PeopleList.Remove(pc);
        }
    }
}
