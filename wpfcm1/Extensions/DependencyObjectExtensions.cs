using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace wpfcm1.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T FindVisualParent<T>(this DependencyObject depObj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(depObj);
            if (parent == null || parent is T)
                return (T)parent;
            return FindVisualParent<T>(parent);
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T) child;
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        public static T FindLogicalParent<T>(this DependencyObject depObj) where T : DependencyObject
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(depObj);
            if (parent == null || parent is T)
                return (T)parent;
            return FindLogicalParent<T>(parent);
        }
    }
}
