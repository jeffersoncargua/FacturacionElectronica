using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class DetalleVentaRepository : Repository<TblDetalleVenta>, IDetalleVentaRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        protected Response _response;

        public DetalleVentaRepository(ApplicationDbContext db, IMapper mapper)
            : base(db)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }

        public async Task<Response> CreateDetalleVentaAsync(DetalleVentaDto detalleVentaDto)
        {
            try
            {
                if (detalleVentaDto != null)
                {
                    await this.CreateAsyn(_mapper.Map<TblDetalleVenta>(detalleVentaDto));

                    _response.IsSuccess = true;
                    _response.Message = "Registro Exitoso";
                    _response.StatusCode = HttpStatusCode.Created;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "Se ha producido un error al registrar el detalle";
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

        public async Task<Response> GetAllDetalleVentaAsync(int idComprobante)
        {
            try
            {
                var detalleVentasDb = await this.GetAllAsync(u => u.IdComprobanteVenta == idComprobante, tracked: false, includeProperties: "TblComprobanteVenta,TblProducto");
                if (detalleVentasDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<List<DetalleVentaDto>>(detalleVentasDb);
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

        public async Task<Response> GetDetalleVentaAsync(int id)
        {
            try
            {
                var detalleVentaDb = await this.GetAsync(u => u.Id == id, tracked: false, includeProperties: "TblComprobanteVenta,TblProducto");
                if (detalleVentaDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<DetalleVentaDto>(detalleVentaDb);
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

        public async Task<Response> RemoveDetalleVentaAsync(int id)
        {
            try
            {
                var comprobanteVentaDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (comprobanteVentaDb != null)
                {
                    await this.DeleteAsync(comprobanteVentaDb);

                    _response.IsSuccess = true;
                    _response.Message = "Se eliminó el registro correctamente.";
                    _response.StatusCode = HttpStatusCode.NoContent;

                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "No existe el registro.";
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

        public async Task<Response> UpdateDetalleVentaAsync(int id, DetalleVentaDto detalleVentaDto)
        {
            try
            {
                var detalleVentaDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (detalleVentaDb != null)
                {
                    TblDetalleVenta detalleVentaUpdated = new()
                    {
                        Id = detalleVentaDb.Id,
                        IdComprobanteVenta = detalleVentaDb.IdComprobanteVenta,
                        IdProducto = detalleVentaDb.IdProducto,
                        Estado = detalleVentaDto.Estado,
                        Cantidad = detalleVentaDto.Cantidad,
                        PrecioUnitario = detalleVentaDto.PrecioUnitario,
                        Descuento = detalleVentaDto.Descuento,
                        VentaIva = detalleVentaDto.VentaIva,
                        Total = detalleVentaDto.Total,
                    };
                    _db.TblDetalleVenta.Update(detalleVentaUpdated);
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