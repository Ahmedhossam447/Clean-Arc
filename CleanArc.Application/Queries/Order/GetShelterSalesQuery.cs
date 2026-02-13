using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Order;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Queries.Order
{
    public class GetShelterSalesQuery : IRequest<PaginationResponse<ShelterSaleResponse>>
    {
        [JsonIgnore]
        public string ShelterId { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
