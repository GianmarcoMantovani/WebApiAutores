using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;
        public CuentasController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_quizas_secreto"); // A partir de esto hacemos la encriptacion
        }

     /*   [HttpGet("hash/{textoPlano}")]
        public ActionResult RealizarHash(string textoPlano)
        {
            var resultado1 = hashService.Hash(textoPlano);
            var resultado2 = hashService.Hash(textoPlano);
            return Ok(new
            {
                textoPlano = textoPlano,
                Hash1 = resultado1,
                Hash2 = resultado2
            });

        }

        [HttpGet("encriptar")]
        public ActionResult Encriptar()
        {
            var textoPlano = "Felipe Gavilan";
            var textoCifrado = dataProtector.Protect(textoPlano);
            var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDesencriptado = textoDesencriptado
            });
        }

        [HttpGet("encriptarPorTiempo")]
        public ActionResult EncriptarPorTiempo()
        {
            var protectorLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();

            var textoPlano = "Felipe Gavilan";
            var textoCifrado = protectorLimitadoPorTiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000); //Paralizo el hilo ejecutado durante 6 seg
            var textoDesencriptado = protectorLimitadoPorTiempo.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDesencriptado = textoDesencriptado
            });
        }*/

        [HttpPost("registrar", Name = "registrarUsuario")] // ruta= api/cuentas/registrar
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        //Traemos el token y su fecha de expiracion y creamos la funcion registrar
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            }; //Tomamos el email
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);
            //Tomamos como resultado el usuario y el password y ya tenemos el usuario creado

            if (resultado.Succeeded) //Usuario creado correctamente
            {
                return await ConstruirToken(credencialesUsuario); //le pasamos las credenciales del usuario y creamos el token
            }
            else
            {
                return BadRequest(resultado.Errors); //Usuario no creado, error
            }
        }

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("RenovarToken", Name ="renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault(); //Email sera del tipo claim
            var email = emailClaim.Value; // el email sera el emailClaim
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };

            return await ConstruirToken(credencialesUsuario);
            //Construimos el token que tendra una nueva fecha de expiracion cada vez que se renueve
        }
            private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario) //metodo para construir token
            {
                var claims = new List<Claim>() //Creamos una lista de claims
                {
                    new Claim("email", credencialesUsuario.Email), //Creamos el claim email
                    new Claim("lo que yo quiera", "cualquier otro valor")
                };

                var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);//Encontramos usuario mediante email
                var claimsDB = await userManager.GetClaimsAsync(usuario); //Obtenemos todos los claims que existen en la BDD de usuario

                claims.AddRange(claimsDB);

                var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
                var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

                var expiracion = DateTime.UtcNow.AddYears(1);
            //Creamos la llave, expiracion, claim y creds

                var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                    expires: expiracion, signingCredentials: creds);
            //Creamos el securityToken donde le pasamos todos los elementos creados anteriormente

                return new RespuestaAutenticacion() //Devolvemos la respuesta
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                    Expiracion = expiracion //escribimos el token con todo lo que contiene securityToken y le damos expiracion de 1 anio en este caso
                };
            }
         
            [HttpPost("HacerAdmin", Name = "hacerAdmin")]
            public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email); //traemos y buscamos al usuario mediante email
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }
        [HttpPost("RemoverAdmin", Name ="removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email); //traemos y buscamos al usuario mediante email
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }
    }
}
