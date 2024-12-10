# React Rich Text Editor Integration with Backend for DOCX and HTML Conversion

This project demonstrates how to integrate a React Rich Text Editor (RTE) with a backend server for converting base64-encoded DOCX documents to HTML strings and vice versa using DocIO. The backend retrieves base64-encoded DOCX strings from the server, converts them into HTML strings, and sends the HTML string to the frontend. When the "Save" button is clicked, the frontend sends the modified HTML string back to the backend, which then converts it back into a DOCX base64 string.

## Prerequisites

Before running the project, ensure you have the following installed on your system:

- **Node.js** (Latest LTS version)
- **Visual Studio Code** (or any code editor of your choice)
- **ASP.NET Core** (for the backend server)

## How to Run the Application

1. **Clone the Repository**

   Clone the project to your local machine:

   ```bash
   git clone Get-Content-From-RTE-Update-Content-To-Server


2. **Run the Backend Server**

    Ensure the backend server is running. If you're using ASP.NET Core, start the server either from Visual Studio or by using the following command in the terminal:
  
   ```bash
    dotnet run

3. **Install Dependencies (Frontend)**

    Navigate to the project directory and install the necessary dependencies for the React frontend:

   ```bash

    cd rich-text-editor

    npm install


4. **Run the React Application**

    In the project directory, run the React app:

   ```bash
    npm start


