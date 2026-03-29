using SQLite;
using SqliteExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteExample.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory,
            "mydatabase.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Person>().Wait();
        }
        // SELECT
        public Task<List<Person>> GetPeopleAsync()
        {
            return _database.Table<Person>().ToListAsync();
        }
        // INSERT
        public Task<int> AddPersonAsync(Person person)
        {
            return _database.InsertAsync(person);
        }
        // DELETE
        public Task<int> DeletePersonAsync(Person person)
        {
            return _database.DeleteAsync(person);
        }
        // UPDATE
        public Task<int> UpdatePersonAsync(Person person)
        {
            return _database.UpdateAsync(person);
        }
    }
}
