using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreAuthentication
{
    public class Customer
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public string SalesRep { get; set; }
    }

    public class CustomerOperations
    {
        public static  OperationAuthorizationRequirement Add =
            new OperationAuthorizationRequirement { Name = "Add" };
        public static OperationAuthorizationRequirement Edit =
            new OperationAuthorizationRequirement { Name = "Edit" };
        public static OperationAuthorizationRequirement Invoice =
            new OperationAuthorizationRequirement { Name = "Invoice" };
    }

    public class CustomerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Customer>
    {
        protected override void Handle(
            AuthorizationContext context, 
            OperationAuthorizationRequirement requirement, 
            Customer customer)
        {
            if (requirement == CustomerOperations.Edit)
            {
                if (context.User.HasClaim("role", "Sales"))
                {
                    if (context.User.FindFirst("sub")?.Value == customer.SalesRep)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
