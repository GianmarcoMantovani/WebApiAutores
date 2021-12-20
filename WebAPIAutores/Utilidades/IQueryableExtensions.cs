using System.Linq;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T> (this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina)
                .Take(paginacionDTO.RecordsPorPagina);
                //Si el usuario se encuentra en la pagina 1 esto significa que el resultado da 0 entonces no nos saltamos ningun registro
              //si el usuario esta en la pagina 2 y son 10 registros por pagina el resultado es 10 entonces se saltan los 10 primeros registros
                // y se muestran los siguientes 10
        }
    }
}
