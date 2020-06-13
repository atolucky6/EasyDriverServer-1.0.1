namespace EasyScada.Api.Interfaces
{
    public enum WriteMode
    {
        WriteAllValue,
        WriteLatestValue,
        WriteLatestValueForBooleanTag,
        WriteLastestValueForNonBooleanTag
    }
}
