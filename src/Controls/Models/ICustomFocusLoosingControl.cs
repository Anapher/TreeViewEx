using System;

namespace TreeViewEx.Controls.Models
{
    public interface ICustomFocusLoosingControl
    {
        event EventHandler FocusLost;
    }
}