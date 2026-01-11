using Application.Parameters;

namespace Application.Features._user.Queries.GetAllUsersQueries
{
    public class GetAllUsersParameters : PagedRequestParameters
    {
        public string? Name { get; set; }
    }
}
