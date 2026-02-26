using Hangfire.Dashboard;
namespace CleanArc.API.Extensions
{


    public class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // This allows access to the dashboard in your local Docker environment.
            // In a real production app
            return true;
        }
    }
}
