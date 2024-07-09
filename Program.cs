using System;
using System.Data.Common;
using BookManagement;
using LibraryManagement;
using LoanManagement;
using MySqlConnector;

namespace LibraryManagement
{
    internal class DBcon
    {
        private string _connectionString;

        public DBcon(string server, string userId, string password, string database)
        {
            // Connection string using the parameters
            _connectionString = $"Server={server};user id={userId};password={password};database={database}";
          
        }

        public MySqlConnection GetConnection()
        {
            // Return a new MySqlConnection using the connection string
            return new MySqlConnection(_connectionString);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // changes connection as your sql server
            DBcon con = new DBcon("localhost", "root", "SQLbonjour04@", "LibraryManagement");

            // Create instances of managers
            BookManager bm = new BookManager(con);
            LoanManager lm = new LoanManager(con);

            // User operation of system
            while (true)
            {
                Console.WriteLine("Library Management System:");
                Console.WriteLine("1. Add a new book");
                Console.WriteLine("2. Update a book");
                Console.WriteLine("3. Delete a book");
                Console.WriteLine("4. List all books");
                Console.WriteLine("5. Check out a book");
                Console.WriteLine("6. Return a book");
                Console.WriteLine("7. List all active loans");
                Console.WriteLine("8. Exit");
                Console.Write("Enter your choice (1-8): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook(bm);
                        break;
                    case "2":
                        UpdateBook(bm);
                        break;
                    case "3":
                        DeleteBook(bm);
                        break;
                    case "4":
                        bm.ListBooks();
                        break;
                    case "5":
                        CheckOutBook(lm);
                        break;
                    case "6":
                        ReturnBook(lm);
                        break;
                    case "7":
                        lm.ListActiveLoans();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

        }

        private static void AddBook(BookManager bm)
        {
            Console.Write("Enter book title: ");
            string title = Console.ReadLine();

            Console.Write("Enter book author: ");
            string author = Console.ReadLine();

            bm.AddBook(title, author);
        }

        private static void UpdateBook(BookManager bm)
        {
            Console.Write("Enter the ID of the book to update: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.Write("Enter new title: ");
                string title = Console.ReadLine();

                Console.Write("Enter new author: ");
                string author = Console.ReadLine();

                bm.UpdateBook(bookId, title, author);
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a numeric value.");
            }
        }

        private static void DeleteBook(BookManager bm)
        {
            Console.Write("Enter the ID of the book to delete: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                bm.DeleteBook(bookId);
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a numeric value.");
            }
        }

        private static void CheckOutBook(LoanManager lm)
        {
            Console.Write("Enter the ID of the book to check out: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.Write("Enter the due date (yyyy-mm-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
                {
                    lm.CheckOutBook(bookId, dueDate);
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please enter the date as yyyy-mm-dd.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a numeric value.");
            }
        }

        private static void ReturnBook(LoanManager lm)
        {
            Console.Write("Enter the Loan ID to return: ");
            if (int.TryParse(Console.ReadLine(), out int loanId))
            {
                lm.ReturnBook(loanId);
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a numeric value.");
            }
        }

    }
}

namespace BookManagement
{
    internal class BookManager
    {
        private DBcon _dbConnector;

        public BookManager(DBcon dbConnector)
        {
            _dbConnector = dbConnector;
        }

        // Add a new book
        public void AddBook(string title, string author)
        {
            using (var con = _dbConnector.GetConnection())
            {
                con.Open();
                string query = "INSERT INTO Books (Title, Author, IsLoanedOut) VALUES (@Title, @Author, 0)";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Author", author);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Book added successfully.");
            }
        }

        // Update book details
        public void UpdateBook(int bookId, string title, string author)
        {
            using (var con = _dbConnector.GetConnection())
            {
                con.Open();
                string query = "UPDATE Books SET Title = @Title, Author = @Author WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Author", author);
                    cmd.Parameters.AddWithValue("@Id", bookId);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Book updated successfully.");
            }
        }

        // Delete a book
        public void DeleteBook(int bookId)
        {
            using (var con = _dbConnector.GetConnection())
            {
                con.Open();
                string query = "DELETE FROM Books WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", bookId);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Book deleted successfully.");
            }
        }

        // List all books
        public void ListBooks()
        {
            using (var con = _dbConnector.GetConnection())
            {
                con.Open();
                string query = "SELECT Id, Title, Author, IsLoanedOut FROM Books";
                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Books in the library:");
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            string author = reader.GetString(2);
                            bool isLoanedOut = reader.GetBoolean(3);

                            string status = isLoanedOut ? "Loaned Out" : "Available";
                            Console.WriteLine($"ID: {id}, Title: {title}, Author: {author}, Status: {status}");
                        }
                    }
                }
            }
        }
    }
}

