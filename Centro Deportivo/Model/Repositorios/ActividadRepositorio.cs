using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio para gestionar las operaciones de la entidad Actividad.
    /// Permite realizar operaciones de leer, crear, actualizar y eliminar registros en la base de datos.
    /// </summary>
    public class ActividadRepositorio
    {
        private readonly CentroDeportivoEntities _db = new CentroDeportivoEntities();

        /// <summary>
        /// Obtiene la lista de actividades deportivas registradas.
        /// </summary>
        /// <returns>Una lista de objetos de tipo <see cref="Actividad"/>.</returns>
        public List<Actividad> Seleccionar()
        {
            return _db.Actividad.ToList();
        }

        /// <summary>
        /// Crea una nueva actividad en la base de datos.
        /// </summary>
        /// <param name="actividad">Objeto Actividad con los datos a insertar.</param>
        public void Crear(Actividad actividad)
        {
            _db.Actividad.Add(actividad);
            _db.SaveChanges();
        }

        /// <summary>
        /// Actualiza los datos de una actividad de la base de datos.
        /// </summary>
        /// <param name="actividadEditada">Objeto actividad con los datos actualizados.</param>
        public void Guardar(Actividad actividadEditada)
        {
            using (var db = new CentroDeportivoEntities())
            {
                // Busca su ID
                var original = db.Actividad.Find(actividadEditada.Id);

                if (original != null)
                {

                    original.Nombre = actividadEditada.Nombre;
                    original.AforoMaximo = actividadEditada.AforoMaximo;

                    db.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Elimina una actividad de la base de datos siempre que no tenga reservas asociadas.
        /// </summary>
        /// <param name="actividad">La actividad que se desea eliminar.</param>
        /// <exception cref="InvalidOperationException">
        /// Se lanza cuando la actividad tiene al menos una reserva vinculada queimpide que se borre.
        /// </exception>
        public void Eliminar(Actividad actividad)
        {
            var encontrada = _db.Actividad.Find(actividad.Id);
            if (encontrada != null)
            {
                // Verificamos si tiene reservas antes de borrar
                if (encontrada.Reserva.Any())
                {
                    throw new InvalidOperationException("No se puede eliminar una actividad con reservas.");
                }

                _db.Actividad.Remove(encontrada);
                _db.SaveChanges();
            }
        }

        
    }
}