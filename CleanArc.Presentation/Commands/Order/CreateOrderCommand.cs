using CleanArc.Application.Contracts.Requests.Order;
using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Core.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Order
{
    public class CreateOrderCommand :IRequest<Result<CreateOrderResponse>>
    {
        public List<CartItemRequest> Items { get; set; }
        [JsonIgnore]
        public string CustomerId { get; set; }
        [JsonIgnore]
        public string?CustomerEmail { get; set; }
    }
}
