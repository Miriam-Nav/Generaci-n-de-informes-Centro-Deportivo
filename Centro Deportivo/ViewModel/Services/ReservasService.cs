using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para la gestión de reservas.
    /// Realiza validaciones de fecha, control de aforo disponible y evita duplicidades.
    /// </summary>
    public class ReservasService
    {
        private readonly ReservaRepositorio _repo = new ReservaRepositorio();

        /// <summary>
        /// Realiza las validaciones completas para una reserva (socio, actividad y fecha).
        /// </summary>
        /// <param name="reserva">Objeto reserva a validar.</param>
        /// <param name="actividad">Actividad asociada para validar aforo.</param>
        /// <exception cref="ArgumentException">Lanzada cuando algún dato no cumple las reglas.</exception>
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

        /// <summary>
        /// Comprueba si existe aforo suficiente en la actividad para la fecha seleccionada.
        /// </summary>
        /// <param name="actividad">Actividad a consultar.</param>
        /// <param name="fecha">Fecha de la reserva.</param>
        /// <param name="reservaId">ID de la reserva actual para excluirla en el conteo.</param>
        /// <exception cref="InvalidOperationException">Lanzada si se ha superado el límite de aforo.</exception>
        private void ValidarAforoDisponible(Actividad actividad, DateTime fecha, int reservaId)
        {
            int ocupadas = actividad.Reserva.Count(r => r.Fecha.Date == fecha.Date && r.Id != reservaId);

            if (ocupadas >= actividad.AforoMaximo)
            { 
                throw new InvalidOperationException($"Aforo máximo alcanzado ({actividad.AforoMaximo}) para esta actividad."); 
            }
        }

        /// <summary>
        /// Comprueba que el socio no tenga ya una reserva para esa misma actividad y fecha.
        /// </summary>
        /// <param name="reserva">Objeto reserva con los datos del socio y la actividad.</param>
        /// <exception cref="ArgumentException">Lanzada si ya existe una reserva igual.</exception>
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

        /// <summary>
        /// Registra una nueva reserva en el sistema tras validar sus datos.
        /// </summary>
        /// <param name="reserva">Instancia de la nueva reserva.</param>
        /// <param name="actividad">Actividad en la que se inscribe el socio.</param>
        public void CrearReserva(Reserva reserva, Actividad actividad)
        {
            ValidarReserva(reserva, actividad);
            _repo.Crear(reserva);
        }

        /// <summary>
        /// Actualiza la información de una reserva.
        /// </summary>
        /// <param name="reserva">Reserva con los datos actualizados.</param>
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

        /// <summary>
        /// Gestiona la eliminación de una reserva.
        /// </summary>
        /// <param name="reserva">Reserva a eliminar.</param>
        public void EliminarReserva(Reserva reserva)
        {
            _repo.Eliminar(reserva);
        }

    }
}