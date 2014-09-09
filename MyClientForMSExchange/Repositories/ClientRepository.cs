using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyClientForMSExchange.Models.Entities;
using MyClientForMSExchange.Repositories.Interfaces;

namespace MyClientForMSExchange.Repositories
{

    //public class ClientRepository : BaseRepository<Client>, IClientRepository
    //{
    //    #region Constructor

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="AdminRepository"/> class.
    //    /// </summary>
    //    /// <param name="repositoryContext">The repository context.</param>
    //    public ClientRepository(IRepositoryContext repositoryContext)
    //        : base(repositoryContext)
    //    {
    //    }

    //    #endregion

    //    #region Implementation of IClientRepository

    //    /// <summary>
    //    /// Gets the admin.
    //    /// </summary>
    //    /// <param name="userEmail">Name of the user.</param>
    //    /// <returns></returns>
    //    public Client GetClient(String userEmail)
    //    {
    //        return GetQueryable().FirstOrDefault(p => p.Email == userEmail);
    //    }

    //    #endregion
    //}
}