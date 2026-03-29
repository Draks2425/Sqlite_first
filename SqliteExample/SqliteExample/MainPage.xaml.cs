using SqliteExample.Models;
using SqliteExample.Services;
using System.Collections.ObjectModel;

namespace SqliteExample
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _dbService;

        public ObservableCollection<Person> PeopleList { get; set; }

        public MainPage(DatabaseService dbService)
        {
            _dbService = dbService;
            LoadPeople();
            InitializeComponent();
            BindingContext = this;
        }

        private void LoadPeople()
        {
            var people = _dbService.GetPeopleAsync().Result;

            if (people == null || people.Count() == 0)
            {
                people = new List<Person>
                {
                    new Person { Name = "Alice", Age = 30 },
                    new Person { Name = "Bob", Age = 25 },
                    new Person { Name = "Charlie", Age = 35 }
                };

                foreach (var person in people)
                {
                    _dbService.AddPersonAsync(person).Wait();
                }
            }

            PeopleList = new ObservableCollection<Person>(people);
        }
    }
}
