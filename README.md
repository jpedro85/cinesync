# CineSync

## Description
CineSync is a social network dedicated to cinema lovers, providing a platform where users can share their interests, opinions, and knowledge about movies. This project was developed as part of the Software Engineering course at the Faculty of Exact Sciences and Engineering (FCEE), under the guidance of professors Leonel Nóbrega, Carlos Dória, and Nuno Santos.

## Project Overview
CineSync was developed to unite cinephiles and enthusiasts of the seventh art, allowing users to record and manage the movies they have seen or wish to see. Additionally, users can rate, comment on, and share opinions about movies, interacting with other community members. The platform also integrates external web services to obtain detailed information about movies.

## Key System Requirements
- **Account Registration and Management**: Allows users to create and manage their accounts, including profile editing and account deletion.
- **Movie Management**: Allows users to add movies to watched and wishlist lists, rate and comment on movies, and search for movies by various criteria.
- **Social Interaction**: Enables users to follow others and interact with them through comments and discussions.
- **Integration with External Web Services**: Integrates detailed information about movies from external services.
- **Privacy and Settings Management**: Allows users to configure their profile privacy settings.

## Non-Functional Requirements
- **Response Time**: The application should respond quickly to user requests.
- **Scalability**: The system should handle an increase in the number of users and data.
- **Data Protection**: User data must be protected against unauthorized access.
- **Attack Protection**: Security measures should be implemented to prevent possible attacks.

## Authors
- Ricardo Vieira
- Carlos Coelho
- João Pedro Abreu
- Pedro Ferreira

## License
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

## Setup Procedure
This project is developed in C# and uses ASP.NET and Blazor.

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or higher)
- Bearer Token for TMDB API
- SMTP (if you want the email service)

### Steps to Setup the Project

1. **Clone the Repository**
   ```bash
   git clone https://github.com/jpedro85/CineSync.git
   cd cinesync/CineSync```

2. **Make the .env**
   For the project to work properly you need a TMDB Bearer Token for the API to be able to fetch movies information thats not already in the Database.
   If you have a STMP like Google you can use it else you will need to go to applicationFacade and make `false` `options.SignIn.RequireConfirmedAccount = true` in line 122
   The .env should look like this:
   ```bash
   BEARER_TOKEN=eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJkNTkyMTlhZTQ3MjcxZmE5NDg3YzI3MTJjMzRhMTZkMiIsInN1YiI6IjY2MGFjNzMzMTVkZWEwMDE2MjMyZTQxZiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.qMp8g8AF-OnuKWknE-1-eN5BqqwHlrvyHXgTqvT_wG4
   SMTP_SERVER=smtp.gmail.com
   SMTP_PORT=587
   SMTP_SENDER_NAME=SenderName
   SMTP_SENDER_EMAIL=SendersEmail
   SMTP_USERNAME=SenderEmail //same as the Email
   SMTP_PASSWORD=YourSecurePassword // recommended to use a passkey for an application
   ```

4.  **Install the dependencies and run the Project**
   If you have the .env setup properly you just need to install the dependencies, build and run the Server
    ```bash
    dotnet restore
    dotnet build
    dotnet run
    ```
After this you just need to head to the browser and access http://localhost:5145 or use the address shown in the log 
