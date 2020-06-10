namespace EasyScada.ServerApplication
{
    /// <summary>
    /// Interface hỗ trợ việc Copy/Cut/Paste đối tượng
    /// </summary>
    public interface ISupportEdit
    {
        void Copy();
        bool CanCopy();
        void Cut();
        bool CanCut();
        void Paste();
        bool CanPaste();
        void Delete();
        bool CanDelete();
    }
}
