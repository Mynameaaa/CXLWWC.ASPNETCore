using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Server.Custom.FeatureCollection
{
    public class CustomFeatureCollection : Dictionary<Type, object>, ICustomFeatureCollection
    {
        
    }
    public static partial class Extensions
    {
        public static T Get<T>(this ICustomFeatureCollection features) => features.TryGetValue(typeof(T), out var value) ? (T)value : default(T);
        public static ICustomFeatureCollection Set<T>(this ICustomFeatureCollection features, T feature)
        {
            features[typeof(T)] = feature;
            return features;
        }
    }
}
