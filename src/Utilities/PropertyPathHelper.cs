﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace TreeViewEx.Utilities
{
    //Thomas Levesque - http://stackoverflow.com/questions/3577802/wpf-getting-a-property-value-from-a-binding-path
    public static class PropertyPathHelper
    {
        internal static Dictionary<Tuple<Type, string>, PropertyInfo> _cacheDic =
            new Dictionary<Tuple<Type, string>, PropertyInfo>();

        private static readonly Dummy _dummy = new Dummy();

        public static object GetValueFromPropertyInfo(object obj, string[] propPaths)
        {
            var current = obj;
            foreach (var ppath in propPaths)
            {
                if (current == null)
                    return null;

                var type = current.GetType();
                var key = new Tuple<Type, string>(type, ppath);

                PropertyInfo pInfo = null;
                lock (_cacheDic)
                {
                    if (!_cacheDic.ContainsKey(key))
                    {
                        pInfo = type.GetProperty(ppath);
                        _cacheDic.Add(key, pInfo);
                    }

                    pInfo = _cacheDic[key];
                }

                if (pInfo == null)
                    return null;
                current = pInfo.GetValue(current);
            }

            return current;
        }

        public static object GetValueFromPropertyInfo(object obj, string propertyPath) =>
            GetValueFromPropertyInfo(obj, propertyPath.Split('.'));

        public static object GetValue(object obj, string propertyPath)
        {
            var dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher == null)
                return GetValueFromPropertyInfo(obj, propertyPath);

            var binding = new Binding(propertyPath) {Mode = BindingMode.OneTime, Source = obj};
            BindingOperations.SetBinding(_dummy, Dummy.ValueProperty, binding);
            return _dummy.GetValue(Dummy.ValueProperty);
        }

        public static object GetValue(object obj, BindingBase binding) => GetValue(obj, ((Binding) binding).Path.Path);

        private class Dummy : DependencyObject
        {
            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(Dummy), new UIPropertyMetadata(null));
        }
    }
}