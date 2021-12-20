using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIAutores.Tests.Mocks
{
    public class AuthorizationServiceMock : IAuthorizationService
    {
        public AuthorizationResult Resultado { get; set; }
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource,
            IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(Resultado); //Task porque es asincrono y retornamos success
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            return Task.FromResult(Resultado);
            //Cualquiera de los 2 metodos que se ejecuten sera exitoso por lo tanto podemos entrar a los links de admin
        }
    }
}
