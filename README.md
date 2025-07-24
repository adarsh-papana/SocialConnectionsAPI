# ***Social Connections API***
This project implements a backend API for managing user profiles and their mutual connections (friendships) in a social network. It provides functionalities for creating users, establishing and removing connections, finding direct friends, identifying friends-of-friends, and calculating the degree of separation between any two users.

## Features
The API provides the following core functionalities:

### User Management:

POST /api/users: Create new user profiles with unique string IDs and display names.

### Connection Management:

POST /api/connections: Establish mutual connections between two users, handling cases of non-existent users or existing connections.

DELETE /api/connections: Remove existing connections between two specified users.

### Social Graph Queries:

GET /api/users/{user_str_id}/friends: Retrieve a list of direct friends (Degree 1 connections) for a given user.

GET /api/users/{user_str_id}/friends-of-friends: Find unique users who are friends of a target user's direct friends, excluding the target user and their direct friends (Degree 2 connections).

GET /api/connections/degree?from_user_str_id={id1}&to_user_str_id={id2}: Calculate the shortest path (degree of separation) between two users using the Breadth-First Search (BFS) algorithm.

## Technologies Used
### Backend Framework: ASP.NET Core Web API (C#)

### Database: SQL Server (configurable for MySQL)

### ORM: Entity Framework Core

### Dependency Management: NuGet

### API Documentation & Testing: Swagger

### Version Control: Git

## Getting Started
Follow these steps to set up and run the project locally.

Prerequisites
.NET SDK (6.0 or later, as per typical ASP.NET Core project setup)

SQL Server (or MySQL) instance

A code editor (e.g., Visual Studio Code, Visual Studio)

1. Clone the Repository
   - git clone https://github.com/AdarshSRM85/SocialConnectionsApi.git
   - cd SocialConnectionsApi

2. Configure Database Connection
   - Open appsettings.json and update the DefaultConnection string to point to your SQL Server (or MySQL) instance.    
    {    
      "ConnectionStrings": {    
        "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=SocialConnectionsDb;Trusted_Connection=True;MultipleActiveResultSets=true"    
        // For MySQL, uncomment and modify:    
        // "DefaultConnection": "Server=localhost;Port=3306;Database=SocialConnectionsDb;Uid=root;Pwd=your_password;"    
      }    
    }    

3. Apply Database Migrations
    - Navigate to the project root directory in your terminal and run the following commands to create the database schema:    
      add-migration {class_name}    
      update-database    

4. Run the Application    
    - dotnet run    

The application will start, typically listening on https://localhost:7000 (or a similar port).    

5. Test with Swagger UI
    - Once the application is running, open your web browser and navigate to:    
      - https://localhost:7000/swagger    

(Replace 7000 with your actual port if different).

The Swagger UI will display all available API endpoints, allowing you to interact with them directly.

API Endpoints Examples (using cURL)    
Replace https://localhost:7000 with your actual API base URL.    

## Create User    
curl -X POST "https://localhost:7000/api/Users" \    
-H "Content-Type: application/json" \    
-d '{    
  "userStrId": "alice",    
  "displayName": "Alice Wonderland"    
}'    

## Create Connection
curl -X POST "https://localhost:7000/api/Connections" \    
-H "Content-Type: application/json" \    
-d '{    
  "user1StrId": "alice",    
  "user2StrId": "bob"    
}'    

## Get Direct Friends
curl -X GET "https://localhost:7000/api/Users/alice/friends"    

## Remove Connection
curl -X DELETE "https://localhost:7000/api/Connections" \    
-H "Content-Type: application/json" \    
-d '{    
  "user1StrId": "alice",    
  "user2StrId": "bob"    
}'    

## Get Friends of Friends    
curl -X GET "https://localhost:7000/api/Users/alice/friends-of-friends"    

## Get Degree of Separation    
curl -X GET "https://localhost:7000/api/Connections/degree?from_user_str_id=alice&to_user_str_id=charlie"    

# ***Demo***
You can watch a video demonstration of this API's functionalities here:
  - [Demonstration Video](https://drive.google.com/file/d/1Sm0zLLXG1wXT4A8SwvZx4_-MmZsgLrHB/view?usp=sharing)  (SwaggerUI)

# Future Enhancements
  - Authentication & Authorization: Implement user authentication (e.g., JWT) and role-based authorization for API access.

  - Logging & Monitoring: Integrate a logging framework (e.g., Serilog) and monitoring tools for production environments.

  - Performance Optimization: For very large graphs, explore graph databases (e.g., Neo4j) or more advanced in-memory graph processing techniques for BFS.

  - Unit & Integration Tests: Add comprehensive unit and integration tests for all service and controller logic.

  - Dockerization: Containerize the application using Docker for easier deployment.
