CREATE DATABASE librarymanagement;
USE librarymanagement;
CREATE TABLE Books (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255),
    Author VARCHAR(255),
    IsLoanedOut BOOLEAN
);
CREATE TABLE Loans (
    LoanId INT AUTO_INCREMENT PRIMARY KEY,
    BookId INT,
    LoanDate DATE,
    DueDate DATE,
    Returned BOOLEAN,
    FOREIGN KEY (BookId) REFERENCES Books(Id)
);
SELECT * FROM books;
SELECT * FROM loans;


















