# AnimeDB Console Application

AnimeDB is a console-based application for managing an anime database. 
It allows users to connect to a database, manage anime-related data, and perform various operations such as creating, renaming, and deleting entries.

## Features

- Connect to an SQL database with configurable credentials.
- Create and manage users, genres, anime, movie episodes, and watchlist entries.
- Import and export user data.
- Delete anime entries from the database.
- Rename users in the database.
- Reset the database (WARNING: This will delete all data).
- Simple text-based menu navigation.

## Requirements

- .NET SDK (latest version recommended)
- SQL Database instance for data storage
- Required NuGet packages:
	- `Microsoft.Data.SqlClient`
	- `System.Configuration.ConfigurationManager`

## Getting Started

1. **Clone the repository:**
   ```sh
   git clone https://github.com/your-repo/anime-db.git
   cd anime-db
   ```

2. **Build the project:**
   ```sh
   dotnet build
   ```

3. **Run the application:**
   ```sh
   dotnet run
   ```

## Usage

Upon launching the application, you'll be prompted to connect to a database by entering:
- Server address
- Database name
- Username and password

Once connected, you can navigate the menu to manage your anime database.

### Available Actions:
- **Create**: Users, genres, anime, movie episodes, watchlist entries
- **Export/Import**: User data to/from a file
- **Delete**: Remove an anime entry
- **Rename**: Change a username
- **Reset**: Erase all database contents (use with caution)

## Notes

- If the database connection fails, the application will return to the connection menu.
- The database connection timeout is set to **5 seconds**.
- Trusting the server certificate may be required for some connections.

## Contributing

Feel free to submit issues or pull requests to improve the project.

## License

This project is licensed under the MIT License.

---

Let me know if you'd like any modifications! 😊