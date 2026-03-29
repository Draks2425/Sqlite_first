using SQLite;

namespace SqliteExample.Models
{
    public class Person
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(100)]
        public string? GPU { get; set; }
        [MaxLength(100)]
        public string? CPU { get; set; }
        [MaxLength(50)]
        public int RAM { get; set; }
        [MaxLength(50)]
        public string? CaseColor { get; set; }
    }
}
