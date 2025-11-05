using AutoMapper;
using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Model.Producto.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion_Electronica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;
        protected ApiResponse _response;
        public ProductoController(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet("GetProductos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll([FromQuery] string? query = null)
        {
            var response = await _productoRepository.GetAllProductoAsync(query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpGet("GetProducto/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Get(int id, [FromQuery] string? query = null)
        {
            var response = await _productoRepository.GetProductoAsync(id, query);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpPost("CreateProducto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] ProductoDto productoDto)
        {
            var response = await _productoRepository.CreateProductoAsync(productoDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpPut("UpdateProducto/{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromBody] ProductoDto productoDto)
        {
            var response = await _productoRepository.UpdateProductoAsync(id, productoDto);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }

        [HttpPut("DeleteProducto/{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var response = await _productoRepository.RemoveProductoAsync(id);

            return StatusCode((int)response.StatusCode, _mapper.Map<ApiResponse>(response));
        }
    }
}