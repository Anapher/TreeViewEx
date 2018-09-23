using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TreeViewEx.Utilities
{
    public static class WpfExtensions
    {
        public static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            var returnVal = source;

            while (returnVal != null && !(returnVal is T))
            {
                DependencyObject tempReturnVal = null;
                if (returnVal is Visual || returnVal is Visual3D) tempReturnVal = VisualTreeHelper.GetParent(returnVal);
                if (tempReturnVal == null)
                    returnVal = LogicalTreeHelper.GetParent(returnVal);
                else returnVal = tempReturnVal;
            }

            return returnVal as T;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                //get parent item
                var parentObject = VisualTreeHelper.GetParent(child);

                //we've reached the end of the tree
                if (parentObject == null) return null;

                //check if the parent matches the type we're looking for
                if (parentObject is T parent) return parent;

                child = parentObject;
            }
        }

        public static T GetDescendantByType<T>(Visual element) where T : Visual
        {
            if (element == null)
                return default;

            if (element.GetType() == typeof(T))
                return (T) element;

            T foundElement = null;
            (element as FrameworkElement)?.ApplyTemplate();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var visual = child as Visual;
                foundElement = GetDescendantByType<T>(visual);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }
    }
}