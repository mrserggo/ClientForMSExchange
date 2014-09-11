namespace MyClientForMSExchange.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Core.EntityFrameworkDAL.Entities;
    using Core.EntityFrameworkDAL.Repositories;
    using Core.EntityFrameworkDAL.Repositories.Interfaces;

    using MyClientForMSExchange.Helpers;
    using MyClientForMSExchange.Models;

    using Ninject;

    /// <summary>
    /// The ninject dependency resolver.
    /// </summary>
    public class NinjectDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// The kernel.
        /// </summary>
        private IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectDependencyResolver"/> class.
        /// </summary>
        public NinjectDependencyResolver()
        {
            this.kernel = new StandardKernel();
            this.AddBindings();
        }

        /// <summary>
        /// The get service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return this.kernel.TryGet(serviceType);
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.kernel.GetAll(serviceType);
        }

        /// <summary>
        /// The add bindings.
        /// </summary>
        private void AddBindings()
        {
            this.kernel.Bind<IAuthenticationHelper>().To<FormsAuthenticationHelper>();
            this.kernel.Bind<IMSExchangeHelper>().To<MSExchangeHelper>();
            this.kernel.Bind(typeof(IRepository<Catalog>)).To<Repository<Catalog>>().WithConstructorArgument("dataContext", new MyClientForMSExchangeContainer());
            this.kernel.Bind(typeof(IRepository<EmailItem>)).To<Repository<EmailItem>>().WithConstructorArgument("dataContext", new MyClientForMSExchangeContainer());
            this.kernel.Bind(typeof(IRepository<Client>)).To<Repository<Client>>().WithConstructorArgument("dataContext", new MyClientForMSExchangeContainer());
        }
    }
}