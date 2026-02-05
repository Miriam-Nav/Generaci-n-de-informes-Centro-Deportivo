using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio para gestionar las operaciones de la entidad Reserva.
    /// Permite realizar operaciones de leer, crear, actualizar y eliminar registros en la base de datos./>.
    /// </summary>
    public class ReservaRepositorio
    {
        // Instancia de la base de datos 
        private readonly CentroDeportivoEntities _db = new CentroDeportivoEntities();

        /// <summary>
        /// Obtiene la lista completa de reservas almacenadas.
        /// </summary>
        /// <returns>Una lista de objetos de tipo <see cref="Reserva"/>.</returns>
        // Obtener todas las reservas
        public List<Reserva> Seleccionar()
        {
            return _db.Reserva
                .Include(r => r.Socio)
                .Include(r => r.Actividad)
                .ToList();
        }

        /// <summary>
        /// Crea una nueva reserva en la base de datos.
        /// </summary>
        /// <param name="reserva">La entidad reserva que contiene los IDs de socio, actividad y la fecha.</param>
        // Crear una nueva reserva
        public void Crear(Reserva reserva)
        {
            _db.Reserva.Add(reserva);
            _db.SaveChanges();
        }

        /// <summary>
        /// Actualiza los datos de una reserva de la base de datos.
        /// </summary>
        /// <param name="reservaEditada">Objeto reserva con los datos actualizados.</param>
        /// <exception cref="Exception">Lanza una excepción si la reserva no existe en la base de datos.</exception>
        // Guardar cambios
        public void Guardar(Reserva reservaEditada)
        {
            using (var db = new CentroDeportivoEntities())
            {
                var original = db.Reserva.Find(reservaEditada.Id);

                if (original != null)
                {
                    original.SocioId = reservaEditada.SocioId;
                    original.ActividadId = reservaEditada.ActividadId;
                    original.Fecha = reservaEditada.Fecha;

                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("No se encontró la reserva original en la base de datos.");
                }
            }
        }

        /// <summary>
        /// Elimina una reserva de la base de datos.
        /// </summary>
        /// <param name="reserva">La reserva que se desea eliminar (se utiliza su Id para la búsqueda).</param>
        // Eliminar una reserva
        public void Eliminar(Reserva reserva)
        {
            var entidad = _db.Reserva.Find(reserva.Id);
            if (entidad != null)
            {
                _db.Reserva.Remove(entidad);
                _db.SaveChanges();
            }
        }

    }
}