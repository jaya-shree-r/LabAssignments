namespace Day4
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
    //Day3-Changed both Regular and Premium member classes 
    public class RegularMember : LibraryMember
    {
        public const int MaxBooks = 3; 
        public RegularMember(string name, string memberId) : base(name, memberId) { }
    }

    public class PremiumMember : LibraryMember
    {
        public const int MaxBooks = 10; 
        public PremiumMember(string name, string memberId) : base(name, memberId) { }
    }


    public class Library
    {
        //Day4-Task => Events and Delegates
        public delegate void LibraryEventHandler(string message);
        public event LibraryEventHandler BookBorrowed;
        public event LibraryEventHandler BookReturned;
        public event LibraryEventHandler LowStockWarning;

        private const int LowStockThreshold = 2; // Set stock warning threshold

        protected virtual void OnBookBorrowed(string message)
        {
            BookBorrowed?.Invoke(message);
        }

        protected virtual void OnBookReturned(string message)
        {
            BookReturned?.Invoke(message);
        }

        protected virtual void OnLowStockWarning(string message)
        {
            LowStockWarning?.Invoke(message);
        }

        //Day3-Task
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

        //Day2 Task
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
            var book = Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null)
                Console.WriteLine("Book not found. Please enter a valid ISBN.");
            return book;
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
            var member = Members.FirstOrDefault(m => m.MemberId == memberId);
            if (member == null)
                Console.WriteLine("Member not found. Please enter a valid Member ID.");
            return member;
        }

        //Day3-Changed BorrowBook to implement limits 
        //Day4-Check stock is running low and  Trigger event 
        public async Task BorrowBookAsync(string memberId, string isbn)
        {
                await Task.Delay(500);
                var member = SearchMember(memberId);
                var book = SearchBook(isbn);

                if (member == null)
                {
                    Console.WriteLine("Member not found. Please check the Member ID.");
                    return;
                }

                if (book == null)
                {
                    Console.WriteLine("Book not found. Please check the ISBN.");
                    return;
                }

                if (book.IsBorrowed)
                {
                    Console.WriteLine($"Sorry, the book '{book.Title}' is already borrowed.");
                    return;
                }

                int maxBooksAllowed = member is RegularMember ? RegularMember.MaxBooks : PremiumMember.MaxBooks;

                if (member.BorrowedBooks.Count >= maxBooksAllowed)
                {
                    Console.WriteLine($"Limit exceeded! {member.Name} can borrow a maximum of {maxBooksAllowed} books.");
                    return;
                }

                book.IsBorrowed = true;
                member.BorrowedBooks.Add(book);

                Console.WriteLine($"Success! {member.Name} has borrowed '{book.Title}'.");

                // Trigger event for book borrowed
                OnBookBorrowed($"Notification: {member.Name} borrowed '{book.Title}'.");

                // Check if stock is running low
                if (Books.Count(b => !b.IsBorrowed) <= LowStockThreshold)
                {
                    OnLowStockWarning("Warning: Library stock is running low!");
                }
        }

        //Day4-Trigger event for book returned
        public async Task ReturnBookAsync(string memberId, string isbn)
        {
                await Task.Delay(500);
                var member = SearchMember(memberId);
                var book = member?.BorrowedBooks.FirstOrDefault(b => b.ISBN == isbn);

                if (member == null)
                {
                    Console.WriteLine("Invalid Member ID. Please enter a valid member ID.");
                    return;
                }
                if (book == null)
                {
                    Console.WriteLine("Invalid Book ISBN. Please enter a valid ISBN.");
                    return;
                }

                book.IsBorrowed = false;
                member.BorrowedBooks.Remove(book);

                Console.WriteLine($"Success! {member.Name} has returned '{book.Title}'.");

                OnBookReturned($"Notification: {member.Name} returned '{book.Title}'.");
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

        //Linq queries - Day3
         public void LINQQueries()
        {
            Console.WriteLine("\nBooks Sorted by Publication Year (Ascending):");
            var booksAsc = Books.OrderBy(b => b.PublicationYear);
            foreach (var book in booksAsc) Console.WriteLine($"{book.Title} ({book.PublicationYear})");

            Console.WriteLine("\nBooks Sorted by Publication Year (Descending):");
            var booksDesc = Books.OrderByDescending(b => b.PublicationYear);
            foreach (var book in booksDesc) Console.WriteLine($"{book.Title} ({book.PublicationYear})");

            Console.WriteLine("\nBooks Grouped by Author:");
            var booksByAuthor = Books.GroupBy(b => b.Author.Name);
            foreach (var group in booksByAuthor)
                Console.WriteLine($"{group.Key}: {group.Count()} books");

            Console.Write("\nEnter keyword to search for books: ");
            string keyword = Console.ReadLine().ToLower();
            var filteredBooks = Books.Where(b => b.Title.ToLower().Contains(keyword));
            foreach (var book in filteredBooks) Console.WriteLine($"{book.Title}");

            Console.WriteLine("\nLibrary Members Grouped by Type:");
            var membersByType = Members.GroupBy(m => m.GetType().Name);
            foreach (var group in membersByType)
                Console.WriteLine($"{group.Key}: {group.Count()} members");

            Console.WriteLine("\nTotal Books Borrowed per Member:");
            var borrowedCounts = Members.Select(m => new { m.Name, Count = m.BorrowedBooks.Count });
            foreach (var entry in borrowedCounts)
                Console.WriteLine($"{entry.Name} borrowed {entry.Count} books");

            Console.WriteLine("\nBooks with Multiple Authors (if duplicate author names are present):");
            var authorsWithMultipleBooks = Books.GroupBy(b => b.Author.Name).Where(g => g.Count() > 1);
            foreach (var group in authorsWithMultipleBooks)
            {
                Console.WriteLine($"{group.Key} has written multiple books:");
                foreach (var book in group)
                    Console.WriteLine($"  - {book.Title}");
            }
        }
        
    }
    
    //Custom Exception - Day 3
    public class BookNotFoundException : Exception
    {
        public BookNotFoundException(string message) : base(message) { }
    }

    public class MemberLimitExceededException : Exception
    {
        public MemberLimitExceededException(string message) : base(message) { }
    }
    class Program4
    {
        public static async Task Main()
        {
            Library library = new Library();
            Console.WriteLine("Library Management System");

            library.BookBorrowed += message => Console.WriteLine(message);
            library.BookReturned += message => Console.WriteLine(message);
            library.LowStockWarning += message => Console.WriteLine(message);


            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Display Books");
                Console.WriteLine("2. Display Members");
                Console.WriteLine("3. Add a Book");
                Console.WriteLine("4. Delete a Book");
                Console.WriteLine("5. Borrow a Book");
                Console.WriteLine("6. Return a Book");
                Console.WriteLine("7. Search for a Book");
                Console.WriteLine("8. Search for a Member");
                Console.WriteLine("9. Add a Member");
                Console.WriteLine("10. Remove a Member");
                Console.WriteLine("11. Update Book Title");
                Console.WriteLine("12. Run LINQ Queries");
                Console.WriteLine("13. Exit");

                Console.Write("Enter your choice: ");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Available Books: ");
                            library.DisplayBooks();
                            break;

                        case 2:
                            Console.WriteLine("Available members: ");
                            library.DisplayMembers();
                            break;

                        case 3:
                            Console.WriteLine("To add a new book.. "); 
                            Console.Write("Enter book title: ");
                            string? title = Console.ReadLine();
                            Console.Write("Enter author name: ");
                            string? authorName = Console.ReadLine();
                            Console.Write("Enter author email: ");
                            string? authorEmail = Console.ReadLine();
                            Console.Write("Enter ISBN: ");
                            string? isbn = Console.ReadLine();
                            Console.Write("Enter publication year: ");
                            if (!int.TryParse(Console.ReadLine(), out int publicationYear))
                            {
                                Console.WriteLine("Invalid year. Please enter a number.");
                                break;
                            }
                            library.AddBook(new Book(title, new Author(authorName, authorEmail), isbn, publicationYear));
                            Console.WriteLine("Book added successfully.");
                            break;

                        case 4:
                            Console.Write("Enter ISBN of the book to delete: ");
                            string? isbnToDelete = Console.ReadLine();
                            if (library.SearchBook(isbnToDelete) == null)
                            {
                                Console.WriteLine("Book not found.");
                                break;
                            }
                            library.DeleteBook(isbnToDelete);
                            Console.WriteLine("Book deleted successfully.");
                            break;

                        case 5:
                            Console.Write("Enter member ID: ");
                            string? memberId = Console.ReadLine();
                            Console.Write("Enter book ISBN to borrow: ");
                            string? borrowIsbn = Console.ReadLine();
                            if (library.SearchMember(memberId) == null)
                            {
                                Console.WriteLine("Member not found.");
                                break;
                            }
                            if (library.SearchBook(borrowIsbn) == null)
                            {
                                Console.WriteLine("Book not found.");
                                break;
                            }
                            await library.BorrowBookAsync(memberId, borrowIsbn);
                            Console.WriteLine("Book borrowed successfully.");
                            break;

                        case 6:
                            Console.Write("Enter member ID: ");
                            string? returnMemberId = Console.ReadLine();
                            Console.Write("Enter book ISBN to return: ");
                            string returnIsbn = Console.ReadLine();
                            var member = library.SearchMember(returnMemberId);
                            var book = member?.BorrowedBooks.FirstOrDefault(b => b.ISBN == returnIsbn);
                            if (member == null)
                            {
                                Console.WriteLine("Member not found.");
                                break;
                            }
                            if (book == null)
                            {
                                Console.WriteLine("Book not found or not borrowed by this member.");
                                break;
                            }
                            await library.ReturnBookAsync(returnMemberId, returnIsbn);
                            Console.WriteLine("Book returned successfully.");
                            break;

                        case 7:
                            Console.Write("Enter ISBN to search for a book: ");
                            string? searchIsbn = Console.ReadLine();
                            var foundBook = library.SearchBook(searchIsbn);
                            if (foundBook == null)
                            {
                                Console.WriteLine("Book not found.");
                            }
                            else
                            {
                                Console.WriteLine($"Found Book: {foundBook.Title}, Author: {foundBook.Author.Name}");
                            }
                            break;

                        case 8:
                            Console.Write("Enter Member ID to search: ");
                            string? searchMemberId = Console.ReadLine();
                            var foundMember = library.SearchMember(searchMemberId);
                            if (foundMember == null)
                            {
                                Console.WriteLine("Member not found.");
                            }
                            else
                            {
                                Console.WriteLine($"Found Member: {foundMember.Name}, Borrowed Books: {foundMember.BorrowedBooks.Count}");
                            }
                            break;

                        case 9:
                            Console.Write("Enter member name: ");
                            string? memberName = Console.ReadLine();
                            Console.Write("Enter member ID: ");
                            string? newMemberId = Console.ReadLine();
                            Console.Write("Enter membership type (Regular/Premium): ");
                            string memberType = Console.ReadLine().ToLower();

                            if (memberType == "regular")
                                library.AddMember(new RegularMember(memberName, newMemberId));
                            else if (memberType == "premium")
                                library.AddMember(new PremiumMember(memberName, newMemberId));
                            else
                                Console.WriteLine("Invalid membership type.");
                            break;

                        case 10:
                            Console.Write("Enter Member ID to remove: ");
                            string? removeMemberId = Console.ReadLine();
                            if (library.SearchMember(removeMemberId) == null)
                            {
                                Console.WriteLine("Member not found.");
                                break;
                            }
                            library.RemoveMember(removeMemberId);
                            Console.WriteLine("Member removed successfully.");
                            break;

                        case 11:
                            Console.Write("Enter ISBN of book to update: ");
                            string? updateIsbn = Console.ReadLine();
                            Console.Write("Enter new title: ");
                            string? newTitle = Console.ReadLine();
                            if (library.SearchBook(updateIsbn) == null)
                            {
                                Console.WriteLine("Book not found.");
                                break;
                            }
                            library.UpdateBook(updateIsbn, newTitle);
                            Console.WriteLine("Book title updated successfully.");
                            break;

                        case 12:
                            library.LINQQueries();
                            break;

                        case 13:
                            exit = true;
                            Console.WriteLine("Exiting Library Management System.");
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 13.");
                            break;
                    }
                }
                catch (BookNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (MemberLimitExceededException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected Error: {ex.Message}");
                }
            }
        }
    }
}
