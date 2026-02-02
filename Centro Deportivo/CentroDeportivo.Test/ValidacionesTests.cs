using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModel.Services; // Donde está tu SocioService
using Model;             // Donde están tus clases de datos

namespace CentroDeportivo.Tests
{
    [TestClass]
    public class SocioTests
    {
        [TestMethod]
        public void ValidarEmail_FormatoCorrecto_RetornaTrue()
        {
            // 1. Arrange (Preparar)
            var service = new SocioService();
            string emailValido = "alumno@estacio.com";

            // 2. Act (Ejecutar)
            bool resultado = service.ValidarEmailFormato(emailValido);

            // 3. Assert (Verificar)
            Assert.IsTrue(resultado, "El email debería ser válido");
        }

        [TestMethod]
        public void ValidarEmail_SinArroba_RetornaFalse()
        {
            // Arrange
            var service = new SocioService();
            string emailInvalido = "alumno.com";

            // Act
            bool resultado = service.ValidarEmailFormato(emailInvalido);

            // Assert
            Assert.IsFalse(resultado, "El email no debería ser válido sin el @");
        }
    }
}