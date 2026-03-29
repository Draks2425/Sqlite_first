using SQLite;

namespace SqliteExample.Models
{
    public class Person
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
