using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace WebAPIAutores.Utilidades
{
    public class CabeceraEstaPresenteAttribute : Attribute, IActionConstraint
    {
        private readonly string cabecera;
        private readonly string valor;

        public CabeceraEstaPresenteAttribute(string cabecera, string valor)
        {
            this.cabecera = cabecera;
            this.valor = valor;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var cabeceras = context.RouteContext.HttpContext.Request.Headers; //tomamos las cabeceras

            if (!cabeceras.ContainsKey(cabecera)) // si en las cabeceras no esta la cabecera que indicamos arriba retornamos falso
            {
                return false;
            }

            return string.Equals(cabeceras[cabecera], valor, StringComparison.OrdinalIgnoreCase);
            //si esta retornamos la cabecera que es igual al valor e ignoramos si es mayuscula o minuscula

            //Esto determina que si la peticion http tiene la cabecera y el valor indicado
            //hay que utilizar el ENDPOINT donde se encuentra este atributo
        }
    }
}
