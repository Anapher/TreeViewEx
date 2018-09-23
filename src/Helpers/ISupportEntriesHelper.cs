namespace TreeViewEx.Helpers
{
    public interface ISupportEntriesHelper<TVm>
    {
        IEntriesHelper<TVm> Entries { get; set; }
    }
}