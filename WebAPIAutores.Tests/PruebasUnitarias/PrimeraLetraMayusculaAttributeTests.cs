using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass] //La clase de 
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {

            // Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute(); //Tomamos el valor de la clase a utilizar
            var valor = "felipe"; //Tomamos el valor del nombre a probar
            var valContext = new ValidationContext(new {Nombre = valor});
            //Instanciamos el ValidationContext, le pasamos el valor nombre y lo guardamos en una variable

            // Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            // Guardamos en la variable resultado el resultado de la validacion entre el nombre y el ValidationContext
            
            // Verificacion
            Assert.AreEqual("La primera letra debe ser mayúscula", resultado.ErrorMessage);
            //Lo primero es el mensaje que esperamos obtener y lo segundo el mensaje de error obtenido
        }
        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {

            // Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute(); //Tomamos el valor de la clase a utilizar
            string valor = null; //Tomamos el valor del nombre a probar
            var valContext = new ValidationContext(new { Nombre = valor });
            //Instanciamos el ValidationContext, le pasamos el valor nombre y lo guardamos en una variable

            // Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            // Guardamos en la variable resultado el resultado de la validacion entre el nombre y el ValidationContext

            // Verificacion
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {

            // Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute(); //Tomamos el valor de la clase a utilizar
            string valor = "Felipe"; //Tomamos el valor del nombre a probar
            var valContext = new ValidationContext(new { Nombre = valor });
            //Instanciamos el ValidationContext, le pasamos el valor nombre y lo guardamos en una variable

            // Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            // Guardamos en la variable resultado el resultado de la validacion entre el nombre y el ValidationContext

            // Verificacion
            Assert.IsNull(resultado);
        }
    }
}