using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoHorarios.Models;
using ProyectoHorarios.Models.ProyectoApi.Models;
using ProyectoHorarios.Services;
using System.Diagnostics.Tracing;

namespace ProyectoHorarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorarioController : ControllerBase
    {
        private readonly HorarioService _horarioService;

        public HorarioController(HorarioService horarioService)
        {
            _horarioService = horarioService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateHorario(HorarioDto horariodto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState + "PRIMER FILTRO");
            }


            if (!DateTime.TryParseExact(horariodto.Dia.ToString(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                ModelState.AddModelError("dia", "El formato de fecha es incorrecto. Debe ser yyyy-MM-dd.");
                return BadRequest(ModelState);
            }

            var horariosExistentes = await _horarioService.GetHorariosAsync(horariodto.IdMedico, DateTime.ParseExact(horariodto.Dia, "yyyy-MM-dd", null));

            if (horariosExistentes.Any(h => h.Hora == horariodto.Hora))
            {
                ModelState.AddModelError("horario", "Error al registrar el horario. El médico ya tiene un horario registrado para esa fecha y hora.");
                return BadRequest(ModelState);
            }

            var horario = new Horario
            {
                IdMedico = horariodto.IdMedico,
                Dia = DateTime.ParseExact(horariodto.Dia, "yyyy-MM-dd", null),
                Hora = horariodto.Hora,
            };

            await _horarioService.CreateHorarioAsync(horario);
            return Ok(horario);
        }

        [HttpGet("{idHorario}")]
        public async Task<IActionResult> GetHorario(string idHorario)
        {
            var horario = await _horarioService.GetHorarioIDAsync(idHorario);
            if (horario == null)
            {
                return NotFound();
            }

            return Ok(horario);
        }

        [HttpPut("{idHorario}")]
        public async Task<IActionResult> UpdateHorario(string idHorario, HorarioDto horarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var horarioExistente = await _horarioService.GetHorarioIDAsync(idHorario);
            if (horarioExistente == null)
            {
                return NotFound();
            }

            horarioExistente.IdMedico = horarioDto.IdMedico;
            horarioExistente.Dia = DateTime.ParseExact(horarioDto.Dia, "yyyy-MM-dd", null);
            horarioExistente.Hora = horarioDto.Hora;

            await _horarioService.UpdateHorarioAsync(horarioExistente);

            return Ok(horarioExistente);
        }

        [HttpDelete("{idHorario}")]
        public async Task<IActionResult> DeleteHorario(string idHorario)
        {
            var horarioExistente = await _horarioService.GetHorarioIDAsync(idHorario);
            if (horarioExistente == null)
            {
                return NotFound();
            }

            await _horarioService.DeleteHorarioAsync(horarioExistente);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHorario()
        {
            var horarios = await _horarioService.GetHorariosAsync();
            return Ok(horarios);
        }

    }
}
