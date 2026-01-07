using AutoMapper;
using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Model.Kushki.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion_Electronica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaRepository;
        private readonly IKushkiService _kushkiService;
        private readonly IMapper _mapper;
        protected ApiResponse _response;
        public VentaController(IVentaService ventaRepository, IKushkiService kushkiService, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _kushkiService = kushkiService;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet("GetAllVentas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAllVentas([FromQuery] string? query = null, [FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            var response = await _ventaRepository.GetAllComprobanteVenta(query, startDate, endDate);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpGet("GetVenta/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetVenta(int id, [FromQuery] string? query = null)
        {
            var response = await _ventaRepository.GetComprobanteVenta(id, query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpGet("GetDetallesVentas/{comprobanteId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetDetallesVentas(int comprobanteId)
        {
            var response = await _ventaRepository.GetAllDetalleVenta(comprobanteId);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpGet("GetAllDocumentosXML")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAllDocumentosXML([FromQuery] string? query = null)
        {
            var response = await _ventaRepository.GetAllDocumentoXML(query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpGet("GetDocumentoXML/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetDocumentoXML(int id, [FromQuery] string? query = null)
        {
            var response = await _ventaRepository.GetDocumentoXML(id, query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpPost("GenerarVenta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GenerarVenta([FromBody] VentaDto ventaDto)
        {
            var response = await _ventaRepository.GenerarVenta(ventaDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpPost("CreateTokenPay")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GenerarTokenKushki([FromBody] RequestTokenDto requestTokenDto)
        {
            var response = await _kushkiService.GenerarTokenKushki(requestTokenDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }
    }
}