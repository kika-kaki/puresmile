using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Responses;

namespace DatabaseContext.Models
{
    public class GoogleManager
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        public string GetProviderKeyByClinicId(int clinicId)
        {
            var clinic = _context.Clinics.FirstOrDefault(x => x.Id == clinicId);
            if (clinic == null)
            {
                return null;
            }
            var ownerEmail = clinic.OwnersEmailAddress;
            var providerKey = _context.Users.FirstOrDefault(x => x.Email == ownerEmail)
                .Logins.FirstOrDefault(x => x.LoginProvider != null)
                .ProviderKey;

            return providerKey;
        }

        public async Task SaveAsync(string userId, TokenResponse tokenResponse)
        {
            try
            {
                var result = _context.GoogleCredentials.FirstOrDefault(x => x.UserId == userId);
                if (result == null)
                {
                    GoogleCredentials googleCredentials = new GoogleCredentials()
                    {
                        AccessToken = tokenResponse.AccessToken,
                        RefreshToken = tokenResponse.RefreshToken,

                        ExpiresInSeconds = (int)tokenResponse.ExpiresInSeconds,

                        Issued = tokenResponse.Issued,
                        IssuedUtc = tokenResponse.IssuedUtc,

                        UserId = userId
                    };
                    _context.GoogleCredentials.Add(googleCredentials);
                   await _context.SaveChangesAsync();
                }

                if (tokenResponse.RefreshToken != null)
                {
                    result.RefreshToken = tokenResponse.RefreshToken;
                }
                result.AccessToken = tokenResponse.AccessToken;
                result.ExpiresInSeconds = (int)tokenResponse.ExpiresInSeconds;
                result.Issued = tokenResponse.Issued;
                result.IssuedUtc = tokenResponse.IssuedUtc;
                result.UserId = userId;

                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TokenResponse> GetAsync(string userId)
        {
            try
            {
                var dbToken = await _context.GoogleCredentials.FirstOrDefaultAsync(x => x.UserId == userId);
                TokenResponse tokenResponse = new TokenResponse()
                {
                    AccessToken = dbToken.AccessToken,
                    RefreshToken = dbToken.RefreshToken,
                    ExpiresInSeconds = dbToken.ExpiresInSeconds,
                    Issued = dbToken.Issued.Value,
                    IssuedUtc = dbToken.IssuedUtc.Value
                };

                return tokenResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

}
