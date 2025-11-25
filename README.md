This is a C# Web API MVC program I've created to demonstrate Authentication, Role-Based Authorization, EF Core, LINQ, ASYNC Programming and more. 
To run this program simply download the contents and run them through an IDE, or simply visit the hosted site here: https://librarywebapp-fzd7d4fwfmdpdjab.canadacentral-01.azurewebsites.net/
You will be given the option to log in, or sign up. If you create an account, base users will be fit into the "User" Role. There exists a seeded Admin account with the "Admin" role for further functionality. 
Users have access to their own user page, and the library from which they can borrow books, or return them from their user page if they want to, or sign out.
Admins have access to a dashboard that allows them to view all books, not just available books within the library currently, and to view who is currently borrowing a book. Admins can also view the top 3 books 
displayed through a chart.
To access the admin account use the details Username: admin Password: Adminpassword123
To access a user account simply create an account, and then login with those details. Keep in mind you cannot repeat usernames, and sessions expire after 60 minutes.
