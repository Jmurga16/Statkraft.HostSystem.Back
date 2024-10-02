using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.Archivo;
using Stakraft.HostSystem.Support.SoporteDto;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivoController : Controller
    {
        private readonly IArchivoService _archivoService;
        public ArchivoController(IArchivoService archivoService)
        {
            _archivoService = archivoService;
        }

        [HttpPost("enviar"), DisableRequestSizeLimit]
        public async Task<Respuesta<List<CargaArchivoOut>>> GuardarAchivoOriginal(List<IFormFile> listFile, [FromForm] string data)
        {
            var respuesta = new Respuesta<List<CargaArchivoOut>>();
            var dato = await _archivoService.GuardarArchivoOriginal(listFile, data);
            return respuesta.RespuestaOperacionCompletado(dato);
        }

        [HttpPost("bandeja")]
        public Respuesta<List<PlantillaProcesadaOut>> ListarBandejaPlanillas(FiltroArchivoIn plantillaProcesadaIn)
        {
            var respuesta = new Respuesta<List<PlantillaProcesadaOut>>();
            var dato = _archivoService.ListarBandejaPlanillas(plantillaProcesadaIn);
            return respuesta.RespuestaOperacionCompletado(dato);
        }
        [HttpPost("bandeja-reenvio/{banco}")]
        public Respuesta<List<PlantillaProcesadaOut>> ListarBandejaReenvio(string banco)
        {
            var respuesta = new Respuesta<List<PlantillaProcesadaOut>>();
            var dato = _archivoService.ListarBandejaReenvio(banco);
            return respuesta.RespuestaOperacionCompletado(dato);
        }
        [HttpPost("reingresar"), DisableRequestSizeLimit]
        public async Task<Respuesta<Object>> ReingresarArchivo(IFormFile file, [FromForm] string data)
        {
            var respuesta = new Respuesta<Object>();
            await _archivoService.ReingresarArchivo(file, data);
            return respuesta.RespuestaCorrecta(null, "Archivo guardado correctamente");
        }



        [HttpPost("detalle/{idArchivo}/{original}")]
        public async Task<IActionResult> ObtenerContenidoArchivo(int idArchivo, bool original)
        {
            var dato = await _archivoService.ObtenerContenidoArchivo(idArchivo, original);
            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = dato.Name,
                Inline = true
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return File(dato.FileBytes, "text/plain");
        }

        [HttpPost("tipoPlanilla/{banco}")]
        public Respuesta<List<SeleccionOpcion>> ListarTipoPlanilla(string banco)
        {
            var respuesta = new Respuesta<List<SeleccionOpcion>>();
            var dato = _archivoService.ListarTipoPlanilla(banco);
            return respuesta.RespuestaOperacionCompletado(dato);
        }

        [HttpPost("logs")]
        public Respuesta<List<LogArchivoOut>> listarLogArchivo(FiltroArchivoIn plantillaProcesadaIn)
        {
            var respuesta = new Respuesta<List<LogArchivoOut>>();
            var dato = _archivoService.ListarLogArchivo(plantillaProcesadaIn);
            return respuesta.RespuestaOperacionCompletado(dato);
        }
    }
}
