# KitapCell — Library Management System
> **⚠️ Note:** This is a capstone project completed as a mandatory graduation requirement for an associate degree program at a Turkish university. Most of the code was written with AI assistance. You'll find Turkish and English comments mixed throughout, inconsistent structure, and shortcuts that made sense only because the deadline was the only real priority. There is no greater purpose behind this project — it was made to graduate, and it did.

---

A full-featured desktop library management system built with **C# and .NET 9**, featuring a built-in web server for browser-based access, PDF/EPUB reading, and comprehensive user management.

---

##  Features

- **Book Catalog** — Add, edit, delete, and search books with automatic cover generation
- **PDF & EPUB Reader** — Integrated document reader with progress tracking and persistent reading positions
- **Loan Management** — Track book loans and returns with due date alerts
- **User Management** — Role-based access control (Admin / Member) with profile support
- **Web Server** — Built-in Kestrel web server for browser-based library access
- **Guest Access** — Optional anonymous browsing mode for the web interface
- **Reports & Statistics** — Reading statistics, most borrowed books, and activity history
- **Dark Mode** — Full dark/light theme support
- **Settings & Backup** — Database backup/restore and factory reset functionality

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# 13 / .NET 9 |
| UI Framework | Windows Forms |
| ORM | Entity Framework Core 9 |
| Database | SQLite |
| Web Server | ASP.NET Core (Kestrel) |
| Browser Control | WebView2 |
| PDF Rendering | PDF.js |
| EPUB Rendering | Bibi |
| Icons | FontAwesome.Sharp |

---

##  Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Windows 10/11 (Windows Forms)
- Visual Studio 2022 or JetBrains Rider

### Build & Run

```bash
# Clone the repository
git clone https://github.com/yagizerhan/KitapCell.git
cd KitapCell/KitapCell

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The database (`library.db`) will be created automatically on first launch via EF Core migrations.

### Default Admin Account
| Field | Value |
|-------|-------|
| Username | `admin` |
| Password | `admin123` |

> ⚠️ Change the admin password after first login.

---

## 📁 Project Structure

```
KitapCell/
├── KitapCell/              # Main application
│   ├── Core/               # Business logic services
│   ├── Data/               # EF Core DbContext
│   ├── Models/             # Entity models
│   ├── Migrations/         # EF Core migrations
│   ├── Repositories/       # Data access layer
│   ├── Services/           # Application services
│   ├── Web/                # Web server & API controllers
│   ├── wwwroot/            # Web UI (HTML/CSS/JS)
│   └── Assets/             # Application assets & icons
├── KitapCell-Landing/      # Static landing page
└── KitapCellSetup.iss      # Inno Setup installer script
```

---

## 📄 License

This project was developed as an academic project. All rights reserved.
