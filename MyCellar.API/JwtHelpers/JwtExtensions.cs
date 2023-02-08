﻿using Microsoft.Extensions.Configuration;
using MyCellar.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCellar.API.JwtHelpers
{
    public static class JwtExtensions
    {
        public static void GenerateToken(this User user, IConfiguration configuration)
        {
            try
            {
                var token = new JwtTokenBuilder()
                                .AddSecurityKey(JwtSecurityKey.Create(configuration.GetValue<string>("JwtSecretKey")))
                                .AddIssuer(configuration.GetValue<string>("JwtIssuer"))
                                .AddAudience(configuration.GetValue<string>("JwtAudience"))
                                .AddExpiry(30)
                                .AddClaim("Id", user.Id.ToString())
                                //.AddClaim(CustomClaimTypes.UserId, user.Id.ToString())
                                .AddRole(user.Role)
                                .Build();
                
                user.Token = token.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
