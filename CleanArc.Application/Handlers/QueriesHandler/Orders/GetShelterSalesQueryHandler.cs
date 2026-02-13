using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Application.Queries.Order;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Orders
{
    public class GetShelterSalesQueryHandler : IRequestHandler<GetShelterSalesQuery, PaginationResponse<ShelterSaleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetShelterSalesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<ShelterSaleResponse>> Handle(GetShelterSalesQuery request, CancellationToken cancellationToken)
        {
            // Get all order items for this shelter, include the parent Order
            var shelterItems = await _unitOfWork.Repository<OrderItem>()
                .GetAsync(
                    oi => oi.ShelterId == request.ShelterId && oi.Order!.Status == "PaymentReceived",
                    cancellationToken,
                    oi => oi.Order!);

            // Group by Order to build per-order sales
            var groupedByOrder = shelterItems
                .GroupBy(oi => oi.OrderId)
                .ToList();

            int totalCount = groupedByOrder.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var pagedSales = groupedByOrder
                .OrderByDescending(g => g.First().Order!.OrderDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(g => new ShelterSaleResponse
                {
                    OrderId = g.Key,
                    OrderDate = g.First().Order!.OrderDate,
                    BuyerId = g.First().Order!.BuyerId,
                    Items = g.Select(oi => new ShelterSaleItemResponse
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        PictureUrl = oi.PictureUrl,
                        Price = oi.Price,
                        Quantity = oi.Quantity
                    }).ToList()
                }).ToList();

            return new PaginationResponse<ShelterSaleResponse>
            {
                Items = pagedSales,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
