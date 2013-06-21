using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeViewSample.DndTree;

namespace TreeViewSample.Navigation
{
    public static class RegionName
    {
        static RegionName() { }
        public const string MainRegion = "MainRegion";

        public static string GetRegionName(RegionKey region)
        {
            switch (region)
            {
                case RegionKey.MainRegion:
                    return MainRegion;
                default:
                    return string.Empty;
            }
        }

        public static IReadOnlyList<string> GetViewKeys(RegionKey region)
        {
            var list = new List<string>();
            switch (region)
            {
                case RegionKey.MainRegion:
                    list.Add(ViewKey.DndTree);
                    break;
                default:
                    break;
            }
            return list;
        }

        public static IDictionary<string, Type> GetViewTypePairs(RegionKey regionName)
        {
            var dic = new Dictionary<string, Type>();
            switch (regionName)
            {
                case RegionKey.MainRegion:
                    dic.Add(ViewKey.DndTree, typeof(IDndTreeView));
                    break;
                default:
                    break;
            }
            return dic;
        }
    }

    public static class ViewKey
    {
        static ViewKey()
        {
            uris = new Dictionary<string, Uri>();
            try
            {
                var t = typeof(DndTreeView);
                uris.Add(DndTree, new Uri(t.Name, UriKind.Relative));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private static readonly Dictionary<string, Uri> uris;

        public const string DndTree = "DndTreeView";

        public static Uri GetViewUri(string viewKey)
        {
            Uri uri;
            uris.TryGetValue(viewKey, out uri);
            return uri;
        }
    }

    public enum RegionKey
    {
        MainRegion,
    }
}
