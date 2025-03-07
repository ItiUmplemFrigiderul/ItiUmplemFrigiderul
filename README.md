# Online Shop - ASP.NET

## 📌 Project Overview
This is a fully functional online shop built using ASP.NET. The application allows users to browse products, add them to the cart, and proceed with checkout. It includes authentication, a product management system, and an order processing module.

## 🚀 Features
- User authentication (login/register)
- Product listing and filtering
- Shopping cart functionality
- Order processing
- Admin panel for managing products

## 📂 Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap for UI

## 🛠️ Installation & Setup
Follow these steps to clone and run the project locally:

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/ItiUmplemFrigiderul/ItiUmplemFrigiderul
cd ItiUmplemFrigiderul
```

### 2️⃣ Configure the Database
- Make sure you have **SQL Server** installed and running.
- Update the `appsettings.json` file with your database connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;"
}
```
- Apply migrations and seed the database:
```bash
dotnet ef database update
```

### 3️⃣ Run the Application
```bash
dotnet run
```
The app should now be accessible at `http://localhost:5174/`.

## ✅ Running the Project in Visual Studio
1. Open the solution file `ItiUmplemFrigiderul.sln` in **Visual Studio**.
2. Set `ItiUmplemFrigiderul` as the **Startup Project**.
3. Press `F5` to build and run the application.

