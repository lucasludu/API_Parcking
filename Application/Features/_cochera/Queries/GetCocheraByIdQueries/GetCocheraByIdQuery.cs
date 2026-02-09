using Application.Wrappers;
using MediatR;
using Models.Response._cochera;

namespace Application.Features._cochera.Queries.GetCocheraByIdQueries
{
    public record GetCocheraByIdQuery(string guid) : IRequest<Response<CocheraResponse>>;
}
