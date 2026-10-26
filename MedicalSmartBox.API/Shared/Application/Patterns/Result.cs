namespace MedicalSmartBox.API.Shared.Application.Patterns;

public abstract record Result<TValue, TError>
{
    public sealed record Success(TValue Value) : Result<TValue, TError>;
    public sealed record Failure(TError Error) : Result<TValue, TError>;
    
    public bool IsSuccess => this is Success;

    public bool IsFailure => this is Failure;
    
    public Result<TNext, TError> Map<TNext>(Func<TValue, TNext> onSuccess) => this switch
    {
        Success success => new Result<TNext, TError>.Success(onSuccess(success.Value)),
        Failure failure => new Result<TNext, TError>.Failure(failure.Error),
        _ => throw new InvalidOperationException("Invalid result state.")
    };
    
    public TResult Fold<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure) => this switch
    {
        Success success => onSuccess(success.Value),
        Failure failure => onFailure(failure.Error),
        _ => throw new InvalidOperationException("Invalid result state.")
    };

    public void Match(Action<TValue> onSuccess, Action<TError> onFailure)
    {
        switch (this)
        {
            case Success success:
                onSuccess(success.Value);
                break;
            case Failure failure:
                onFailure(failure.Error);
                break;
            default:
                throw new InvalidOperationException("Invalid result state.");
        }
    }
}