using System;

namespace Unity
{
    class UnityServiceProvider : IServiceProvider
    {
        private IUnityContainer Container { get; }

        public UnityServiceProvider(IUnityContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}
