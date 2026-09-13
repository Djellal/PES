# AGENTS.md

PES is a Radzen-generated Blazor Server app (French UI) for evaluating trainees and printing/sending attestations, backed by PostgreSQL. The deployable app is `APP/server` (`Pes.csproj`, net9.0).

## Structure

- `APP/server` — the real Blazor Server app (Radzen.Blazor + EF Core/Npgsql + ASP.NET Identity).
- `APP/meta` — Radzen project source (`app.json`, `pages/*.json`, `data/DMdel.json`); the server app is generated from it with the Radzen desktop app.
- `Database/DataModel` — scaffold EF Core project (net7.0) that owns the app-schema migrations. The server's own `Data/Migrations` cover **Identity only**.
- `others/` — systemd unit + Apache vhost used in production, miscellaneous assets, and the Syncfusion DLLs referenced by `Pes.csproj` (required for the build; don't move/delete).
- `PES.sql` / `others/PES.sql` — PostgreSQL dumps of the `PES` database.

## Build / run

- Build app: `dotnet build APP/server/Pes.csproj` (SDK 10.0.x installed; targets net9.0). Succeeds; the known `NU1902` advisories on MailKit/MimeKit are pre-existing and safe to ignore.
- Build schema project: `dotnet build Database/DataModel/DataModel/DataModel.csproj`. Warnings (`CS8981` lowercase migration names, `CS8618`) are pre-existing.
- Run app: `dotnet run --project APP/server` → binds `http://localhost:5033` (hard-coded in `Program.cs`).
- No tests, no CI, no linter/formatter configuration in this repo.

Requires a local PostgreSQL DB `PES` with user `pes` / `pes@2023`. Connection string is read from the `ConnectionStrings__DMdelConnection` env var (`APP/.env` holds it) and falls back to `appsettings.json`, which also contains the SMTP creds used to email attestations.

## Architecture & gotchas

- Two EF contexts share one DB: `ApplicationIdentityDbContext` (Identity; migrations in `APP/server/Data/Migrations`) and `DMdelContext` (app schema; migrations only in `Database/DataModel`).
- **Generated files — don't hand-edit:** `DMdelService.cs`, `DMdelContext.cs`, `Models/DMdel/*.cs`, and page `.razor` / `.razor.designer.cs`. Custom behavior goes in the `*.Custom.cs` partials (`DMdelService.Custom.cs`, `SecurityService.Custom.cs`, ...), page `.razor.cs` partials, and `Startup.custom.cs`. `app.json` marks `Program.cs` / `Startup.cs` as ignored by regeneration.
- **`Data/DMdelContext.Custom.cs` is excluded from the build** (`<Compile Remove="Data\DMdelContext.Custom.cs">` in `Pes.csproj`); edits there have no effect.
- To change the DB schema: update a model in `Database/DataModel` and add a migration there (`dotnet ef` via that project), then propagate the model changes into `APP/server/Models/DMdel` and `APP/meta/data/DMdel.json` (normally through Radzen regeneration) — the server keeps its own copy of models and relationship config.
- Role checks use the `Constants.cs` string constants: `admin`, `coordinateur`, `membre_jury`, `president_jury`, `expert`. Jury-role users only see trainees of their own `Etabid`.
- Global state (active session, selected etablissement) comes from `GlobalsService.ActiveSession` / `.Etab`.
- Keep UI text French and use Radzen components (e.g. `RadzenDataGrid`, `RadzenDropdown`) rather than plain HTML tables.

## Deployment

`dotnet publish -c Release`, then run `Pes.dll` under systemd (`others/pes.service`, working dir `/home/pgadmin/pes-app/`) behind Apache (`others/pes.conf`), which proxies port 5033 and websockets for `/_blazor`.