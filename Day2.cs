namespace Day2
{
    public class Author
    {
        public string Name { get; }
        public string Email { get; }

        public Author(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }

    public class Book
        {
        public string? Title { get; set; } 
        public Author Author { get; }
        public string? ISBN { get; }
        public int PublicationYear { get; }
        public bool IsBorrowed { get; set; }
        public Book(string title, Author author, string isbn, int publicationYear)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            PublicationYear = publicationYear;
            IsBorrowed = false;
        }
    }

    public class LibraryMember
    {
        public string Name { get; }
        public string MemberId { get; }
        public List<Book> BorrowedBooks { get; }

        public LibraryMember(string name, string memberId)
        {
            Name = name;
            MemberId = memberId;
            BorrowedBooks = new List<Book>();
        }
    }

    public class RegularMember : LibraryMember
    {
        public RegularMember(string name, string memberId) : base(name, memberId) { }
    }

    public class PremiumMember : LibraryMember
    {
        public PremiumMember(string name, string memberId) : base(name, memberId) { }
    }

    public class Library
    {
        public List<Book> Books { get; }
        public List<LibraryMember> Members { get; }

        public Library()
        {
            Books = new List<Book>
            {
                new Book("book1", new Author("author1", "author1@email.com"), "1111", 2001),
                new Book("book2", new Author("author2", "author2@email.com"), "2222", 2005)
            };
            Members = new List<LibraryMember>
            {
                new RegularMember("RMember1", "RM01"),
                new PremiumMember("PMember2", "PM02")
            };
        }
        public void AddBook(Book book)
        {
            if (Books.Any(b => b.ISBN == book.ISBN))
            {
                Console.WriteLine("A book with this ISBN already exists.");
                return;
            }
            Books.Add(book);
        }

        public void UpdateBook(string isbn, string newTitle)
        {
            var book = Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book != null)
            {
                book.Title = newTitle;
            }
        }

        public void DeleteBook(string isbn)
        {
            Books.RemoveAll(b => b.ISBN == isbn);
        }
        public Book SearchBook(string isbn)
        {
            return Books.FirstOrDefault(b => b.ISBN == isbn);
        }
        public void AddMember(LibraryMember member)
        {
            Members.Add(member);
        }
        public void RemoveMember(string memberId)
        {
            Members.RemoveAll(m => m.MemberId == memberId);
        }
        public LibraryMember SearchMember(string memberId)
        {
            return Members.FirstOrDefault(m => m.MemberId == memberId);
        }

        public async Task BorrowBookAsync(string memberId, string isbn)
        {
            await Task.Delay(500);
            var member = SearchMember(memberId);
            var book = SearchBook(isbn);

            if (member != null && book != null && !book.IsBorrowed)
            {
                book.IsBorrowed = true;
                member.BorrowedBooks.Add(book);
            }
        }
        public async Task ReturnBookAsync(string memberId, string isbn)
        {
            await Task.Delay(500);
            var member = SearchMember(memberId);
            var book = member?.BorrowedBooks.FirstOrDefault(b => b.ISBN == isbn);

            if (member != null && book != null)
            {
                book.IsBorrowed = false;
                member.BorrowedBooks.Remove(book);
            }
        }
        public void DisplayBooks()
        {
            foreach (var book in Books)
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author.Name}, ISBN: {book.ISBN}, Year: {book.PublicationYear}, Borrowed: {book.IsBorrowed}");
            }
        }
        public void DisplayMembers()
        {
            foreach (var member in Members)
            {
                Console.WriteLine($"Member: {member.Name}, ID: {member.MemberId}, Borrowed Books: {member.BorrowedBooks.Count}");
            }
        }  
    }
    class Program2
    {
        public static async Task Main()
        {
            Library library = new Library();
            Console.WriteLine("Library Management System");
            Console.WriteLine("Available Books:");
            library.DisplayBooks();
            Console.WriteLine("Available Members:");
            library.DisplayMembers();

            Console.WriteLine("Add a new book: ");

            Console.Write("book title: ");
            string title = Console.ReadLine();
            Console.Write("author name: ");
            string authorName = Console.ReadLine();
            Console.Write("author email: ");
            string authorEmail = Console.ReadLine();
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine();
            Console.Write("publication year: ");
            if (!int.TryParse(Console.ReadLine(), out int publicationYear))
            {
                Console.WriteLine("Invalid year. Please enter a number.");
                return;
            }
            library.AddBook(new Book(title, new Author(authorName, authorEmail), isbn, publicationYear));
            library.DisplayBooks();

            Console.Write("ISBN to delete: ");
            string isbnToDelete = Console.ReadLine();
            library.DeleteBook(isbnToDelete);
            library.DisplayBooks();

            Console.Write("member ID to borrow book: ");
            string memberId = Console.ReadLine();
            Console.Write("book ISBN to borrow: ");
            string borrowIsbn = Console.ReadLine();
            await library.BorrowBookAsync(memberId, borrowIsbn);
            library.DisplayBooks();

            Console.Write("member ID to return book: ");
            string returnMemberId = Console.ReadLine();
            Console.Write("book ISBN to return: ");
            string returnIsbn = Console.ReadLine();
            await library.ReturnBookAsync(returnMemberId, returnIsbn);
            library.DisplayBooks();

             Console.Write("Enter ISBN to search for a book: ");
            string searchIsbn = Console.ReadLine();
            var foundBook = library.SearchBook(searchIsbn);
            Console.WriteLine(foundBook != null ? $"Found Book: {foundBook.Title}, Author: {foundBook.Author.Name}" : "Book not found");

            Console.Write("Enter Member ID to search: ");
            string searchMemberId = Console.ReadLine();
            var foundMember = library.SearchMember(searchMemberId);
            Console.WriteLine(foundMember != null ? $"Found Member: {foundMember.Name}, Borrowed Books: {foundMember.BorrowedBooks.Count}" : "Member not found");
       
        }
    }
}
