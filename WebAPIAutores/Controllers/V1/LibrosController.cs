using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name ="crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await context.Autores
                .Where(autorBD => libroCreacionDTO.AutoresIds.Contains(autorBD.Id)).Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros //Traemos el libro de la BDD
                .Include(x => x.AutoresLibros) //incluimos el listado de autoreslibros
                .FirstOrDefaultAsync(x => x.Id == id); //Donde el id sea igual al id que tenemos aca

            if (libroDB == null) // Si el libro no existe retornamos not found
            {
                return NotFound();
            }

            libroDB = mapper.Map(libroCreacionDTO, libroDB);
            //mappeamos para llevar las propiedades del DTO hacia libroDB, asi actualizamos este ultimo

            AsignarOrdenAutores(libroDB); //Creamos una funcion para asignar el orden a los autores del libro

            await context.SaveChangesAsync(); //Guardamos los cambios en la BDD
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null) //Verificamos que existan libros
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i; //Asignamos un orden a los autores del libro
                }
            }

        }

        [HttpPatch("{id:int}", Name ="patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null) //Verificamos que no sea nulo para evitar error de formato
            {
                return BadRequest();
            }

            var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);  //Traemos el libro de la BDD

            if (libroDB == null) //Si el libro es nulo devolvemos error
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDB); //mappeamos el libroDB al libroPatchDTO
            //estamos llenando el libropatchDTO con las informaciones del libro de la BDD

            patchDocument.ApplyTo(libroDTO, ModelState); //aplicamos al librodto los cambios del patchdocument y si hay error se aplica al modelstate

            var esValido = TryValidateModel(libroDTO); //creamos la variable esvalido

            if (!esValido) //Verificamos que sea valido
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDB); //Mappeamos

            await context.SaveChangesAsync(); //Aplicamos cambios a la BDD
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
