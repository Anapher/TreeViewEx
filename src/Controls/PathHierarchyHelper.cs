﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TreeViewEx.Controls.Models;
using TreeViewEx.Utilities;

namespace TreeViewEx.Controls
{
    /// <summary>
    ///     Use Path to query for hierarchy of ViewModels.
    /// </summary>
    public class PathHierarchyHelper : IHierarchyHelper
    {
        public PathHierarchyHelper(string parentPath, string valuePath, string subEntriesPath) : this()
        {
            ParentPath = parentPath;
            ValuePath = valuePath;
            SubentriesPath = subEntriesPath;
        }

        public PathHierarchyHelper()
        {
            Separator = '\\';
            StringComparisonOption = StringComparison.OrdinalIgnoreCase;
        }

        public virtual string ExtractPath(string pathName)
        {
            if (string.IsNullOrEmpty(pathName))
                return "";
            if (pathName.IndexOf(Separator) == -1)
                return "";
            return pathName.Substring(0, pathName.LastIndexOf(Separator));
        }

        public virtual string ExtractName(string pathName)
        {
            if (string.IsNullOrEmpty(pathName))
                return "";
            if (pathName.IndexOf(Separator) == -1)
                return pathName;
            if (pathName.EndsWith(":\\") && pathName.Length == 3)
                return pathName.Substring(0, 2);

            return pathName.Substring(pathName.LastIndexOf(Separator) + 1);
        }

        public IEnumerable<object> GetHierarchy(object item, bool includeCurrent)
        {
            if (includeCurrent)
                yield return item;

            var current = GetParent(item);
            while (current != null)
            {
                yield return current;
                current = GetParent(current);
            }
        }

        public string GetPath(object item) => item == null ? "" : GetValuePath(item);

        public object GetItem(object rootItem, string path)
        {
            var queue = new Queue<string>(path.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries));
            var current = rootItem;
            while (current != null && queue.Any())
            {
                var nextSegment = queue.Dequeue();
                object found = null;
                foreach (var item in List(current))
                {
                    var valuePathName = GetValuePath(item);
                    var value = ExtractName(valuePathName); //Value may be full path, or just current value.
                    if (value.Equals(nextSegment, StringComparisonOption))
                    {
                        found = item;
                        break;
                    }
                }

                current = found;
            }

            return current;
        }

        public async ValueTask<object> GetItemAsync(object rootItem, string path)
        {
            var queue = new Queue<string>(path.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries));
            var current = rootItem;
            while (current != null && queue.Any())
            {
                var nextSegment = queue.Dequeue();
                object found = null;
                foreach (var item in await ListAsync(current))
                {
                    var valuePathName = GetValuePath(item);
                    var value = ExtractName(valuePathName); //Value may be full path, or just current value.
                    if (value.Equals(nextSegment, StringComparisonOption))
                    {
                        found = item;
                        break;
                    }
                }

                current = found;
            }

            return current;
        }

        public char Separator { get; set; }
        public StringComparison StringComparisonOption { get; set; }
        public string ParentPath { get; set; }
        public string ValuePath { get; set; }
        public string SubentriesPath { get; set; }

        public ValueTask<IEnumerable> ListAsync(object item) => item is IEnumerable enumerable ? new ValueTask<IEnumerable>(enumerable) : GetAutoCompleteEntries(item);
        public IEnumerable List(object item) => item is IEnumerable enumerable ?  enumerable : GetSubEntries(item);

        protected virtual object GetParent(object item) =>
            PropertyPathHelper.GetValueFromPropertyInfo(item, ParentPath);

        protected virtual string GetValuePath(object item) =>
            PropertyPathHelper.GetValueFromPropertyInfo(item, ValuePath) as string;

        protected virtual IEnumerable GetSubEntries(object item) =>
            PropertyPathHelper.GetValueFromPropertyInfo(item, SubentriesPath) as IEnumerable;

        protected virtual ValueTask<IEnumerable> GetAutoCompleteEntries(object item) =>
            ((IAsyncAutoComplete) item).GetAutoCompleteEntries();
    }

    public interface IAsyncAutoComplete
    {
        ValueTask<IEnumerable> GetAutoCompleteEntries();
    }

    /// <summary>
    ///     Generic version of AutoHierarchyHelper, which use Path to query for hierarchy of ViewModels
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PathHierarchyHelper<T> : PathHierarchyHelper
    {
        private readonly PropertyInfo _propInfoParent;
        private readonly PropertyInfo _propInfoSubEntries;
        private readonly PropertyInfo _propInfoValue;

        public PathHierarchyHelper(string parentPath, string valuePath, string subEntriesPath) : base(parentPath,
            valuePath, subEntriesPath)
        {
            _propInfoSubEntries = typeof(T).GetProperty(subEntriesPath);
            _propInfoValue = typeof(T).GetProperty(valuePath);
            _propInfoParent = typeof(T).GetProperty(parentPath);
        }

        protected override object GetParent(object item) => _propInfoParent.GetValue(item);

        protected override IEnumerable GetSubEntries(object item) => _propInfoSubEntries.GetValue(item) as IEnumerable;

        protected override string GetValuePath(object item) => _propInfoValue.GetValue(item) as string;
    }
}