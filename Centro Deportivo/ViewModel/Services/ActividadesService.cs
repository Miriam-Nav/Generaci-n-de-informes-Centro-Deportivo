using Model;
using Model.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio encargado de la lógica para la gestión de actividades.
    /// Realiza validaciones de nombres obligatorios, aforos positivos y control de duplicados.
    /// </summary>
    public class ActividadesService
    {
        private readonly ActividadRepositorio _repo = new ActividadRepositorio();

        /// <summary>
        /// Recupera la lista completa de actividades desde el repositorio.
        /// </summary>
        /// <returns>Una lista de objetos Actividad.</returns>
        public List<Actividad> ObtenerTodas()
        {
            return _repo.Seleccionar();
        }

        /// <summary>
        /// Realiza las validaciones completas para una actividad (nombre y aforo).
        /// </summary>
        /// <param name="actividad">Objeto actividad a validar.</param>
        /// <param name="esNueva">Define si es una creación (true) o una modificación (false).</param>
        /// <exception cref="ArgumentException">Lanzada cuando algún dato no cumple las reglas.</exception>
        public void ValidarActividad(Actividad actividad, bool esNueva)
        {
            ValidarNombreObligatorio(actividad.Nombre);
            ValidarAforoPositivo(actividad.AforoMaximo);
            ValidarNombreUnico(actividad, esNueva);
        }

        /// <summary>
        /// Valida que el nombre de la actividad no sea nulo ni espacios en blanco.
        /// </summary>
        /// <param name="nombre">Cadena de texto con el nombre a validar.</param>
        /// <exception cref="ArgumentException">Lanzada si el nombre está vacío.</exception>
        // Validar que el nombre no sea nulo o vacío
        private void ValidarNombreObligatorio(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre de la actividad es obligatorio.");
            }
        }

        /// <summary>
        /// Valida que el aforo máximo sea un número entero positivo.
        /// </summary>
        /// <param name="aforo">Valor numérico del aforo.</param>
        /// <exception cref="ArgumentException">Lanzada si el aforo es menor o igual a cero.</exception>
        // Validar que el aforo sea mayor a 0
        private void ValidarAforoPositivo(int aforo)
        {
            if (aforo <= 0)
            {
                throw new ArgumentException("El aforo debe ser mayor que 0.");
            }
        }

        /// <summary>
        /// Comprueba que no exista otra actividad con el mismo nombre.
        /// </summary>
        /// <param name="actividad">Objeto actividad para comparar el nombre e ID.</param>
        /// <param name="esNueva">Define si se debe ignorar el ID actual durante la búsqueda.</param>
        /// <exception cref="ArgumentException">Lanzada si el nombre ya está registrado.</exception>
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

        /// <summary>
        /// Registra una nueva actividad en el sistema tras validar sus datos.
        /// </summary>
        /// <param name="actividad">Instancia de la nueva actividad.</param>
        public void CrearActividad(Actividad actividad)
        {
            ValidarActividad(actividad, true);
            _repo.Crear(actividad);
        }

        /// <summary>
        /// Gestiona la eliminación de una actividad.
        /// </summary>
        /// <param name="actividad">Actividad a eliminar.</param>
        public void EliminarActividad(Actividad actividad)
        {
            _repo.Eliminar(actividad);
        }

        /// <summary>
        /// Actualiza la información de una actividad.
        /// </summary>
        /// <param name="actividad">Actividad con los datos actualizados.</param>
        public void ActualizarActividad(Actividad actividad)
        {
            ValidarActividad(actividad, false);
            _repo.Guardar(actividad);
        }
    }
}