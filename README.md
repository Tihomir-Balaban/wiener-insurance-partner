# Insurance Partners

Insurance Partners is an ASP.NET Core MVC web application for managing partners of an insurance company and their insurance policies.

The application was implemented according to the Assignment 1 specification: partner listing, partner creation, partner editing, partner soft delete, policy creation, policy editing, policy soft delete, validation, Dapper-based SQL Server access, Bootstrap 4 UI, and delivery-quality setup.

## Technology Stack

* ASP.NET Core MVC
* .NET 10
* SQL Server
* Dapper Micro ORM
* DbUp for SQL migration scripts
* Bootstrap 4.6.2
* jQuery
* xUnit
* FluentAssertions

## Architecture

The application uses a layered ASP.NET Core MVC monolith architecture.

```text
Controllers
    ↓
Services
    ↓
Repositories
    ↓
Dapper
    ↓
SQL Server
```

### Layers

* `Controllers` handle HTTP requests and responses.
* `Services` contain application/business logic.
* `Repositories` contain Dapper SQL access.
* `Models` represent database/domain entities.
* `ViewModels` represent UI forms, tables, modals, and AJAX responses.
* `Validation` contains custom validation logic, including Croatian OIB validation.
* `Database/Migrations` contains SQL migration scripts executed by DbUp.

## Project Structure

```text
InsurancePartners/
├── InsurancePartners.Web/
│   ├── Controllers/
│   ├── Database/
│   │   └── Migrations/
│   ├── Models/
│   ├── Repositories/
│   ├── Services/
│   ├── Validation/
│   ├── ViewModels/
│   ├── Views/
│   ├── wwwroot/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
│
├── InsurancePartners.Tests/
│   ├── Validation/
│   ├── ViewModels/
│   └── ModelValidationTestHelper.cs
│
└── InsurancePartners.sln
```

## Implemented Features

### Partner Features

* List all non-deleted partners.
* Sort partners by creation date, newest first.
* Show `FullName` instead of separate `FirstName` and `LastName`.
* Hide `Address`, `CreateByUser`, and `ExternalCode` from the main list.
* Show full partner details in a Bootstrap modal.
* Create a new partner.
* Edit an existing partner.
* Soft delete a partner.
* Highlight newly created or edited partner rows.
* Mark a partner with `*` when:

    * the partner has 5 or more policies, or
    * the total policy amount is greater than 5,000 HRK.

### Policy Features

* Add a policy to a partner through a Bootstrap modal.
* Edit a policy through a Bootstrap modal.
* Soft delete a policy.
* Update policy count, total policy amount, and the `*` marker in real time after policy changes.

### Validation

Partner validation includes:

* `FirstName`: required, 2–255 characters.
* `LastName`: required, 2–255 characters.
* `PartnerNumber`: required, exactly 20 digits.
* `CroatianPIN`: optional, but if entered must be a valid Croatian OIB.
* `PartnerTypeId`: required, allowed values are `1` Personal and `2` Legal.
* `CreateByUser`: required email address, max 255 characters.
* `ExternalCode`: required, alphanumeric, 10–20 characters.
* `Gender`: required, allowed values are `M`, `F`, and `N`.

Policy validation includes:

* `PolicyNumber`: required, alphanumeric, 10–15 characters.
* `PolicyAmount`: required, greater than 0.

### Database Constraints

The database enforces important constraints:

* Unique `PartnerNumber`.
* Unique `ExternalCode`.
* Unique `CroatianPIN` when provided.
* Unique `PolicyNumber`.
* Foreign key from `Policies` to `Partners`.
* Foreign key from `Partners` to `PartnerTypes`.
* Soft-delete columns for partners and policies.
* Row version columns for safer update handling.

## Database Setup

The application expects a SQL Server database.

Create an empty database named:

```text
InsurancePartners
```

Example SQL:

```sql
IF DB_ID(N'InsurancePartners') IS NULL
BEGIN
    CREATE DATABASE [InsurancePartners];
END;
GO
```

The tables are created automatically by DbUp when the application starts in Development mode.

DbUp migration scripts are located in:

```text
InsurancePartners.Web/Database/Migrations
```

Current migration scripts:

```text
001_CreatePartnerTypes.sql
002_CreatePartners.sql
003_CreatePolicies.sql
004_CreateIndexes.sql
005_SeedPartnerTypes.sql
```

DbUp also creates a schema version table to track executed scripts.

## Connection String

The public `appsettings.json` contains a placeholder connection string.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVER_NAME;Database=DB_NAME;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

For local development, configure:

```text
InsurancePartners.Web/appsettings.Development.json
```

Example using Windows authentication:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-87SROV5;Database=InsurancePartners;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

Do not commit real database passwords to GitHub.

## Running the Application

### Using JetBrains Rider

1. Open `InsurancePartners.sln`.
2. Confirm the startup project is `InsurancePartners.Web`.
3. Confirm the connection string in `appsettings.Development.json`.
4. Run the application.

On startup in Development mode, DbUp applies any missing SQL migration scripts.

### Using CLI

```bash
dotnet restore
dotnet build
dotnet run --project InsurancePartners.Web
```

Then open the URL shown in the console.

## Running Tests

Run all tests with:

```bash
dotnet test
```

The test project includes tests for:

* Croatian OIB checksum validation.
* Partner ViewModel validation.
* Policy ViewModel validation.

## Security Notes

Implemented security-related protections:

* Parameterized SQL through Dapper.
* Anti-forgery validation for unsafe HTTP methods.
* POST-based delete operations.
* Server-side validation.
* Friendly production error handling.
* Basic security headers:

    * `X-Content-Type-Options`
    * `X-Frame-Options`
    * `Referrer-Policy`
    * `Permissions-Policy`

## Important Assumptions

* Authentication/login is not implemented because the assignment does not explicitly require it.
* `CreateByUser` is handled as a required email input field.
* `PartnerNumber` is stored as `CHAR(20)` in SQL Server because it must preserve exactly 20 digits.
* `CroatianPIN` is stored as text because OIB is an identifier, not a number used for arithmetic.
* Delete operations are implemented as soft deletes.
* Policy entry is implemented through a Bootstrap modal, as required by the dialog/modal behavior in the specification.
* Bootstrap 4.6.2 is loaded through CDN.