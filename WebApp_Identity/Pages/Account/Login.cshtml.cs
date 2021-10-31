using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        //two-way data binding
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
            this.Credential = new Credential { UserName = "admin" };
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            //return page result
            if (!ModelState.IsValid) return Page();

            //verify the credential
            if (Credential.UserName=="admin" && Credential.Password=="password")
            {
                //creating the security context
                //claims are key value pairs
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", ""),
                    //new Claim("Admin", "true"),
                    new Claim("Manager", ""),
                    new Claim("HireDate", "2021-05-01")
                    };
                //auth type will be a cookie, the scheme is called "MyCookieAuth" (as registered in Startup file)
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                //create claims principal to hold the security context. Can be empty or can provide a primary "identity"
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                //to keep a persisent cookie for a period of time regardless of browser closing
                var authProperties = new AuthenticationProperties {
                    IsPersistent = Credential.RememberMe
                };

                //to encrypt and serialize the security context into the cookie to/from header
                //needs a cookie authentication handler. this is registered in startup file
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
                return RedirectToPage("/index");
            }

            return Page();
        }


    }

    //model to communicate between front and back
    public class Credential 
    {
        [Required]
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }


    }






}
