using MarketPlace.Domain.Common.BaseErrors;
namespace MarketPlace.Domain.Common.ResultPattern;
public class Result<TValue>
{
    private readonly TValue? _value; // this is the value of the result, it can be null if the result is a failure

    public bool IsSuccess { get; } // this is a boolean that indicates whether the result is a success or a failure and it is set in the constructor
    public bool IsFailure => !IsSuccess; // this is a boolean that indicates whether the result is a failure or a success and it is the opposite of IsSuccess
    public BaseError Error { get; } // this is the error of the result, it can be null if the result is a success

    private Result(TValue? value) // this is a private constructor that takes a value and sets the IsSuccess property to true, the _value property to the value, and the Error property to null
    {
        IsSuccess = true;
        _value = value;
        Error = null;
    }
    private Result(BaseError error) // this is a private constructor that takes an error and sets the IsSuccess property to false, the _value property to null, and the Error property to the error
    {
        IsSuccess = false;
        _value = default;
        Error = error;
    }

    public TValue Value => IsSuccess ? _value! :
    throw new InvalidOperationException("Cannot access the value of a failed result.");

    //success
    public static Result<TValue> Success(TValue value) // this is a static method that takes a value and returns a new Result<TValue> object with the value set to the value and the IsSuccess property set to true
    {
        return new Result<TValue>(value);
    }
//failure
    public static Result<TValue> Failure(BaseError error) // this is a static method that takes an error and returns a new Result<TValue> object with the Error property set to the error and the IsSuccess property set to false
    {
        return new Result<TValue>(error);
    }



    public static implicit operator Result<TValue>(TValue value) => Success(value); // this is an implicit operator that allows you to convert a TValue to a Result<TValue> by calling the Success method with the value

    public static implicit operator Result<TValue>(BaseError error) => Failure(error);

 
}

public class Result // this is a non-generic version of the Result<TValue> class that can be used when you don't need to return a value, only indicate success or failure
{
    private Result(bool isSuccess, BaseError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    private bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public BaseError? Error { get; }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(BaseError error)
    {
        return new Result(false, error);
    }

    public static implicit operator Result(BaseError error)
    {
        return Failure(error);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return Result<TValue>.Success(value);
    }

    public static Result<TValue> Failure<TValue>(BaseError error)
    {
        return Result<TValue>.Failure(error);
    }
}