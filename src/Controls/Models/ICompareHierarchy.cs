namespace TreeViewEx.Controls.Models
{
    public interface ICompareHierarchy<in T>
    {
        HierarchicalResult CompareHierarchy(T value1, T value2);
    }
}