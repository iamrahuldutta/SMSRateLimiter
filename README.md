# SMSRateLimiter

## Prerequisites

- [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download)
- [Node.js 18.x+](https://nodejs.org/)
- [Redis](https://redis.io/) (optional, fallback to memory cache if Redis is not available)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- Visual Studio or VS Code (optional)

Validates:
● A maximum number of messages can be sent from a single business phone number per second.
● A maximum number of messages can be sent across the entire account per second.

If either limit is exceeded, the endpoint throws HTTP 400 (Bad Request) error.

Both the configurations can be managed from appsettings.json:

"RateLimitOptions": {
  "MaxPerAccountPerSecond": 100,
  "MaxPerNumberPerSecond": 3
}


## How to Run the Project

---

### Start the API Project (`Startup` project)

#### Navigate to the API project folder:

- cd SMSRateLimiter.Startup

- dotnet restore

- cd..

- cd .\SMSRateLimiter.Infrastructure\

- update-database

- dotnet run

#### Start the React Monitoring Dashboard

- cd sms-monitoring-dashboard

- npm install

- npm start

Service	URL
Swagger (API Docs)	http://localhost:5167/swagger
React Dashboard	http://localhost:3000


