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

### The Easy Way

1. Download the [latest release](https://github.com/VerumHades/AnimeDB/releases/latest/download/release.zip)
2. Extract the zip archive
3. Find and run `AnimeDB.exe`

### The Visual Studio Way
1. **Clone the repository:**
   ```sh
   https://github.com/VerumHades/AnimeDB.git
   cd AnimeDB
   ```
    
2. **Open the project in Visual Studio**  
Open the project in visual studio by clicking the AnimeDB.sln file

3. **Install NuGet packages if missing**  
These two required NuGet packages might be missing so you will need to install them manualy:
- `Microsoft.Data.SqlClient`
- `System.Configuration.ConfigurationManager`

3. **Run the application:**  
Press the run button in visual studio

## Usage


### Navigating around the application

You may navigate using the `w`, `arrow_up` and `s`, `arrow_down` keys to select options.
![Image](./Images/screenshot.png)
> Options preceded by `>` are selected

Upon launching the application, you'll be prompted to connect to a database by entering:
> Warning: Connecting may take 5 seconds, the freeze when trying to connect, so just wait   
> Tip: Ticking the `Trust server certificate` might fix your connection issue, otherwise check your credentials

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
