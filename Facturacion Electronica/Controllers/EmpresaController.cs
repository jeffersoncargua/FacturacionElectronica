using AutoMapper;
using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion_Electronica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;
        protected ApiResponse _response;
        public EmpresaController(IEmpresaRepository empresaRepository, IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet("GetEmpresas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll([FromQuery] string? query = null)
        {
            var response = await _empresaRepository.GetAllEmpresaAsync(query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpGet("GetEmpresa/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Get(int id, [FromQuery] string? query = null)
        {
            var response = await _empresaRepository.GetEmpresaAsync(id, query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        /* [HttpPost("CreateEmpresa")]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // public async Task<ActionResult<ApiResponse>> Create([FromBody] EmpresaDto empresaDto)
        // {
        //    var response = await _empresaRepository.CreateEmpresaAsync(empresaDto);

        //    return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        // }*/

        [HttpPost("CreateEmpresa")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Create([FromForm] EmpresaDto empresaDto)
        {
            var response = await _empresaRepository.CreateEmpresaAsync(empresaDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        /*// [HttpPost("UpdateFileEmpresa")]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // public async Task<ActionResult<ApiResponse>> UpdateCertificate([FromQuery] string ruc, [FromQuery] string tipoArchivo, IFormFile? file = null)
        // {
        //    var response = await _empresaRepository.UpdatePathFileAsync(ruc, tipoArchivo, file);

        //    return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        // }*/

        /*[HttpPut("UpdateEmpresa/{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromBody] EmpresaDto empresaDto)
        {
            var response = await _empresaRepository.UpdateEmpresaAsync(id, empresaDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }*/

        [HttpPut("UpdateEmpresa/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromForm] EmpresaDto empresaDto)
        {
            var response = await _empresaRepository.UpdateEmpresaAsync(id, empresaDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpDelete("DeleteEmpresa/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var response = await _empresaRepository.RemoveEmpresaAsync(id);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }
    }
}