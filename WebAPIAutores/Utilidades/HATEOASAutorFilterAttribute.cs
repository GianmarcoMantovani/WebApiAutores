using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Utilidades
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute //Hereda de la clase HATEOASFiltro
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces) //Inyectamos nuestro metodo de la clase GeneradorEnlaces
        {
            this.generadorEnlaces = generadorEnlaces;
        }

        public async Task OnResultExecutionAsync (ResultExecutingContext context,
            ResourceExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context); //Le asignamos a una variable que debe incluir HATEOAS

            if (!debeIncluir) //Si no lo debe incluir seguimos avanzando
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult; //Tomamos el valor de object result

            /* var modelo = resultado.Value as AutorDTO ?? throw new
                 ArgumentNullException("Se esperaba una instancia de AutorDTO"); //Tomamos el valor del AutorDTO y si es nulo devolvemos error*/
            var autorDTO = resultado.Value as AutorDTO;
            if(autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ?? throw new
                    ArgumentException("Se esperaba una instancia de AutorDTO o List<AutorDTO>");

                autoresDTO.ForEach(async autor => await generadorEnlaces.GenerarEnlaces(autor)); 
                resultado.Value = autoresDTO;
                //Si es nulo devolvemos que se esperaba una instancia y le damos a resultado los enlaces de autorDTO
            }
            else
            {
                await generadorEnlaces.GenerarEnlaces(autorDTO); //Generamos los enlaces del AutorDTO
            }
             

             await next(); //Le indicamos al siguiente filtro que siga

        }
    }
}
