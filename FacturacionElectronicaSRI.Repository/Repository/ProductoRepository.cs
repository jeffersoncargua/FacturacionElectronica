using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Producto.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class ProductoRepository : Repository<TblProductos>, IProductoRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        protected Response _response;

        public ProductoRepository(ApplicationDbContext db, IMapper mapper)
            : base(db)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }

        public async Task<Response> CreateProductoAsync(ProductoDto productoDto)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.CodigoPrincipal == productoDto.CodigoPrincipal || u.CodigoAuxiliar == productoDto.CodigoAuxiliar || u.Descripcion.Equals(productoDto.Descripcion), tracked: false);
                if (empresaDb == null)
                {
                    await this.CreateAsyn(_mapper.Map<TblProductos>(productoDto));

                    _response.IsSuccess = true;
                    _response.Message = "Registro Exitoso";
                    _response.StatusCode = HttpStatusCode.Created;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "Ya existe un registro con el codigo principal, auxiliar y/o detalle.";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Se produjo un error en el servidor: {ex}";
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        public async Task<Response> GetAllProductoAsync(string? query = null)
        {
            try
            {
                var productosDb = await this.GetAllAsync(u => u.CodigoPrincipal == query || u.CodigoAuxiliar == query || u.Descripcion.Contains(query ?? string.Empty), tracked: false);
                if (productosDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<List<ProductoDto>>(productosDb);
                    _response.StatusCode = HttpStatusCode.OK;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "No se encontró el registro solicitado";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Se produjo un error en el servidor: {ex}";
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        public async Task<Response> GetProductoAsync(int id, string? query = null)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.CodigoPrincipal == query || u.CodigoAuxiliar == query || u.Descripcion.Contains(query ?? string.Empty), tracked: false);
                if (empresaDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<ProductoDto>(empresaDb);
                    _response.StatusCode = HttpStatusCode.OK;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "No se encontró el registro solicitado";
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Se produjo un error en el servidor: {ex}";
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        public async Task<Response> RemoveProductoAsync(int id)
        {
            try
            {
                var productoDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (productoDb != null)
                {
                    await this.DeleteAsync(productoDb);

                    _response.IsSuccess = true;
                    _response.Message = "Se eliminó el registro correctamente.";
                    _response.StatusCode = HttpStatusCode.NoContent;

                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "El registro no existe.";
                _response.StatusCode = HttpStatusCode.NotFound;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Se produjo un error en el servidor: {ex}";
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        public async Task<Response> UpdateProductoAsync(int id, ProductoDto productoDto)
        {
            try
            {
                var productoDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (productoDb != null)
                {
                    TblProductos productoUpdated = new()
                    {
                        Id = productoDb.Id,
                        CodigoPrincipal = productoDto.CodigoPrincipal,
                        CodigoAuxiliar = productoDto.CodigoAuxiliar,
                        Descripcion = productoDto.Descripcion,
                        PrecioUnitario = productoDto.PrecioUnitario,
                        Estado = productoDto.Estado,
                        Cantidad = productoDto.Cantidad,
                    };
                    _db.TblProducto.Update(productoUpdated);
                    await _db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Se actualizó el registro correctamente.";
                    _response.StatusCode = HttpStatusCode.NoContent;

                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "Se actualizó el registro correctamente.";
                _response.StatusCode = HttpStatusCode.NotFound;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Se produjo un error en el servidor: {ex}";
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
    }
}