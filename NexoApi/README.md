# NexoApi

This directory contains backend related files, like APIs, databases, etc.

## ✨ Features

- Manage **Users**, **Tasks**, and **Projects**
- Simple REST API with basic endpoints
- Minimalistic design, easy to extend

## 🛠️ Tech Stack

- **Database:** PostgreSQL database with 4 tables (`users`, `tasks`, `projects`, and `refresh_tokens`)  
- **Packages:** Various NuGet packages for convenience and functionality

## 🌍 API Overview

**Auth**

* `POST /auth/register` → Register user
* `POST /auth/login` → Login & get JWT (with its refresh token)

**Users**

* `GET /users` → List users
* `POST /users` → Create a new user (only `pm` can do this, but I don't implement it yet. So it just behave the same like `/auth/register` 😂)

**Projects**

* `GET /projects` → List projects
* `POST /projects` → Add a new project (same like before, only `pm` can do this)

📖 That's only a few of them, full API docs available via Scalar at `/scalar`, check it out 😁 

> ⚠️ **Note:** Even tho you checked at `/scalar` you might think that the API documentation is not clear either, well indeed. I don't provide exhaustive information about the APIs, or how they supposedly works. Because I think there are still flaws or imperfections, but the APIs works no problems/bugs, and I will try to make the API documentation clear ASAP 🧙🏻‍♂️



## Installation

1. Clone this repository:

```bash
git clone https://github.com/thewitchcat/nexo.git
cd NexoApi
```


2. Restore NuGet packages:
```bash
dotnet restore
```

3. Don't forget to update your database connection string in `appsettings.json`.

4. Apply migrations and create the database:
> before ran this command, don't  forget to install `dotnet-ef` as a global tool in your `dotnet`. If you don't have it, this command will fail.
```bash
dotnet-ef database update
```

5. Run the application:
```bash
dotnet run
```

The backend API should now be running on `https://localhost:5015` (or the port you configured, check at `launchSettings.json` file).