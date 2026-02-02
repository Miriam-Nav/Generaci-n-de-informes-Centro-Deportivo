using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Repositorios
{
    public class ActividadRepositorio
    {
        private readonly CentroDeportivoEntities _db = new CentroDeportivoEntities();

        public List<Actividad> Seleccionar()
        {
            return _db.Actividad.ToList();
        }

        public void Crear(Actividad actividad)
        {
            _db.Actividad.Add(actividad);
            _db.SaveChanges();
        }

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
    }
}