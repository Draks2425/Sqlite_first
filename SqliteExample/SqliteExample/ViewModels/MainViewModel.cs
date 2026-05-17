using SqliteExample.Models;
using SqliteExample.Services;
using SqliteExample.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SqliteExample.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const int PageSize = 10;
        private readonly DatabaseService _database;
        private Person? _selectedPC;
        private bool _isEditing;
        private int _pageNumber = 1;
        private string? _currentGPU;
        private string? _currentCPU;
        private int _currentRAM;
        private string? _currentColor;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Person> PcList { get; } = new();
        public List<int> RAMOptions { get; } = new() { 16, 32, 64 };
        public List<string> ColorOptions { get; } = new() { "Czarny", "Biały", "Szary", "Niebieski" };

        public string? CurrentGPU
        {
            get => _currentGPU;
            set
            {
                if (_currentGPU == value) return;
                _currentGPU = value;
                OnPropertyChanged();
            }
        }

        public string? CurrentCPU
        {
            get => _currentCPU;
            set
            {
                if (_currentCPU == value) return;
                _currentCPU = value;
                OnPropertyChanged();
            }
        }

        public int CurrentRAM
        {
            get => _currentRAM;
            set
            {
                if (_currentRAM == value) return;
                _currentRAM = value;
                OnPropertyChanged();
            }
        }

        public string? CurrentColor
        {
            get => _currentColor;
            set
            {
                if (_currentColor == value) return;
                _currentColor = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing == value) return;
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public string ButtonText => IsEditing ? "Zapisz" : "Dodaj";

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (_pageNumber == value) return;
                _pageNumber = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddOrSaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public MainViewModel(DatabaseService database)
        {
            _database = database;
            CurrentColor = "Czarny";
            CurrentRAM = 16;

            AddOrSaveCommand = new Command(AddOrSave);
            CancelCommand = new Command(CancelEdit);
            DeleteCommand = new Command<Person>(DeletePerson);
            OpenDetailCommand = new Command<Person>(async person => await OpenDetailAsync(person));
            NextPageCommand = new Command(NextPage);
            PreviousPageCommand = new Command(PreviousPage);

            EnsureSeedData();
            LoadPage();
        }

        public void LoadPage()
        {
            var pcs = _database.GetPeopleAsync().Result ?? new List<Person>();
            var maxPage = Math.Max(1, (int)Math.Ceiling(pcs.Count / (double)PageSize));

            if (PageNumber > maxPage)
            {
                PageNumber = maxPage;
            }

            var pageItems = pcs
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            PcList.Clear();
            foreach (var pc in pageItems)
            {
                PcList.Add(pc);
            }
        }

        private void AddOrSave()
        {
            if (string.IsNullOrWhiteSpace(CurrentGPU) || string.IsNullOrWhiteSpace(CurrentCPU)) return;

            if (IsEditing && _selectedPC != null)
            {
                _selectedPC.GPU = CurrentGPU;
                _selectedPC.CPU = CurrentCPU;
                _selectedPC.RAM = CurrentRAM;
                _selectedPC.CaseColor = CurrentColor;
                _database.UpdatePersonAsync(_selectedPC).Wait();
                IsEditing = false;
                _selectedPC = null;
            }
            else
            {
                var pc = new Person { GPU = CurrentGPU, CPU = CurrentCPU, RAM = CurrentRAM, CaseColor = CurrentColor };
                _database.AddPersonAsync(pc).Wait();
            }

            CurrentGPU = string.Empty;
            CurrentCPU = string.Empty;
            LoadPage();
        }

        private void CancelEdit()
        {
            IsEditing = false;
            CurrentGPU = string.Empty;
            CurrentCPU = string.Empty;
            CurrentRAM = 16;
            CurrentColor = "Czarny";
        }

        private void DeletePerson(Person? pc)
        {
            if (pc == null) return;
            _database.DeletePersonAsync(pc).Wait();
            LoadPage();
        }

        private async Task OpenDetailAsync(Person? pc)
        {
            if (pc == null) return;
            await Shell.Current.GoToAsync(nameof(PcDetailPage), true, new Dictionary<string, object>
            {
                ["pc"] = pc
            });
        }

        private void NextPage()
        {
            var pcs = _database.GetPeopleAsync().Result ?? new List<Person>();
            var maxPage = Math.Max(1, (int)Math.Ceiling(pcs.Count / (double)PageSize));
            if (PageNumber >= maxPage) return;
            PageNumber++;
            LoadPage();
        }

        private void PreviousPage()
        {
            if (PageNumber <= 1) return;
            PageNumber--;
            LoadPage();
        }

        private void EnsureSeedData()
        {
            var pcs = _database.GetPeopleAsync().Result;
            if (pcs?.Count > 0) return;

            var defaultPcs = new List<Person>
            {
                new Person { GPU = "RTX 4060", CPU = "i5-13600K", RAM = 16, CaseColor = "Czarny" },
                new Person { GPU = "RTX 4070", CPU = "i7-13700K", RAM = 32, CaseColor = "Biały" },
                new Person { GPU = "RTX 4090", CPU = "i9-14900K", RAM = 64, CaseColor = "Niebieski" }
            };

            foreach (var pc in defaultPcs)
            {
                _database.AddPersonAsync(pc).Wait();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