namespace LoanManagement
{
    internal class LoanManager
    {
        private DBcon _dbConnector;

        public LoanManager(DBcon dbConnector)
        {
            _dbConnector = dbConnector;
        }

        // Loan a book
        public void CheckOutBook(int bookId, DateTime dueDate)
        {
            using (var con = _dbConnector.GetConnection())
            {
                con.Open();

                // Check if the book is already loaned out
                string checkQuery = "SELECT IsLoanedOut FROM Books WHERE Id = @BookId";
                using (var checkCmd = new MySqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@BookId", bookId);
                    var isLoanedOut = (bool?)checkCmd.ExecuteScalar();

                    if (isLoanedOut == true)
                    {
                        Console.WriteLine("This book is already loaned out.");
                        return;
                    }
                }

                // Create a new loan record
                string insertQuery = "INSERT INTO Loans (BookId, LoanDate, DueDate, Returned) VALUES (@BookId, @LoanDate, @DueDate, 0)";
                using (var insertCmd = new MySqlCommand(insertQuery, con))
                {
                    insertCmd.Parameters.AddWithValue("@BookId", bookId);
                    insertCmd.Parameters.AddWithValue("@LoanDate", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@DueDate", dueDate);
                    insertCmd.ExecuteNonQuery();
                }

                // Mark the book as loaned out
                string updateQuery = "UPDATE Books SET IsLoanedOut = 1 WHERE Id = @BookId";
                using (var updateCmd = new MySqlCommand(updateQuery, con))
                {
                    updateCmd.Parameters.AddWithValue("@BookId", bookId);
                    updateCmd.ExecuteNonQuery();
                }

                Console.WriteLine("Book checked out successfully.");
            }
        }

        // Return a book 
        public void ReturnBook(int loanId)
        {
            using (var con = _dbConnector.GetConnection())
            {
                con.Open();

                // Get the book ID for specific loan information
                string selectQuery = "SELECT BookId FROM Loans WHERE LoanId = @LoanId AND Returned = 0";
                using (var selectCmd = new MySqlCommand(selectQuery, con))
                {
                    selectCmd.Parameters.AddWithValue("@LoanId", loanId);
                    var bookId = (int?)selectCmd.ExecuteScalar();

                    if (bookId == null)
                    {
                        Console.WriteLine("Invalid loan ID or the book is already returned.");
                        return;
                    }

                    // Mark the loan as returned
                    string updateLoanQuery = "UPDATE Loans SET Returned = 1 WHERE LoanId = @LoanId";
                    using (var updateLoanCmd = new MySqlCommand(updateLoanQuery, con))
                    {
                        updateLoanCmd.Parameters.AddWithValue("@LoanId", loanId);
                        updateLoanCmd.ExecuteNonQuery();
                    }

                    // Mark the book as available
                    string updateBookQuery = "UPDATE Books SET IsLoanedOut = 0 WHERE Id = @BookId";
                    using (var updateBookCmd = new MySqlCommand(updateBookQuery, con))
                    {
                        updateBookCmd.Parameters.AddWithValue("@BookId", bookId);
                        updateBookCmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Book returned successfully.");
                }
            }
        }

        // List all active loans
        public void ListActiveLoans()
        {
            using (var con = _dbConnector.GetConnection())
            {
                con.Open();
                string query = @"
                    SELECT l.LoanId, b.Title, b.Author, l.LoanDate, l.DueDate 
                    FROM Loans l
                    JOIN Books b ON l.BookId = b.Id
                    WHERE l.Returned = 0";

                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Active Loans:");
                        while (reader.Read())
                        {
                            int loanId = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            string author = reader.GetString(2);
                            DateTime loanDate = reader.GetDateTime(3);
                            DateTime dueDate = reader.GetDateTime(4);

                            Console.WriteLine($"Loan ID: {loanId}, Title: {title}, Author: {author}, Loan Date: {loanDate.ToShortDateString()}, Due Date: {dueDate.ToShortDateString()}");
                        }
                    }
                }
            }
        }
    }
}
