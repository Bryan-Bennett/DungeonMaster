using System;

namespace Unity
{
    public static class UnityExtensions
    {
        public static IServiceProvider ToServiceProvider(this IUnityContainer container)
        {
            return new UnityServiceProvider(container);
        }
    }


}
