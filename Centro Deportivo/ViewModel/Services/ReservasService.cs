using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Services
{
    public class ReservasService
    {
        private readonly ReservaRepositorio _repo = new ReservaRepositorio();

        public void ValidarReserva(Reserva reserva, Actividad actividad)
        {
            if (reserva.SocioId <= 0)
            { 
                throw new ArgumentException("Debe seleccionar un socio."); 
            }

            if (actividad == null)
            { 
                throw new ArgumentException("Debe seleccionar una actividad."); 
            }
            if (reserva.Fecha.Date < DateTime.Today)
            {
                throw new ArgumentException("No se permiten reservas con fechas anteriores a la actual.");
            }

            ValidarAforoDisponible(actividad, reserva.Fecha, reserva.Id);
            ValidarReservaDuplicada(reserva, actividad);
        }

        private void ValidarAforoDisponible(Actividad actividad, DateTime fecha, int reservaId)
        {
            int ocupadas = actividad.Reserva.Count(r => r.Fecha.Date == fecha.Date && r.Id != reservaId);

            if (ocupadas >= actividad.AforoMaximo)
            { 
                throw new InvalidOperationException($"Aforo máximo alcanzado ({actividad.AforoMaximo}) para esta actividad."); 
            }
        }

        private void ValidarReservaDuplicada(Reserva reserva, Actividad actividad)
        {
            var todas = _repo.Seleccionar();

            bool tieneReserva = todas.Any(r =>
                r.SocioId == reserva.SocioId &&
                r.ActividadId == reserva.ActividadId &&
                r.Fecha.Date == reserva.Fecha.Date &&
                r.Id != reserva.Id); 

            if (tieneReserva)
            {
                throw new ArgumentException("Este socio ya tiene una reserva para esta actividad en la fecha seleccionada.");
            }
        }


        public void CrearReserva(Reserva reserva, Actividad actividad)
        {
            ValidarReserva(reserva, actividad);
            _repo.Crear(reserva);
        }

        public void EliminarReserva(Reserva reserva)
        {
            _repo.Eliminar(reserva);
        }

        public void ActualizarReserva(Reserva reservaEditada)
        {
            var todas = _repo.Seleccionar();

            bool yaExiste = todas.Any(r =>
                r.SocioId == reservaEditada.SocioId &&
                r.ActividadId == reservaEditada.ActividadId &&
                r.Fecha.Date == reservaEditada.Fecha.Date &&
                r.Id != reservaEditada.Id); 

            if (yaExiste)
            {
                throw new ArgumentException("Error: Este socio ya tiene una reserva para esta actividad en la fecha seleccionada.");
            }
            _repo.Guardar(reservaEditada);
        }


    }
}