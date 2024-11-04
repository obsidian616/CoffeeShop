using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CoffeeShop.Backend.Models.Components
{
    public class CustomPrincipal: IPrincipal
    {
        public IIdentity Identity { get; private set; }
        // 自定義屬性
        public int Id { get; set; }

        public string Name { get; set; }
        public string[] Functions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="functions">能操作的功能"</param>
        public CustomPrincipal(IIdentity identity, int id, string name, string[] functions)
        {
            Identity = identity;
            Id = id;
            Name = name;
            Functions = functions;
        }

        public bool IsInRole(string role)
        {
            //pre condition checks
            return Functions != null && Functions.Contains(role.Trim().ToLower());
        }
    }
}