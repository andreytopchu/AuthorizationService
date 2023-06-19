namespace Identity.Domain.Constants;

public static class ValidationErrors
{
    public const string ValidationPath = "validations";

    public const string InvalidName = "InvalidName";
    public const string InvalidUri = "InvalidUri";
    public const string UriMustBeAbsolute = "UriMustBeAbsolute";
    public const string InvalidEmail = "InvalidEmail";
    public const string MaxLengthExceeded = "MaxLengthExceeded";
    public const string MinLengthLess = "MinLengthLess";
    public const string Required = "Required";
    public const string MustBeTrue = "MustBeTrue";
    public const string ConditionUsageMustBeTrue = "ConditionUsageMustBeTrue";
    public const string InvalidPhoneRus = "InvalidPhoneRus";
    public const string InvalidPassword = "InvalidPassword";
    public const string DateMustBeGreaterThenCurrentMoment = "DateMustBeGreaterThenCurrentMoment";
    public const string BirthYearMustBeGreaterThen1900 = "BirthYearMustBeGreaterThen1900";
    public const string BirthYearCantBeFuture = "BirthYearCantBeFuture";
    public const string UnknownTypeLinkDetected = "UnknownTypeLinkDetected";
    public const string DeepLinkMustBeValid = "DeepLinkMustBeValid";
    public const string UnknownOSDetected = "UnknownOSDetected";
    public const string NotDateTimeObject = "NotDateTimeObject";
    public const string TryingToApplyNonExistentPolicy = "TryingToApplyNonExistentPolicy";
    public const string WrongStatusStory = "WrongStatusStory";
    public const string InvalidCredentials = "InvalidCredentials";
    public const string UserNotFound = "UserNotFound";
    public const string BruteforceCooldown = "BruteforceCooldown";
}