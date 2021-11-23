using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DataBase
{
    class DataBase
    {
        private SqliteConnection conn;

        public DataBase()
        {
            conn = new SqliteConnection("Data Source=Library.db");
            conn.Open();
            initDataBase();
        }

        private void initDataBase()
        {
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = conn;

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Genres(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, genre TEXT NOT NULL)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Authors(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, surname TEXT NOT NULL)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Book(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, title TEXT NOT NULL, author INTEGER, genre INTEGER, FOREIGN KEY(author) REFERENCES Authors(id), FOREIGN KEY(genre) REFERENCES Genres(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE VIEW IF NOT EXISTS V AS SELECT * FROM Book b LEFT JOIN Authors a ON b.author = a.id LEFT JOIN Genres g ON b.genre = g.id ORDER BY a.id";
            cmd.ExecuteNonQuery();

        }

        public string GetAll(string search = null)
        {
            string retval = null;

            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = conn;

            if (search != null)
            {
                search = search.Insert(0, "\"%");
                search = search.Insert(search.Length, "%\"");
                cmd.CommandText = $"SELECT * FROM V WHERE title LIKE {search} or name LIKE {search} or surname LIKE {search} ORDER BY id";
            }
            else
                cmd.CommandText = $"SELECT * FROM V ORDER BY id";

            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = reader.GetValue(0);
                        var title = reader.GetValue(1);
                        var name = reader.GetValue(5);
                        var surname = reader.GetValue(6);
                        var genre = reader.GetValue(8);
                        retval += $"{id}\t{title}\t{name} {surname}\t{genre}\n";
                    }
                }
            }
            return retval;
        }

        public string GetAuthors()
        {
            string retval = null;

            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM Authors ORDER BY id";

            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = reader.GetValue(0);
                        var name = reader.GetValue(1);
                        var surname = reader.GetValue(2);
                        retval += $"{id} {name} {surname}\n";
                    }
                }
            }
            return retval;

        }

        public string GetGenres()
        {
            string retval = null;
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM Genres ORDER BY id";
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = reader.GetValue(0);
                        var name = reader.GetValue(1);
                        retval += $"{id} {name}\n";
                    }
                }
            }
            return retval;
        }

        private int addAuthor(string author)
        {
            string[] aname = author.Split(" ", 2);
            if (aname.Length == 2)
            {
                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"INSERT INTO Authors VALUES (NULL, \"{aname[0]}\", \"{aname[1]}\")";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT MAX(id) FROM Authors";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            return id;
                        }
                    }
                }
            }
            return 0;
        }

        private int addGenre(string genre)
        {
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"INSERT INTO Genres VALUES (NULL, \"{genre}\")";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT MAX(id) FROM Genres";
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = id = reader.GetInt32(0);
                        return id;
                    }
                }
            }
            return 0;
        }

        public bool AddBookInteractive()
        {
            Console.Write("Enter Title: ");
            string title = Console.ReadLine();

            if (title.Length > 0)
            {
                int author_id = 0;
                int genre_id = 0;

                Console.WriteLine(this.GetAuthors());
                Console.Write("Select author or enter name for creating new one: ");
                string author = Console.ReadLine();
                if (author.Length > 0)
                {
                    if (int.TryParse(author, out author_id) == false)
                    {
                        author_id = addAuthor(author);
                    }
                }

                Console.WriteLine(this.GetGenres());
                Console.Write("Select genre or enter for creating new one: ");
                string genre = Console.ReadLine();
                if (genre.Length > 0)
                {
                    if (int.TryParse(genre, out genre_id) == false)
                    {
                        genre_id = addGenre(genre);
                    }
                }


                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"INSERT INTO Book VALUES (NULL, \"{title}\", \"{ author_id}\", \"{ genre_id}\")";
                cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        public bool RemoveBook(string ids)
        {
            int id = 0;
            int.TryParse(ids, out id);
            if (id != 0)
            {
                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"DELETE FROM Book WHERE id =\"{id}\"";
                if (cmd.ExecuteNonQuery() != 0)
                    return true;
            }
            return false;
        }

        ~DataBase()
        {
            conn.Close();
        }

    }
}