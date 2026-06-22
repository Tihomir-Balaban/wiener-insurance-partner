namespace InsurancePartners.Web.Services;

public sealed record ServiceResult<T>(
    bool Succeeded,
    T? Value,
    IReadOnlyList<ServiceError> Errors)
{
    public static ServiceResult<T> Success(T value)
    {
        return new ServiceResult<T>(true, value, []);
    }

    public static ServiceResult<T> Failure(string key, string message)
    {
        return new ServiceResult<T>(
            false,
            default,
            [new ServiceError(key, message)]);
    }

    public static ServiceResult<T> Failure(IReadOnlyList<ServiceError> errors)
    {
        return new ServiceResult<T>(false, default, errors);
    }
}