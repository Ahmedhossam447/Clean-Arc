using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<ProfileResponse>>
    {
        private readonly IUserService _userService;

        public UpdateProfileCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<ProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateProfileAsync(
                request.UserId,
                request.FullName,
                request.PhotoUrl,
                request.Location,
                request.Bio,
                request.PhoneNumber);

            if (!result)
            {
                return UserErrors.NotFound;
            }

            var updatedProfile = await _userService.GetProfileAsync(request.UserId);
            
            if (updatedProfile == null)
            {
                return UserErrors.NotFound;
            }

            return new ProfileResponse
            {
                Id = updatedProfile.Id,
                UserName = updatedProfile.UserName,
                Email = updatedProfile.Email,
                FullName = updatedProfile.FullName,
                PhotoUrl = updatedProfile.PhotoUrl,
                Location = updatedProfile.Location,
                Bio = updatedProfile.Bio,
                PhoneNumber = updatedProfile.PhoneNumber,
                Roles = updatedProfile.Roles
            };
        }
    }
}
