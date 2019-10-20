using Elsa.Persistence.EntityFrameworkCore;

namespace Elsa.Guides.Dashboard.WebApp
{
    // To run migrations:
    // dotnet ef database update --context ElsaContext

    public class ElsaContextFactory : DesignTimeElsaContextFactory
    {
    }
}