using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;
using WebAPIAutores.Utilidades;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentariosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "obtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId, [FromQuery]PaginacionDTO paginacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }
            var queryable = context.Comentarios.Where(comentarioDB => comentarioDB.LibroId == libroId).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var comentarios = await queryable.OrderBy(comentario => comentario.Id)
                .Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost(Name = "crearComentario" )]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim=> claim.Type == "email").FirstOrDefault(); //Email sera del tipo claim
            var email = emailClaim.Value; // el email sera el emailClaim

            var usuario = await userManager.FindByEmailAsync(email); //Devuelve un identityUser donde buscamos por email el ID del usuario
            var usuarioId = usuario.Id; //ASignamos el ID a la variable usuarioId

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);//Verificamos q exista un comentario

            if (!existeLibro)
            {
                return NotFound(); 
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO); //mappeamos el comentario
            comentario.LibroId = libroId; //asignamos el id del libro al comentario
            comentario.UsuarioId = usuarioId; //asignamos el id del usuario al comentario
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
            //Necesitamos el ID y el libroID ya que esta en la ruta principal del controlador
        }

        [HttpPut("{id:int}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId); //Verificamos que exista el libro

            if (!existeLibro)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id); //Verificamos que exista el comentario

            if (!existeComentario)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO); //mappeamos
            comentario.Id = id;//asignamos el id
            comentario.LibroId = libroId; //asignamos el id del libro
            context.Update(comentario); //Actualizamos el comentario
            await context.SaveChangesAsync(); //Guardamos en la BDD
            return NoContent();
        }
    }
}
