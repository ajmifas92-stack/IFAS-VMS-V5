# IFAS VMS - License Manager

This package is the `02-IFAS-LICENSE-MANAGER` module of IFAS VMS.

## Requirements

- .NET 8 SDK
- Windows/Linux/macOS for normal development
- SQLite is used automatically by the application

## Build

```bash
dotnet restore
dotnet build
```

## Run

```bash
dotnet run
```

The first run creates:

- `Data/ifas-license-manager.db`
- `Keys/ifas-license-private.pem`
- `Keys/ifas-license-public.pem`
- `Output/Licenses/`

## Important

Keep the private RSA key secret. Customers and the VMS Server must not receive it.
