using HotelListingApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// Converts a Result<T> (with a value) into an appropriate HTTP response.
        protected ActionResult<T> ToActionResult<T>(Result<T> result)
            => result.IsSuccess ? Ok(result.Value) : MapErrorsToResponse(result.Errors);

        /// <summary>
        /// Converts a Result (without a value) into an appropriate HTTP response.
        /// </summary>
        protected ActionResult ToActionResult(Result result)
            => result.IsSuccess ? NoContent() : MapErrorsToResponse(result.Errors);

        /// <summary>
        /// Maps domain errors (from ErrorCodes) to HTTP responses.
        /// </summary>

        protected ActionResult MapErrorsToResponse(Error[] errors)
        {
            if (errors is null || errors.Length == 0)              // If somehow we got no errors, return a generic 500 Problem.

            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An error occurred",
                    detail: "No error details provided"
                );
            }

            var e = errors[0];  // take the first error (simplify for one at a time)

            var errorDetails = string.Join("; ", errors.Select(x => x.Description));

            return e.ErrorCode switch
            {
                ErrorTypes.NotFound => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Resource not found",
                    detail: errorDetails
                ),
                ErrorTypes.Validation => ValidationProblem(
                    title: "Validation failed",
                    detail: errorDetails
                ),
                ErrorTypes.BadRequest => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Bad request",
                    detail: errorDetails
                ),
                ErrorTypes.Conflict => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: ErrorTypes.Conflict,
                    detail: errorDetails
                ),
                ErrorTypes.Forbid => Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Forbidden",
                    detail: errorDetails
                ),
                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: string.Join("; ", errors.Select(x => x.Description)),
                    title: e.ErrorCode
                )
            };
        }
    }
}
