1.init https
   dotnet dev-certs https --trust

1. init Database
pwsh
cd Identity.Api

dotnet ef migrations add "Add Identity Schema" --project ../Identity.Infrastructure/ --context IdentityContext -o Data/Migrations
dotnet ef migrations list --project ../Identity.Infrastructure/ --context IdentityContext

dotnet ef database update --project ../Identity.Infrastructure/ --context IdentityContext

2. Update Database
``` bash
dotnet ef migrations add [Nem] --output-dir Data/Migrations
dotnet ef database update