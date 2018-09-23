namespace TreeViewEx.Helpers.Selectors
{
    public interface ISupportTreeSelector<VM, T> : ISupportEntriesHelper<VM>
    {
        ITreeSelector<VM, T> Selection { get; set; }
    }
}