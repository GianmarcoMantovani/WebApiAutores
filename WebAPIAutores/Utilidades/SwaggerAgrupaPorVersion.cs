using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace WebAPIAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention //Implementamos la interfaz
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace; //Nos da el namespace del controlador. EJ: Controllers.V1
            var versionAPI = namespaceControlador.Split('.').Last().ToLower(); // Esto nos da v1 por ej y verificamos la version de la api
            controller.ApiExplorer.GroupName = versionAPI;
            //esto es una convencion que agrupa nuestros controladroes segun la version que este en el namespace
        }
    }
}
