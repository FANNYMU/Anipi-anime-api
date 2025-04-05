# Anipi - Professional Anime API

A comprehensive RESTful API for anime information built with ASP.NET Core.

## Features

- **Comprehensive Anime Database**: Access to a large collection of anime titles
- **Powerful Filtering**: Filter anime by title, type, status, year, season, and tags
- **Pagination**: Get paginated results for better performance
- **Sorting**: Sort results by various fields
- **Metadata Endpoints**: Access anime types, statuses, seasons, years, and popular tags
- **Well-Documented API**: Swagger UI for easy exploration and testing
- **Professional Response Format**: Consistent JSON response format with proper error handling

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later

### Running the API

1. Clone the repository
2. Navigate to the project directory
3. Run the application:

```bash
dotnet run
```

4. Open your browser and navigate to `http://localhost:5235` to access the Swagger UI

## API Endpoints

### Main Endpoints

- `GET /api`: Get API information and available versions
- `GET /api/v1/anime`: Get a paginated list of anime with optional filtering and sorting
- `GET /api/v1/anime/{id}`: Get a specific anime by ID

### Metadata Endpoints

- `GET /api/v1/types`: Get available anime types
- `GET /api/v1/statuses`: Get available anime statuses
- `GET /api/v1/seasons`: Get available seasons
- `GET /api/v1/years`: Get available years
- `GET /api/v1/tags`: Get popular tags

### Query Parameters for `/api/v1/anime`

| Parameter      | Description                            | Example                |
| -------------- | -------------------------------------- | ---------------------- |
| page           | Page number (default: 1)               | `?page=2`              |
| pageSize       | Items per page (default: 20, max: 100) | `?pageSize=50`         |
| title          | Filter by title (partial match)        | `?title=naruto`        |
| type           | Filter by exact type                   | `?type=TV`             |
| status         | Filter by exact status                 | `?status=FINISHED`     |
| year           | Filter by year                         | `?year=2022`           |
| season         | Filter by season                       | `?season=SPRING`       |
| tag            | Filter by tag (partial match)          | `?tag=action`          |
| sortBy         | Field to sort by (title, rating, year) | `?sortBy=rating`       |
| sortDescending | Sort in descending order if true       | `?sortDescending=true` |

## Example Requests

### Get anime with pagination

```
GET /api/v1/anime?page=1&pageSize=20
```

### Filter anime by title and type

```
GET /api/v1/anime?title=dragon&type=TV&page=1&pageSize=20
```

### Get anime from a specific year and season

```
GET /api/v1/anime?year=2022&season=WINTER
```

### Get anime sorted by rating

```
GET /api/v1/anime?sortBy=rating&sortDescending=true
```

## Response Format

All API responses follow a consistent format:

```json
{
  "success": true,
  "message": "Success",
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 1000,
    "totalPages": 50,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

## License

This project is licensed under the MIT License.
