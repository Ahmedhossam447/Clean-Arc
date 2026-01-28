using CleanArc.Core.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace Clean_Arc.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return MapErrorToActionResult<T>(result.Error, controller);
    }

    public static ActionResult ToActionResult(
        this Result result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.NoContent();

        return MapErrorToActionResult(result.Error, controller);
    }

    private static ActionResult<T> MapErrorToActionResult<T>(Error error, ControllerBase controller)
    {
        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Instance = controller.HttpContext.Request.Path
        };

        if (error.Code.EndsWith(".NotFound") ||
            error.Code.EndsWith(".NotFoundByEmail") ||
            error.Code.EndsWith(".OwnerNotFound") ||
            error.Code.EndsWith(".AdopterNotFound"))
        {
            return controller.NotFound(problemDetails);
        }

        if (error.Code == "Animal.AlreadyAdopted" ||
            error.Code == "Animal.CannotAdoptOwnAnimal" ||
            error.Code.EndsWith(".AlreadyExists"))
        {
            return controller.Conflict(problemDetails);
        }

        if (error.Code == "User.InvalidCredentials")
        {
            return controller.Unauthorized(problemDetails);
        }

        return controller.BadRequest(problemDetails);
    }

    private static ActionResult MapErrorToActionResult(Error error, ControllerBase controller)
    {
        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Instance = controller.HttpContext.Request.Path
        };

        if (error.Code.EndsWith(".NotFound"))
            return controller.NotFound(problemDetails);

        if (error.Code == "Animal.AlreadyAdopted" ||
            error.Code == "Animal.CannotAdoptOwnAnimal")
        {
            return controller.Conflict(problemDetails);
        }

        if (error.Code == "User.InvalidCredentials")
            return controller.Unauthorized(problemDetails);

        return controller.BadRequest(problemDetails);
    }
}
