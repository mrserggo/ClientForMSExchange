namespace MyClientForMSExchange.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using MyClientForMSExchange.Helpers;
    using Ninject;

    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            kernel.Bind<IAuthenticationHelper>().To<FormsAuthenticationHelper>();
            kernel.Bind<IMSExchangeHelper>().To<MSExchangeHelper>();
        }

    }
}