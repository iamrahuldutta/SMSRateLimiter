# SMSRateLimiter

## ✅ Prerequisites

- [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download)
- [Node.js 18.x+](https://nodejs.org/)
- [Redis](https://redis.io/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- Visual Studio or VS Code (optional)

## 🚀 How to Run the Project

---

### 1️⃣ Start the API Project (`Startup` project)

#### 📂 Navigate to the API project folder:
```bash
cd SMSRateLimiter.API

dotnet restore

cd..

cd SMSRateLimiter.Infrastructure

dotnet ef database update

dotnet run


Start the React Monitoring Dashboard

cd sms-monitoring-dashboard

npm install

npm start


http://localhost:3000
