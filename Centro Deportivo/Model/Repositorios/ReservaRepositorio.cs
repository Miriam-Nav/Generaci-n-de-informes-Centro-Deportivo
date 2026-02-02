using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Model.Repositorios
{
    public class ReservaRepositorio
    {
        // Instancia de la base de datos 
        private readonly CentroDeportivoEntities _db = new CentroDeportivoEntities();

        // Obtener todas las reservas
        public List<Reserva> Seleccionar()
        {
            return _db.Reserva
                .Include(r => r.Socio)
                .Include(r => r.Actividad)
                .ToList();
        }

        // Crear una nueva reserva
        public void Crear(Reserva reserva)
        {
            _db.Reserva.Add(reserva);
            _db.SaveChanges();
        }

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
    }
}