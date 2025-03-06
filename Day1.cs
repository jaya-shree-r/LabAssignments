namespace Day1
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
        public string Title { get; }
        public Author Author { get; }
        public string ISBN { get; }
        public int PublicationYear { get; }

        public Book(string title, Author author, string isbn, int publicationYear)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            PublicationYear = publicationYear;
        }
    }
    public class LibraryMember
    {
        public string Name { get; }
        public int MemberId { get; }
        public List<Book> BorrowedBooks { get; }

        public LibraryMember(string name, int memberId)
        {
            Name = name;
            MemberId = memberId;
            BorrowedBooks = new List<Book>();
        }
    }
    public class RegularMember : LibraryMember
    {
        public RegularMember(string name, int memberId) : base(name, memberId) { }
    }

    public class PremiumMember : LibraryMember
    {
        public PremiumMember(string name, int memberId) : base(name, memberId) { }
    }
    public class Library
    {
        public List<Book> Books { get; }
        public List<LibraryMember> Members { get; }

        public Library()
        {
            Books = new List<Book>();
            Members = new List<LibraryMember>();
        }
    }
}
