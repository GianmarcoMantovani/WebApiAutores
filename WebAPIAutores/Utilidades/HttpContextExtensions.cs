using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Utilidades
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable) 
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());
            //utilizamos el httpcontext para colocar en la cabecera de la respuesta el dato de la cantidad total de registros que hay
            //utilizamos CountAsync para contar los registros de la tabla que obtenemos aqui en forma de IQueryable
        }
    }
}
