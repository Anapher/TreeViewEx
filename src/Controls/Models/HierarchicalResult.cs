using System;

namespace TreeViewEx.Controls.Models
{
    [Flags]
    public enum HierarchicalResult
    {
        Parent = 1 << 1,
        Current = 1 << 2,
        Child = 1 << 3,
        Unrelated = 1 << 4,
        Related = Parent | Current | Child,
        All = Related | Unrelated
    }
}