using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPIAutores.Controllers.V1;
using WebAPIAutores.Tests.Mocks;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtenemos4Links()
        {
            //Preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success(); //asignamos el success al mock ya que lo pasamos a mock general
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock(); //Traemos el AuthorizationService y el UrlHelper
            //Ejecucion

            var resultado = await rootController.Get(); //Asignamos el resultado de GET de rootController


            //Verificacion
            Assert.AreEqual(4, resultado.Value.Count());
            //Obtenemos el IEnumarble de datoHATEOAS y los contamos (se supone que son 4)
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2Links()
        {
            //Preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed(); //Asignamos al mock que retorne Failed
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock(); //Traemos el AuthorizationService y el UrlHelper
            //Ejecucion

            var resultado = await rootController.Get(); //Asignamos el resultado de GET de rootController


            //Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
            
        }
        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2Links_UsandoMoq()
        {
            //Preparacion
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURLHelper = new Mock<IUrlHelper>();
            mockURLHelper.Setup(x =>
            x.Link(It.IsAny<string>(),
            It.IsAny<object>()))
                .Returns(String.Empty);

            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockURLHelper.Object; //Traemos el AuthorizationService y el UrlHelper
            //Ejecucion

            var resultado = await rootController.Get(); //Asignamos el resultado de GET de rootController


            //Verificacion
            Assert.AreEqual(2, resultado.Value.Count());

        }

    }
}
