using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Application.Queries.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Auth
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<PublicProfileResponse>>
    {
        private readonly IUserService _userService;

        public GetUserProfileQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<PublicProfileResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _userService.GetProfileAsync(request.UserId, cancellationToken);
            
            if (profile == null)
            {
                return UserErrors.NotFound;
            }

            // Return public profile (excludes sensitive information like email and phone)
            return new PublicProfileResponse
            {
                Id = profile.Id,
                UserName = profile.UserName,
                FullName = profile.FullName,
                PhotoUrl = profile.PhotoUrl,
                Location = profile.Location,
                Bio = profile.Bio
            };
        }
    }
}
