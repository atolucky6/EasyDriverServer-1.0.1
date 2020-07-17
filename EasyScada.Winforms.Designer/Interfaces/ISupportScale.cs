namespace EasyScada.Winforms.Designer
{
    public interface ISupportScale
    {
        bool EnableScale { get; set; }
        double Gain { get; set; }
        double Offset { get; set; }
        decimal RawValue { get; }
        decimal ScaledValue { get; }
    }
}
