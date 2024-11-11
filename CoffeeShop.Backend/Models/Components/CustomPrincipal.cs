using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace CoffeeShop.Backend.Models.Components
{
    public class CustomPrincipal : ClaimsPrincipal
    {
        // 修改構造函數，確保傳遞 userId 給 CreateClaimsIdentity
        public CustomPrincipal(IIdentity identity, int id, string name, string[] functions)
            : base(CreateClaimsIdentity(identity, functions, id))  // 傳遞 id 給 CreateClaimsIdentity
        {
            Identity = identity;
            Id = id;
            Name = name;
            Functions = functions;
        }

        public new IIdentity Identity { get; private set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Functions { get; set; }

        public override bool IsInRole(string role)
        {
            return Functions != null && Functions.Contains(role.Trim().ToLower());
        }

  
        private static ClaimsIdentity CreateClaimsIdentity(IIdentity identity, string[] functions, int userId)
        {
            var claimsIdentity = new ClaimsIdentity(identity);

  
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

            if (functions != null)
            {
                foreach (var function in functions)
                {
                    claimsIdentity.AddClaim(new Claim("functionId", function.ToLower().Trim()));
                }
            }

            return claimsIdentity;
        }
    }
}