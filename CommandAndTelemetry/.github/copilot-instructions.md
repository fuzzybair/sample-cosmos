# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction
- Always verify NuGet package versions before recommending exact version numbers; Npgsql.EntityFrameworkCore.PostgreSQL may not have matching 10.0.2—do not assume package versions.
- When using the generic Host in console apps, add Microsoft.Extensions.Hosting and Microsoft.Extensions.DependencyInjection NuGet packages if types under Microsoft.Extensions.* are unresolved.

## Code Style
- Use specific formatting rules
- Follow naming conventions

## Project-Specific Rules
- Project should use PostgreSQL snake_case naming convention and enable UseSnakeCaseNamingConvention for the PostgreSQL provider.
- Attempt automatic database creation at startup: try applying migrations (Migrate), if that fails fallback to EnsureCreated(), and if that fails, generate SQL creation scripts to a file.