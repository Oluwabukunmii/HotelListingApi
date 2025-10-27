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
            // If somehow we got no errors, return a generic 500 Problem.
            if (errors == null || errors.Length == 0)
                return Problem();

            var e = errors[0]; // take the first error (simplify for one at a time)

            // Match each error code to an appropriate HTTP status code
            return e.ErrorCode switch
            {
                ErrorTypes.NotFound => NotFound(e.Description),
                ErrorTypes.Validation => BadRequest(e.Description),
                ErrorTypes.BadRequest => BadRequest(e.Description),
                ErrorTypes.Conflict => Conflict(e.Description),
                ErrorTypes.Forbid => Forbid(e.Description),
                _ => Problem(detail: string.Join("; ", errors.Select(x => x.Description)), title: e.ErrorCode)
            };
        }
    }
}
