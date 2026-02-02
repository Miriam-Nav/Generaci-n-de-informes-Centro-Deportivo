using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Services
{
    public class SocioService
    {
        private readonly SocioRepositorio _repo;

        public SocioService()
        {
            _repo = new SocioRepositorio();
        }

        // VALIDAR EMAIL
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
        public void CrearSocio(Socio socio)
        {
            ValidarSocio(socio, true);
            _repo.Crear(socio);
        }

        // ACTUALIZAR
        public void ActualizarSocio(Socio socio)
        {
            ValidarSocio(socio, false);
            _repo.Guardar(socio);
        }

        // ELIMINAR
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