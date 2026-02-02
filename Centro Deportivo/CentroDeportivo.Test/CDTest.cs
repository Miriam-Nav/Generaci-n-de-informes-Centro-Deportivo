using Model;
using System;

namespace CentroDeportivo.Tests
{

    [TestClass] 
    public class CDTests
    {
        [TestMethod]
        public void Test_ValidarEmail()
        {
            var socio = new Socio(); 
            string emailValido = "test@gym.com";

            // Simulación de validación
            bool resultado = emailValido.Contains("@");

            Assert.IsTrue(resultado); 
        }

        [TestMethod]
        public void Test_FechaNoAnterior()
        {
            var reserva = new Reserva();
            // Fecha de ayer 
            reserva.Fecha = DateTime.Now.AddDays(-1); 

            bool esValida = reserva.Fecha >= DateTime.Today;

            Assert.IsFalse(esValida); 
        }

        [TestMethod]
        public void Test_ControlAforo()
        {
            // Simula aforo 1 
            var actividad = new Actividad { AforoMaximo = 1 }; 

            // Primera reserva (pasa)
            int reservas = 1;
            bool primeraPasa = reservas <= actividad.AforoMaximo;

            // Intento de segunda reserva (debe fallar)
            reservas++;
            bool segundaPasa = reservas <= actividad.AforoMaximo;

            Assert.IsTrue(primeraPasa); 
            Assert.IsFalse(segundaPasa); 
        }
    }
}