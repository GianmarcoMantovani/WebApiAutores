using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        { 
            var datosHateoas = new List<DatoHATEOAS>(); //Obtenemos el listado de datoHATEOAS

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerRoot", new { }),
                descripcion: "self", metodo: "GET"));
            //Agregamos un nuevo dato HATEOAS y a traves del constructor pasamos los datos (enlace, descripcion y metodo)

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }),
                descripcion: "autores", metodo: "GET"));

            if (esAdmin.Succeeded)
            {

                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }),
                    descripcion: "autor-crear", metodo: "POST"));

                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }),
                    descripcion: "libro-crear", metodo: "POST"));
            }

            return datosHateoas; //Devolvemos los datosHATEOAS
        }
    }
}
