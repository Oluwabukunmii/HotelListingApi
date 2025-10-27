using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelListingApi.Common
{                                                  //my errorcode is basically errortype.the badrequest or other des
    public readonly record struct Error(string ErrorCode, string Description)     //Represents an error with a unique code and human-readable description

    {
        public static readonly Error None = new("", "");                                  // A default "no error" value (useful for initialization)


        public bool IsNone => string.IsNullOrWhiteSpace(ErrorCode);         // True if no actual error exists (code is empty)

    }

    //  Represents the result of an operation (either Success or Failure)
    public readonly record struct Result
    {
        public bool IsSuccess { get; }    // true if operation succeeded
        public Error[] Errors { get; }    // contains one or more errors if failed

        private Result(bool isSuccess, Error[] errors)          // Private constructor: ensures Result can only be created via factory methods

            => (IsSuccess, Errors) = (isSuccess, errors);

        //factory helper methods

        //if result is success ,return true (for isSuccess)and []for errors,which is empty)
        public static Result Success() => new(true, []); // => means returns

        public static Result<T> Success<T>(T value) => Result<T>.Success(value);


        public static Result Failure(params Error[] errors) => new(false, errors);  //params because it allows me to pass the error array of one or more errors

        public static Result NotFound(params Error[] errors) => new(false, errors); 
        public static Result BadRequest(params Error[] errors) => new(false, errors);

        // ✅ Combines multiple results into one:
        // If ANY of them fail → collect all their errors into a single failure result.
        // Otherwise → return Success().
        public static Result Combine(params Result[] results)
            => results.Any(r => !r.IsSuccess) // check if any failed
               ? Failure(                     // if yes, return failure
                    results
                        .Where(r => !r.IsSuccess)    // get all failed results
                        .SelectMany(r => r.Errors)   // collect all their errors
                        .ToArray())                  // convert to array
               : Success();                          // otherwise success
    }

    // ✅ Generic version: holds a value if successful (e.g., Result<User> or Result<int>)
    public readonly record struct Result<T>
    {
        public bool IsSuccess { get; }    // true if success
        public T? Value { get; }          // holds actual value on success, T -means hold any type of value
        public Error[] Errors { get; }    // holds errors if failure

        // Private constructor (only created via factory methods)
        private Result(bool isSuccess, T? value, Error[] errors)
            => (IsSuccess, Value, Errors) = (isSuccess, value, errors);

        // ✅ Factory Methods
        public static Result<T> Success(T value) => new(true, value, []);          // success result with value
        public static Result<T> Failure(params Error[] errors) => new(false, default, errors); // failed result
        public static Result<T> NotFound() => new(false, default, []);             // not found result
        public static Result<T> BadRequest() => new(false, default, []);           // bad request result
        public static Result<T> BadRequest(params Error[] errors) => new(false, default, errors);

        // ✅ Map: transforms Value<T> → Value<K> (like Select in LINQ)
        // If success → apply transformation function to the value.
        // If failure → propagate the same errors.
        public Result<K> Map<K>(Func<T, K> map)
            => IsSuccess ? Result<K>.Success(map(Value!)) : Result<K>.Failure(Errors);

        // ✅ Bind (a.k.a. FlatMap or Then): chains another Result-returning function
        // If success → run next function on the value.
        // If failure → stop and return the current errors.
        public Result<K> Bind<K>(Func<T, Result<K>> next)
            => IsSuccess ? next(Value!) : Result<K>.Failure(Errors);

        // ✅ Ensure: validates a successful result using a condition (predicate)
        // If predicate returns false → convert to a failure with the given error.
        // Otherwise → keep it as is.
        public Result<T> Ensure(Func<T, bool> predicate, Error error)
            => IsSuccess && !predicate(Value!) ? Failure(error) : this;
    }
}
