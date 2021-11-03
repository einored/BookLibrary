using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BookLibrary
{
    public class BookList
    {
        public List<Book> Books { get; set; }
    }

    public class Book
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public DateTime PubDate { get; set; }
        public string ISBN { get; set; }
        public Owner Owner { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime TimeTaken { get; set; }
        public int BookHoldingDuration { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            string jsonFile = @"C:\Users\LENOVO\source\repos\BookLibrary\BookLibrary.json";
            List<Book> books = new List<Book>();
            books = readJsonFile(jsonFile, books);

            //books = addBook(jsonFile, books, "Harry Potter", "J. K. Rowling", "Fantasy", "EN", DateTime.Parse("1869-01-01"), "958-5-26-178414-5");

            //books = takeBook(jsonFile, books, "958-5-26-178214-5", "Ein", "Red", 31);

            //books = returnBook(jsonFile, books, "958-5-26-178414-5");

            //fillterByName(books, "Harrty Potter");
            //fillterByAuthor(books, "J. K. Rowling");
            //fillterByCategory(books, "Fantasy");
            //fillterByLanguage(books, "LT");
            //fillterByISBN(books, "958-5-26-178414-5");
            //fillterByAvailability(books, "available");  //available taken

            //deleteBook(jsonFile, books, "958-5-26-448414-5");
        }

        public static List<Book> readJsonFile(string jsonFile, List<Book> books)
        {
            if (File.Exists(jsonFile))
            {
                string json = File.ReadAllText(jsonFile);
                books = JsonSerializer.Deserialize<List<Book>>(json);
                
                return books;
            }
            else
            {
                Console.WriteLine("Empty file");
                return new List<Book>();
            }                
        }

        public static List<Book> addBook(string jsonFile, List<Book> books, string bookName, string bookAuthor, string bookCategory, string bookLanguage, DateTime bookPubDate, string bookISBN)
        {
            var b = new Book
            {
                Name = bookName,
                Author = bookAuthor,
                Category = bookCategory,
                Language = bookLanguage,
                PubDate = bookPubDate,
                ISBN = bookISBN,
                Owner = new Owner()
                {
                    Name = "Visma",
                    Surname = "Visma",
                    TimeTaken = DateTime.Now,
                    BookHoldingDuration = 0
                }
            };

            books.Add(b);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(books, options);

            File.WriteAllText(jsonFile, jsonString);
            Console.WriteLine(jsonString);

            return books;
        }

        public static List<Book> takeBook(string jsonFile, List<Book> books, string isbn, string ownerName, string ownerSurname, int bookHoldingDurationDays)
        {
            bool bookExist = false;
            int booksHoldingCount = 0;

            foreach (var book in books)
            {
                if (book.Owner.Name == ownerName && book.Owner.Surname == ownerSurname)
                    booksHoldingCount++;

                if (book.ISBN == isbn)
                    bookExist = true;
            }
            if (bookExist)
            {
                if (bookHoldingDurationDays > 60)
                {
                    Console.WriteLine("Book holding duration should be less than 60 days");
                }
                if (booksHoldingCount >= 3)
                {
                    Console.WriteLine("Already holding maximum number of book (max 3 books)");
                }
                if (bookHoldingDurationDays <= 60 && booksHoldingCount < 3)
                {
                    books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.Name = ownerName);
                    books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.Surname = ownerSurname);
                    books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.TimeTaken = DateTime.Now);
                    books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.BookHoldingDuration = bookHoldingDurationDays);

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString = JsonSerializer.Serialize(books, options);

                    File.WriteAllText(jsonFile, jsonString);
                }
            }
            else
                Console.WriteLine("Bad ISBN, book not found");

            return books;
        }

        public static List<Book> returnBook(string jsonFile, List<Book> books, string isbn)
        {
            DateTime dateTime = DateTime.UtcNow.Date;

            int index = books.FindIndex(a => a.ISBN.Equals(isbn));
            int duration = (int)Math.Abs((dateTime - books[index].Owner.TimeTaken).TotalDays);
            Console.WriteLine(duration);
            if (duration > books[index].Owner.BookHoldingDuration)
            {
                Console.WriteLine("It's better to be late, than to arrive ugly :)");

                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.Name = "Visma");
                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.Surname = "Visma");
                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.TimeTaken = DateTime.Now);
                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.BookHoldingDuration = 0);
            }
            else
            {
                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.Name = "Visma");
                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.Surname = "Visma");
                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.TimeTaken = DateTime.Now);
                books.Where(w => w.ISBN == isbn).ToList().ForEach(s => s.Owner.BookHoldingDuration = 0);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(books, options);

            File.WriteAllText(jsonFile, jsonString);

            return books;
        }

        public static void fillterByName(List<Book> books, string keyWord)
        {
            var filtered = new List<Book>();

            foreach (var book in books)
            {
                if (book.Name == keyWord)
                {
                    filtered.Add(book);
                }
            }

            Console.WriteLine(string.Join(',', filtered));
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(filtered, options);
            Console.WriteLine(jsonString);
        }

        public static void fillterByAuthor(List<Book> books, string keyWord)
        {
            var filtered = new List<Book>();

            foreach (var book in books)
            {
                if (book.Author == keyWord)
                {
                    filtered.Add(book);
                }
            }

            Console.WriteLine(string.Join(',', filtered));
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(filtered, options);
            Console.WriteLine(jsonString);
        }

        public static void fillterByCategory(List<Book> books, string keyWord)
        {
            var filtered = new List<Book>();

            foreach (var book in books)
            {
                if (book.Category == keyWord)
                {
                    filtered.Add(book);
                }
            }

            Console.WriteLine(string.Join(',', filtered));
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(filtered, options);
            Console.WriteLine(jsonString);
        }

        public static void fillterByLanguage(List<Book> books, string keyWord)
        {
            var filtered = new List<Book>();

            foreach (var book in books)
            {
                if (book.Language == keyWord)
                {
                    filtered.Add(book);
                }
            }

            Console.WriteLine(string.Join(',', filtered));
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(filtered, options);
            Console.WriteLine(jsonString);
        }

        public static void fillterByISBN(List<Book> books, string keyWord)
        {
            var filtered = new List<Book>();

            foreach (var book in books)
            {
                if (book.ISBN == keyWord)
                {
                    filtered.Add(book);
                }
            }

            Console.WriteLine(string.Join(',', filtered));
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(filtered, options);
            Console.WriteLine(jsonString);
        }

        public static void fillterByAvailability(List<Book> books, string keyWord)
        {
            var filtered = new List<Book>();

            if (keyWord == "taken")
            {
                foreach (var book in books)
                {
                    if (book.Owner.Name != "Visma")
                    {
                        filtered.Add(book);
                    }
                }

                Console.WriteLine(string.Join(',', filtered));
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(filtered, options);
                Console.WriteLine(jsonString);
            }
            else if (keyWord == "available")
            {
                foreach (var book in books)
                {
                    if (book.Owner.Name == "Visma")
                    {
                        filtered.Add(book);
                    }
                }
                Console.WriteLine(string.Join(',', filtered));
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(filtered, options);
                Console.WriteLine(jsonString);
            }
            else
                Console.WriteLine("Bad keyword");
        }

        public static List<Book> deleteBook(string jsonFile, List<Book> books, string isbn)
        {
            var b1 = new Book
            {
                Name = "Jonas",
                Author = "Jonaitis",
                Category = "Istorinis",
                Language = "LT",
                PubDate = DateTime.Parse("1869-01-01"),
                ISBN = "6-178-63",
                Owner = new Owner()
                {
                    Name = "Visma",
                    Surname = "",
                    TimeTaken = DateTime.Now,
                    BookHoldingDuration = 0
                }
            };

            int index = books.FindIndex(a => a.ISBN.Equals(isbn));
            if (index > 0)
            {
                books.RemoveAt(index);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(books, options);

                File.WriteAllText(jsonFile, jsonString);
            }
            return books;
        }
    }
}
