# SMSRateLimiter

## Prerequisites

- [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download)
- [Node.js 18.x+](https://nodejs.org/)
- [Redis](https://redis.io/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- Visual Studio or VS Code (optional)

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


