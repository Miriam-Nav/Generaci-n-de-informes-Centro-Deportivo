using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Services
{
    public class ActividadesService
    {
        private readonly ActividadRepositorio _repo = new ActividadRepositorio();

        public List<Actividad> ObtenerTodas()
        {
            return _repo.Seleccionar();
        }

        public void ValidarActividad(Actividad actividad, bool esNueva)
        {
            ValidarNombreObligatorio(actividad.Nombre);
            ValidarAforoPositivo(actividad.AforoMaximo);
            ValidarNombreUnico(actividad, esNueva);
        }

        // Validar que el nombre no sea nulo o vacío
        private void ValidarNombreObligatorio(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre de la actividad es obligatorio.");
            }
        }

        // Validar que el aforo sea mayor a 0
        private void ValidarAforoPositivo(int aforo)
        {
            if (aforo <= 0)
            {
                throw new ArgumentException("El aforo debe ser mayor que 0.");
            }
        }

        // Validar duplicados
        private void ValidarNombreUnico(Actividad actividad, bool esNueva)
        {
            var todas = _repo.Seleccionar();

            bool nombreEnUso;

            if (esNueva)
            {
                nombreEnUso = todas.Any(a => a.Nombre.Equals(actividad.Nombre, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                nombreEnUso = todas.Any(a => a.Nombre.Equals(actividad.Nombre, StringComparison.OrdinalIgnoreCase) && a.Id != actividad.Id);
            }

            if (nombreEnUso)
            {
                throw new ArgumentException("Ya existe una actividad con ese nombre.");
            }
        }

        public void CrearActividad(Actividad actividad)
        {
            ValidarActividad(actividad, true);
            _repo.Crear(actividad);
        }

        public void EliminarActividad(Actividad actividad)
        {
            _repo.Eliminar(actividad);
        }

        public void ActualizarActividad(Actividad actividad)
        {
            ValidarActividad(actividad, false);
            _repo.Guardar(actividad);
        }
    }
}