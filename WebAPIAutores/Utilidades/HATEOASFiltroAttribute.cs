using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Utilidades
{
    public class HATEOASFiltroAttribute : ResultFilterAttribute //Hereda de la clase base
    {
        protected bool DebeIncluirHATEOAS(ResultExecutingContext context)
        {
            var resultado = context.Result as ObjectResult;

            if (!EsRespuestaExitosa(resultado)) //Si la respuesta no es exitosa retornamos falso
            {
                return false;
            }

            var cabecera = context.HttpContext.Request.Headers["incluirHATEOAS"]; //Pedimos la cabecera incluirHATEOAS

            if (cabecera.Count == 0) //Si no hay cabeceras con el nombre incluirHATEOAS devolvemos falso
            {
                return false;
            }

            var valor = cabecera[0];

            if(!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) //Si la cabecera no es igual a Y de Yes retornamos false
            {
                return false;
            }

            return true; //Si es exitosa retornamos TRUE
        }

        private bool EsRespuestaExitosa(ObjectResult resultado)
        {
            if (resultado == null || resultado.Value == null) //Si el resultado es nulo o no valido retornamos falso
            {
                return false;
            }
            if(resultado.StatusCode.HasValue && !resultado.StatusCode.Value.ToString().StartsWith("2"))
            //Si el StatusCode no empieza con 2 quiere decir que no fue exitoso, por lo tanto retornamos falso
            {
                return false;
            }
            return true; //Si es exitosa la respuesta retornamos true
        }
    }
}
