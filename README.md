# StockExchange

A multi-layered .NET 8 Web API project for managing a stock exchange system. The solution is organized using clean architecture principles, separating API, business logic, and data access layers for maintainability and scalability.

## Table of Contents
- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [API Endpoints](#api-endpoints)
  - [AccountController](#accountcontroller)
  - [StocksController](#stockscontroller)
  - [TradeNotificationsController](#tradenotificationscontroller)
- [Example Requests & Responses](#example-requests--responses)
- [Notes](#notes)

---

## Features
- User registration and authentication (JWT-based)
- Role-based authorization (Read/Write)
- Stock management (CRUD)
- Trade notification processing

## Architecture
- **StockExchange.Api**: Web API layer (controllers, authentication, routing)
- **StockExchange.Core**: Business logic, domain models, service interfaces
- **StockExchange.Infrastructure**: Data access (Entity Framework, repositories, DbContext)

This separation ensures loose coupling and testability.

## Project Structure
```
StockExchange.sln
StockExchange.Api/                # API controllers, startup, config
StockExchange.Core/               # Entities, interfaces, services, models
StockExchange.Infrastructure/     # DbContext, repositories
```

## Setup Instructions
1. **Prerequisites**
  - [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

2. **Clone the repository**
  ```powershell
  git clone https://github.com/dheeraj-singh03/StockExchange.git
  cd StockExchange
  ```

3. **Run the API**
  ```powershell
  dotnet run --project StockExchange.Api
  ```

> **Note:** This project uses in-memory storage for demo purposes. No external database setup is required.

## API Endpoints

### AccountController
- `POST /api/account/register` — Register a new broker
- `POST /api/account/add-role` — Add a new role
- `POST /api/account/assign-role` — Assign a role to a broker
- `POST /api/account/login` — Authenticate and receive JWT

### StocksController
- `GET /api/stocks/get-all` — Get all stocks (Read/Write role)
- `GET /api/stocks/get-range?tickerSymbols=SYM1,SYM2` — Get stocks by ticker symbols (Read/Write role)
- `GET /api/stocks/get-single?tickerSymbol=SYM1` — Get a single stock (Read/Write role)
- `POST /api/stocks/add` — Add a new stock (Write role)

### TradeNotificationsController
- `POST /api/tradenotifications/trade` — Submit a trade notification (Write role)

## Example Requests & Responses

### Register Broker
**Request:**
```http
POST /api/account/register
Content-Type: application/json

{
  "username": "broker1",
  "password": "Password123!",
  "email": "test2@test.com"
}
```
**Response:**
```json
{
  "message": "User registered successfully"
}
```

### Login
**Request:**
```http
POST /api/account/login
Content-Type: application/json

{
  "username": "broker1",
  "password": "Password123!"
}
```
**Response:**
```json
{
  "token": "<JWT_TOKEN>"
}
<img width="1658" height="772" alt="image" src="https://github.com/user-attachments/assets/c12c8078-1eeb-4bdb-9e14-7e97570eadb4" />

```

### Get All Stocks
**Request:**
```http
GET /api/stocks/get-all
Authorization: Bearer <JWT_TOKEN>
```
**Response:**
```json
[
  {
    "stockSymbol": "AAPL",
    "price": 150.25
  },
  ...
]
<img width="1431" height="752" alt="image" src="https://github.com/user-attachments/assets/506fff8a-50f8-4438-83f5-e1f0150d955d" />

```

### Submit Trade Notification
**Request:**
```http
POST /api/tradenotifications/trade
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "tickerSymbol": "AAPL",
  "shareCount": 10,
  "price": 100,
  "brokerName": "Broker1"
}
<img width="1432" height="581" alt="image" src="https://github.com/user-attachments/assets/c1ef85be-77cd-4a54-a9ff-b5ada3f9873e" />

```
**Response:**
```json
"Trade notification processed successfully."
```


## Notes
- All endpoints (except registration and login) require JWT authentication.
- Use the `/api/account/add-role` and `/api/account/assign-role` endpoints to manage user roles.
- Use the `/api/stocks/add` endpoint to create stocks
- The project uses Entity Framework Core with an in-memory provider for demonstration. No persistent data storage is used.

---
