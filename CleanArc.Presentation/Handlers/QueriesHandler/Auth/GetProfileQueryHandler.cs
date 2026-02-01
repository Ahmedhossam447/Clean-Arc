using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Application.Queries.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Auth
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileResponse>>
    {
        private readonly IUserService _userService;

        public GetProfileQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<ProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _userService.GetProfileAsync(request.UserId, cancellationToken);
            
            if (profile == null)
            {
                return UserErrors.NotFound;
            }

            return new ProfileResponse
            {
                Id = profile.Id,
                UserName = profile.UserName,
                Email = profile.Email,
                FullName = profile.FullName,
                PhotoUrl = profile.PhotoUrl,
                Location = profile.Location,
                Bio = profile.Bio,
                PhoneNumber = profile.PhoneNumber,
                Roles = profile.Roles
            };
        }
    }
}
