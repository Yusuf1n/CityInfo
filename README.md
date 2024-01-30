# CityInfo

The API provides information about cities and their points of interest. With the API, you can perform all CRUD operations, making it a great tool for managing city-related data.

### Technologies Used 
- .NET 6
- ASP.NET Core 6 Web API
- EF Core 6
- SQL Server

### Features
- Dependency Injection (DI)
- Logging

### Dependencies
- Serilog
- AutoMapper

### Getting Started
To get started with the CityInfo API, follow these steps:

1. Clone the repository:
```console
git clone https://github.com/yusuf1n/CityInfo
```

2. Navigate to the project directory:
```console
cd CityInfo
```

3. Install dependencies:
```console
dotnet restore
```

4. Update database:
```console
dotnet ef database update
```
5. Run the application:
```console
dotnet run
```

The API will be accessible at https://localhost:7285 by default.
