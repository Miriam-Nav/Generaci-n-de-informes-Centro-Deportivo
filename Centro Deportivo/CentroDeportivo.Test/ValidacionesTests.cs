using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModel.Services; 
using Model;             

namespace CentroDeportivo.Tests
{
    [TestClass]
    public class SocioTests
    {
        [TestMethod]
        public void ValidarEmail_FormatoCorrecto_RetornaTrue()
        {

            var service = new SocioService();
            string emailValido = "alumno@estacio.com";


            bool resultado = service.ValidarEmailFormato(emailValido);

            Assert.IsTrue(resultado, "El email debería ser válido");
        }

        [TestMethod]
        public void ValidarEmail_SinArroba_RetornaFalse()
        {

            var service = new SocioService();
            string emailInvalido = "alumno.com";


            bool resultado = service.ValidarEmailFormato(emailInvalido);

      
            Assert.IsFalse(resultado, "El email no debería ser válido sin el @");
        }
    }
}
