using SqliteExample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteExample.ViewModels
{
    class MainViewModel
    {
        private readonly DatabaseService _database;
        public MainViewModel(DatabaseService database)
        {
            _database = database;
        }
        public async Task LoadData()
        {
            var people = await _database.GetPeopleAsync();
        }
    }
}
