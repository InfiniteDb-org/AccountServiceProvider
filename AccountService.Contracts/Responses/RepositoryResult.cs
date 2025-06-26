namespace AccountService.Contracts.Responses;

public class RepositoryResult : ResponseResult
{
}

public class RepositoryResult<T> : ResponseResult
{
    public T? Result { get; set; }

    public static RepositoryResult<T> Success(T result, string message = "") => new()
    {
        Succeeded = true, Message = message, Result = result
    };
    
    public static RepositoryResult<T> Fail(string message) => new()
    {
        Succeeded = false, Message = message, Result = default
    };
}