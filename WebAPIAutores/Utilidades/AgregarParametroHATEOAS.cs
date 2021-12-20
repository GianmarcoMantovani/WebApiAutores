using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace WebAPIAutores.Utilidades
{
    public class AgregarParametroHATEOAS : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if(context.ApiDescription.HttpMethod != "GET") //Con esto vemos a que metodo responde
            {
                return; //Si no responde a un GET vamos a filtrarlo y no vamos a hacer nada de lo que sigue
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>(); //Si es nulo lo inicializamos
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "incluirHATEOAS",
                In = ParameterLocation.Header, //Indicamos donde se encuentra (la cabecera)
                Required = false //False porque no es requerido
            });
        }
    }
}
