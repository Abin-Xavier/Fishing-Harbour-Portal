# Coastal Fishing Harbour Portal

A web portal for a coastal fishing harbour — boat registration, daily fish market prices, and live harbour status — built as a DevOps portfolio project.

## Status

- ✅ **Phase 1 — Frontend**: plain HTML/CSS/JavaScript, no build step required.
- ✅ **Phase 2 — Backend**: .NET 8 Web API (boat registration, fish market, harbour status endpoints), EF Core + MySQL via Pomelo.
- ⏳ **Phase 3 — Database wiring**: schema is ready (`database/database.sql`); point the API's connection string at a live MySQL instance and run migrations to finish this phase.
- ⏳ **Phase 4–7 — Docker compose, GitHub repo, CI/CD, AWS deployment**.

## Running the frontend locally

No build tools needed. From the `frontend/` folder, either:

```bash
# Option A — just open it
open index.html

# Option B — serve it (recommended, avoids any file:// quirks)
cd frontend
python3 -m http.server 8080
# then visit http://localhost:8080
```

## Running the backend locally

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download) and a reachable MySQL server.

```bash
cd backend/FishingAPI

# 1. Point at your database — either edit appsettings.Development.json,
#    or set an environment variable (recommended, keeps secrets out of git):
export DB_CONNECTION_STRING="Server=localhost;Port=3306;Database=fishing_harbour;User=root;Password=yourpassword;"

# 2. Load the schema (once)
mysql -u root -p < ../../database/database.sql

# 3. Restore & run
dotnet restore
dotnet run
# API available at http://localhost:5000 (or the port dotnet run prints),
# Swagger UI at /swagger in Development mode.
```

Use `FishingAPI.http` (open in VS Code with the REST Client extension, or in JetBrains Rider) to try every endpoint without leaving the editor.

> **Note:** This sandbox doesn't have the .NET SDK or outbound network access, so the backend code below was written carefully but not compiled or run here. Double-check it builds with `dotnet build` once you have it on a machine with the SDK installed, and open an issue/ask if anything doesn't compile — happy to fix it.

### API endpoints

| Method | Path | Purpose |
|---|---|---|
| GET | `/api/boats` | List all registered boats |
| GET | `/api/boats/{id}` | Get one boat |
| POST | `/api/boats` | Register a new boat |
| GET | `/api/fish` | List today's fish market |
| GET | `/api/fish/{id}` | Get one fish lot |
| POST | `/api/fish` | Add a fish lot |
| PUT | `/api/fish/{id}` | Update a fish lot's quantity/price |
| DELETE | `/api/fish/{id}` | Remove a sold-out lot |
| GET | `/api/harbour-status` | Dashboard figures (active boats, today's catch, registered fishermen, cold storage) |
| PUT | `/api/harbour-status` | Update today's catch / cold storage flag |
| GET | `/health` | Liveness probe for Docker/AWS health checks |

## Structure

```
Fishing-Harbour-Portal/
│
├── frontend/
│   ├── index.html        Home
│   ├── about.html        About the Harbour (history, facilities, infrastructure)
│   ├── register.html     Boat Registration form
│   ├── market.html       Fish Market price table
│   ├── dashboard.html    Harbour Status dashboard
│   ├── contact.html      Contact page
│   ├── style.css         Shared stylesheet (design tokens at the top)
│   └── script.js         Shared header/footer injection + page interactions
│
├── backend/
│   └── FishingAPI/
│       ├── FishingAPI.csproj
│       ├── Program.cs            App startup — EF Core, CORS, Swagger, /health
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Dockerfile
│       ├── FishingAPI.http       Manual endpoint tests (VS Code REST Client / Rider)
│       ├── Models/                Boat, FishDetail, HarbourStatusEntity, User
│       ├── Data/HarbourDbContext.cs
│       ├── Dtos/                  Request/response shapes per endpoint group
│       └── Controllers/           BoatsController, FishController, HarbourStatusController
│
├── database/
│   └── database.sql      MySQL schema: boats, fish_details, harbour_status, users
│
└── README.md
```

## Notes on the frontend build

- Header and footer are injected by `script.js` on every page (`renderHeader` / `renderFooter`), so navigation only has to be edited in one place.
- The Boat Registration form validates inputs and stores demo entries in the browser's `localStorage` (visible in the "Today's Logbook" table on that page) — swap this for real `fetch()` calls to `POST /api/boats` once the backend is reachable from the page.
- The Fish Market table can be sorted by name, quantity or price client-side. Replace the hard-coded `<tr>` rows with a `fetch('/api/fish')` call to make it live.
- The Harbour Status dashboard counts up to its figures on load — replace the hard-coded `data-countto` values with a `fetch('/api/harbour-status')` call to make it live.

## Notes on the backend build

- Active boats and registered fishermen are **computed live** from the `boats` and `users` tables rather than stored separately, so the dashboard can't drift out of sync with the actual data.
- `harbour_status` is expected to hold exactly one row (today's catch tonnage + cold storage flag); the harbour office would update it via `PUT /api/harbour-status`.
- CORS is restricted to a small allow-list in `appsettings.json` (`Cors:AllowedOrigins`) — add your deployed frontend's origin there once it has a real URL.
- `User`/`users` table is wired into the DbContext but has no controller yet — that's intentionally left for the "Login system" extra feature later.

## Next steps

1. Install the .NET 8 SDK and confirm the backend builds (`dotnet build`) and runs against a local MySQL instance.
2. Point the frontend's three "live" spots above at the real API instead of static/localStorage data.
3. Continue with Phases 4–7: Dockerize both frontend (Nginx serving static files) and backend, push to GitHub, wire up GitHub Actions CI/CD, and deploy to AWS EC2 behind an Nginx reverse proxy.
