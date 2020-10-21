using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PtcApi.Model;

namespace PtcApi.Security
{
    public class SecurityManager {
        private JwtSettings _settings;
        public SecurityManager(JwtSettings settings)
        {
            _settings = settings;
        }

        public AppUserAuth ValidateUser(AppUser user) {
            AppUserAuth returnedUser = new AppUserAuth();
            AppUser authUser = null;
            using(var db = new PtcDbContext()) {
                authUser = db.Users.Where(
                    u => u.UserName == user.UserName
                    && u.Password == user.Password 
                ).FirstOrDefault();
            }

            if (authUser != null) {
                returnedUser = BuildUserAuthObject(authUser);
            }

            return returnedUser;
        }

        protected List<AppUserClaim> GetUserClaims(AppUser authUser) {
            List<AppUserClaim> list = new List<AppUserClaim>();
            try
            {
                using(var db = new PtcDbContext()) {
                    list = db.Claims.Where(u => u.UserId == authUser.UserId).ToList();
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception("Exception trying to get the claims for User:", ex);
            }

            return list;
        }

        protected AppUserAuth BuildUserAuthObject(AppUser authUser) {
            AppUserAuth userAuth = new AppUserAuth();
            List<AppUserClaim> claims = new List<AppUserClaim>();

            userAuth.UserName = authUser.UserName;
            userAuth.IsAuthenticated = true;
            userAuth.BearerToken = new Guid().ToString();
            userAuth.Claims = GetUserClaims(authUser);
            userAuth.BearerToken = BuildJwtToken(userAuth);
            return userAuth;
        }

        protected string BuildJwtToken(AppUserAuth authUser)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            List<Claim> JwtClaims = new List<Claim>();
            JwtClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, authUser.UserName));
            JwtClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            // Add custom claims
            JwtClaims.Add(new Claim("isAuthenticated", authUser.IsAuthenticated.ToString().ToLower()));
            authUser.Claims.ForEach(i =>
            {
                JwtClaims.Add(new Claim(i.ClaimType, i.ClaimValue.ToString().ToLower()));
            });

            // create the jwt token here.
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: JwtClaims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_settings.MinutesToExpiration),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }    
}