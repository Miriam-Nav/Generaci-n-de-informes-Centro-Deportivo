using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Repositorios
{
    public class SocioRepositorio
    {
        private readonly CentroDeportivoEntities _db = new CentroDeportivoEntities();

        // OBTENER LISTA DE SOCIOS
        public List<Socio> Seleccionar()
        {
            return _db.Socio.ToList();
        }

        // CREAR SOCIO
        public void Crear(Socio socio)
        {
            _db.Socio.Add(socio);
            _db.SaveChanges();
        }

        // GUARDAR SOCIO
        public void Guardar(Socio socioEditado)
        {
            using (var db = new CentroDeportivoEntities())
            {
                var original = db.Socio.Find(socioEditado.Id);

                if (original != null)
                {
                    original.Nombre = socioEditado.Nombre;
                    original.Email = socioEditado.Email;
                    original.Activo = socioEditado.Activo;

                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("No se pudo encontrar el socio para actualizar.");
                }
            }
        }

        // ELIMINAR SOCIO
        public void Eliminar(Socio socio)
        {
            // Busca el socio real en DB usando su ID
            var socioEnDb = _db.Socio.Find(socio.Id);

            if (socioEnDb != null)
            {
                _db.Socio.Remove(socioEnDb);
                _db.SaveChanges();
            }
        }
    }
}