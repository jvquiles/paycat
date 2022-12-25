namespace Paycat.Messages;

public class Result<T>
    where T : class
{
    public T Value { get; set; }
    public string ErrorMessage { get; set; }
    public bool IsError { get; set; }

    public Result()
    {
    }

    public Result(T value)
    {
        Value = value;
        IsError = false;
    }

    public Result(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsError = true;
    }

    public static Result<T> Ok(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Error(string errorMessage)
    {
        return new Result<T>(errorMessage);
    }
}