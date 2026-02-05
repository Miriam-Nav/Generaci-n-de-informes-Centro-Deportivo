using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio para gestionar las operaciones de la entidad Socio.
    /// Permite realizar operaciones de leer, crear, actualizar y eliminar registros en la base de datos.
    /// </summary>
    public class SocioRepositorio
    {
        private readonly CentroDeportivoEntities _db = new CentroDeportivoEntities();

        /// <summary>
        /// Obtiene la lista completa de socios almacenados.
        /// </summary>
        /// <returns>Una lista de objetos de tipo <see cref="Socio"/>.</returns>
        public List<Socio> Seleccionar()
        {
            return _db.Socio.ToList();
        }

        /// <summary>
        /// Crea un nuevo socio en la base de datos.
        /// </summary>
        /// <param name="socio">Objeto Socio con los datos a insertar.</param>
        public void Crear(Socio socio)
        {
            _db.Socio.Add(socio);
            _db.SaveChanges();
        }

        /// <summary>
        /// Actualiza los datos de un socio de la base de datos.
        /// </summary>
        /// <param name="socioEditado">Objeto socio con los datos actualizados.</param>
        /// <exception cref="Exception">Lanza una excepción si el socio no existe en la base de datos.</exception>
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

        /// <summary>
        /// Elimina un socio de la base de datos.
        /// </summary>
        /// <param name="socio">>El socio que se desea eliminar (se utiliza su Id para la búsqueda).</param>
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