namespace EasyDriverPlugin
{
    /// <summary>
    /// Định nghĩa đối tượng sẽ chứa các parameters
    /// </summary>
    public interface ISupportParameters
    {
        IParameterContainer ParameterContainer { get; set; }
    }
}
