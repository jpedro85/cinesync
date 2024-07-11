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

### Steps to Setup the Project

1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-username/CineSync.git
   cd cinesync/CineSync```

2.  **Install the dependencies and run the Project**
    ```bash
    dotnet restore
    dotnet build
    dotnet run
    ```
After this you just need to head to the browser and access http://localhost:5145
