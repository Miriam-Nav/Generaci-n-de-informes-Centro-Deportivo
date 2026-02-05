using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para la gestión de socios.
    /// Realiza validaciones de formato, comprobaciones de duplicidad de emails e integridad referencial.
    /// </summary>
    public class SocioService
    {
        private readonly SocioRepositorio _repo = new SocioRepositorio();

        // VALIDAR EMAIL
        /// <summary>
        /// Comprueba si la cadena de texto proporcionada cumple con los requisitos mínimos de un correo electrónico.
        /// </summary>
        /// <param name="email">Cadena de texto con el email a validar.</param>
        /// <returns>Verdadero si el formato es aceptable, falso en caso contrario.</returns>
        public bool ValidarEmailFormato(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) { 
                return false; 
            }

            // Comprueaba que tenga un @
            if (email.Contains("@"))
            {
                return true;
            }
            
            return false;
        }

        // VALIDAR SOCIO
        /// <summary>
        /// Realiza las validaciones de negocio completas para un socio (nombre, formato de email y unicidad).
        /// </summary>
        /// <param name="socio">Objeto socio a validar.</param>
        /// <param name="esNuevo">Define si es una creación (true) o una modificación (false) para el control de duplicados.</param>
        /// <exception cref="ArgumentException">Lanzada cuando algún dato no cumple las reglas.</exception>
        public void ValidarSocio(Socio socio, bool esNuevo)
        {
            if (string.IsNullOrWhiteSpace(socio.Nombre))
            { 
                throw new ArgumentException("El nombre no puede estar vacío"); 
            }

            if (!ValidarEmailFormato(socio.Email))
            { 
                throw new ArgumentException("El email no es válido"); 
            }

            var lista = _repo.Seleccionar();

            bool existe;

            if (esNuevo)
            {
                // Comprueba que ningun usuario tenga ese email
                existe = lista.Any(s => s.Email == socio.Email);
            }
            else {
                // Comprueba que ningun usuario a parte de ese tenga ese email
                existe = lista.Any(s => s.Email == socio.Email && s.Id != socio.Id);
            }

            if (existe)
            {
                throw new ArgumentException("El email ya está en uso");
            }
        }

        // CREAR
        /// <summary>
        /// Registra un nuevo socio en el sistema tras validar sus datos.
        /// </summary>
        /// <param name="socio">Instancia del nuevo socio.</param>
        public void CrearSocio(Socio socio)
        {
            ValidarSocio(socio, true);
            _repo.Crear(socio);
        }

        // ACTUALIZAR
        /// <summary>
        /// Actualiza la información de un socio existente previa validación.
        /// </summary>
        /// <param name="socio">Socio con los datos actualizados.</param>
        public void ActualizarSocio(Socio socio)
        {
            ValidarSocio(socio, false);
            _repo.Guardar(socio);
        }

        // ELIMINAR
        /// <summary>
        /// Gestiona la eliminación de un socio verificando que no existan reservas activas.
        /// </summary>
        /// <param name="socio">Socio a eliminar.</param>
        /// <exception cref="InvalidOperationException">Lanzada si el socio tiene registros de reserva vinculados.</exception>
        public void EliminarSocio(Socio socio)
        {
            // Comprueba si tiene reservas activas
            if (socio.Reserva != null && socio.Reserva.Any())
            {
                throw new InvalidOperationException("No se puede eliminar un socio con reservas activas.");
            }
            _repo.Eliminar(socio);
        }
    }
}