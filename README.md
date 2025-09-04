# Quizzer (C#)

## Project structure

```
Quizzer/
├─ Quizzer.sln
├─ Quizzer.Core/ # Class library: entities, DbContext, services, seed
│ ├─ Quizzer.Core.csproj
│ └─ src/
│ ├─ Entities/
│ │ ├─ QuizSet.cs
│ │ ├─ Question.cs
│ │ ├─ AnswerOption.cs
│ │ ├─ UserProfile.cs
│ │ ├─ QuizAttempt.cs
│ │ └─ AttemptAnswer.cs
│ ├─ Data/
│ │ └─ QuizDbContext.cs
│ ├─ Services/
│ │ └─ QuizEngine.cs
│ └─ Seed/SeedData.cs
└─ Quizzer.Cli/ # Console app: runs quizzes from Bash or any shell
├─ Quizzer.Cli.csproj
└─ Program.cs
```

---

## Initial Setup

```
# .NET 8 SDK
mkdir -p Quizzer && cd Quizzer

dotnet new sln -n Quizzer

dotnet new classlib -n Quizzer.Core
mkdir -p Quizzer.Core/src/{Entities,Data,Services,Seed}

# EF Core + SQLite in Core (for simplicity in Step 1)
dotnet add Quizzer.Core package Microsoft.EntityFrameworkCore --version 8.*
dotnet add Quizzer.Core package Microsoft.EntityFrameworkCore.Sqlite --version 8.*

# Console app
dotnet new console -n Quizzer.Cli

# Wire up references
dotnet sln add Quizzer.Core/Quizzer.Core.csproj Quizzer.Cli/Quizzer.Cli.csproj
dotnet add Quizzer.Cli reference Quizzer.Core/Quizzer.Core.csproj
```

---

## Run it

```
cd Quizzer
# Build solution
dotnet build

# Run CLI (uses ./quiz.db in working dir)
dotnet run --project Quizzer.Cli
```


# Next Steps

- Step 2 - REST API (ASP.NET Core Minimal API):

    - Endpoints: list quiz sets, get quiz by slug, start attempt, fetch randomized questions (server-shuffled), submit answer, complete attempt, user history.
    - Switch from EnsureCreated() to proper EF Core migrations; add authentication-ready user model (simple first: username only, later JWT/OIDC).

- Step 3 - ArchitectUI React frontend:

    - ArchitectUI shell + pages: Dashboard (recent scores, streaks), Take Quiz, Quiz Set browser, Reports (per-user, per-quiz), Admin (create/edit quiz sets & questions).
    - Serve the React build from the API (or host separately...) and add role-based routes later.

- Step 4 - Add tests to cli, core, and frontend.

---


