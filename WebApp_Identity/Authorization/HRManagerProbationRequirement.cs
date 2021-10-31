using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp_Identity.Authorization
{
    //register in startup file
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        

        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }


    //handler as abstract class, need DI for auth handler
    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            //get date and check if passed period
            //get claim

            //do not return a fail because need to allow checking on different policies
            if (!context.User.HasClaim(x => x.Type == "HireDate"))
                //return empty task
                return Task.CompletedTask;

            var hireDate = DateTime.Parse(context.User.FindFirst(x => x.Type == "HireDate").Value);
            var period = DateTime.Now - hireDate;
            if (period.Days > 30 * requirement.ProbationMonths)
                context.Succeed(requirement);
            return Task.CompletedTask;

            
        }
    }







}
