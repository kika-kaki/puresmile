using System;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using DatabaseContext.Models;
using DatabaseContext;
using Google.Apis.Auth.OAuth2;
using Microsoft.Owin.Security.Google;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;

namespace PureSmileUI
{
    public partial class Startup
    {
       GoogleManager gm = new GoogleManager();
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User, int>(
                    validateInterval: TimeSpan.FromMinutes(30),
                    regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                    getUserIdCallback: (id) => (id.GetUserId<int>()))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            app.UseFacebookAuthentication(
               appId: "336843990039919",
               appSecret: "bef7c2dec23141f6c8bbf95a2bbcf699");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "524208834445-qpp0ogkhmnj4hmvcgpepchrddf23m9o8.apps.googleusercontent.com",
            //    ClientSecret = "rDsnTtYZeqTa_I7y-kpPI8uQ"
            //});
            
            var google = new GoogleOAuth2AuthenticationOptions()
            {
                
                AccessType = "offline", // Request a refresh token.
                ClientId = GoogleSecrets.ClientId,
                ClientSecret = GoogleSecrets.ClientSecret,
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    
                    OnAuthenticated = async context =>
                    {
                        var userId = context.Id;
                        context.Identity.AddClaim(new Claim("GoogleUserId", userId));

                        var tokenResponse = new TokenResponse()
                        {
                            AccessToken = context.AccessToken,
                            RefreshToken = context.RefreshToken,
                            ExpiresInSeconds = (long)context.ExpiresIn.Value.TotalSeconds,
                            Issued = DateTime.Now
                        };

                        gm.SaveAsync(userId, tokenResponse);
                    },
                },
            };

            foreach (var scope in GoogleRequestedScopes.Scopes)
            {
                google.Scope.Add(scope);
            }

            app.UseGoogleAuthentication(google);
        }
    }
}