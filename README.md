# Library Management System
Task: Library Management System
Interview Home Assignment for Redstone Alliance Sdn Bhd

## Prerequisites

- Visual Studio with .NET Core installed
- MySQL Server installed locally or accessible remotely
- Packages: MySqlConnector, MySql.Data

## Setup Instructions

** Database Setup **
- Execute the database setup scripts provided in the folder 'LibaryManagementDB.sql'.

** Configuration **
- get all the code from folder LibraryManagement/LibraryManagement/Program.cs
- write down your own server configuration on line 33.

## Usage

Input operation option.
- User need to input number from 1-8 to run a specific operation.
- User need to input number 8 to exit.
  
Add a new book to the library.
- User need to input title and author of book.
- New book is added.
  
Update details of existing books.
- User need to input book id.
- User need to update a new title and author of book.
- Book have been updated.
  
Delete books from the library.
- User need to input book id.
- Book have been deleted.
  
List all available books.
- All list of books will display.
  
Check out books and set due dates.
- User need to input book id.
- User need to input due date of the book.
- Book is loan out.

Return books and mark them as available again.
- User need to input loan id.
- Book is available.
  
List all active loans.
- All list of loans will display.

## Application Design

** Project Structure **
- LibraryManagement: Manages the database connection.
- BookManagement: Handles all book-related operations.
- LoanManagement: Manages loan-related functionalities.

** Classes **
- DBcon: Manages the connection to the MySQL database.
- BookManager: Adds, updates, deletes, and lists books.
- LoanManager: Handles book checkouts and returns.
- Program: Serves as the main entry point.

Faizal Sapuri
faizal20004@gmail.com





