using AutoMapper;
using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion_Electronica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CostumerController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        protected ApiResponse _response;
        public CostumerController(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet("GetCostumers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll([FromQuery] string? query = null)
        {
            var response = await _clienteRepository.GetAllClienteAsync(query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpGet("GetCostumer/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Get(int id, [FromQuery] string? query = null)
        {
            var response = await _clienteRepository.GetClienteAsync(id, query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpPost("CreateCostumer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] ClienteDto clienteDto)
        {
            var response = await _clienteRepository.CreateClienteAsync(clienteDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpPut("UpdateCostumer/{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromBody] ClienteDto clienteDto)
        {
            var response = await _clienteRepository.UpdateClienteAsync(id, clienteDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpDelete("DeleteCostumer/{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var response = await _clienteRepository.RemoveClienteAsync(id);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }
    }
}