namespace InsurancePartners.Web.Services;

public sealed record ServiceResult(
    bool Succeeded,
    IReadOnlyList<ServiceError> Errors)
{
    public static ServiceResult Success()
    {
        return new ServiceResult(true, []);
    }

    public static ServiceResult Failure(string key, string message)
    {
        return new ServiceResult(false, [new ServiceError(key, message)]);
    }

    public static ServiceResult Failure(IReadOnlyList<ServiceError> errors)
    {
        return new ServiceResult(false, errors);
    }
}