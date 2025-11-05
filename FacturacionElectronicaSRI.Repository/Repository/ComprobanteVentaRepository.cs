using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Newtonsoft.Json;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class ComprobanteVentaRepository : Repository<TblComprobanteVenta>, IComprobanteVentaRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        protected Response _response;

        public ComprobanteVentaRepository(ApplicationDbContext db, IMapper mapper)
            : base(db)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }

        public async Task<Response> CreateComprobanteVentaAsync(ComprobanteVentaDto comprobanteVentaDto)
        {
            try
            {
                if (!TipoComprobanteExist(comprobanteVentaDto.TipoComprobante))
                {
                    _response.IsSuccess = false;
                    _response.Message = "El tipo de comprobante no existe.";
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }

                var comprobanteVentaDB = await this.GetAsync(u => u.IdCliente == comprobanteVentaDto.IdCliente, tracked: false);
                if (comprobanteVentaDB != null)
                {
                    await this.CreateAsyn(_mapper.Map<TblComprobanteVenta>(comprobanteVentaDto));

                    _response.IsSuccess = true;
                    _response.Message = "Registro Exitoso";
                    _response.StatusCode = HttpStatusCode.Created;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "El cliente no tiene registros";
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

        public async Task<Response> GetAllComprobanteVentaAsync(string? query = null, string? startDate = null, string? endDate = null)
        {
            try
            {
                List<TblComprobanteVenta> comprobanteVentasDb;

                if (!string.IsNullOrEmpty(query) && !(string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)))
                {
                    var startDateParsed = JsonConvert.DeserializeObject<DateTime>(startDate!);
                    var endDateParsed = JsonConvert.DeserializeObject<DateTime>(endDate!);
                    comprobanteVentasDb = await this.GetAllAsync(u => u.Cliente!.Identificacion.Contains(query ?? string.Empty) || (u.FechaEmision >= startDateParsed && u.FechaEmision <= endDateParsed), tracked: false, includeProperties: "TblEmpresa,TblCliente");
                }
                else if (!(string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)))
                {
                    var startDateParsed = JsonConvert.DeserializeObject<DateTime>(startDate!);
                    var endDateParsed = JsonConvert.DeserializeObject<DateTime>(endDate!);
                    comprobanteVentasDb = await this.GetAllAsync(u => u.FechaEmision >= startDateParsed && u.FechaEmision <= endDateParsed, tracked: false, includeProperties: "TblEmpresa,TblCliente");
                }
                else
                {
                    comprobanteVentasDb = await this.GetAllAsync(tracked: false, includeProperties: "TblEmpresa,TblCliente");
                }

                if(comprobanteVentasDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<List<ComprobanteVentaDto>>(comprobanteVentasDb);
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

        public async Task<Response> GetComprobanteVentaAsync(int id, string? query = null)
        {
            try
            {
                var comprobanteVentaDb = await this.GetAsync(u => u.Id == id || u.Cliente!.Identificacion == query, tracked: false, includeProperties: "TblEmpresa,TblCliente");
                if (comprobanteVentaDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<ComprobanteVentaDto>(comprobanteVentaDb);
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

        public async Task<Response> RemoveComprobanteVentaAsync(int id)
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

        public async Task<Response> UpdateComprobanteVentaAsync(int id, ComprobanteVentaDto comprobanteVentaDto)
        {
            try
            {
                var comprobanteVentaDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (comprobanteVentaDb != null)
                {
                    TblComprobanteVenta comprobanteVentaUpdated = new()
                    {
                        Id = comprobanteVentaDb.Id,
                        IdEmpresa = comprobanteVentaDb.IdEmpresa,
                        IdCliente = comprobanteVentaDb.IdCliente,
                        FechaEmision = comprobanteVentaDb.FechaEmision,
                        TipoComprobante = comprobanteVentaDto.TipoComprobante,
                        NumeroComprobante = comprobanteVentaDto.NumeroComprobante,
                        FormaPago = comprobanteVentaDto.FormaPago,
                        Subtotal = comprobanteVentaDto.Subtotal,
                        Subtotal0 = comprobanteVentaDto.Subtotal0,
                        Subtotal12 = comprobanteVentaDto.Subtotal12,
                        Descuento = comprobanteVentaDto.Descuento,
                        TotalIva = comprobanteVentaDto.TotalIva,
                        DocSri = comprobanteVentaDb.DocSri,
                    };
                    _db.TblComprobanteVenta.Update(comprobanteVentaUpdated);
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

        private static bool TipoComprobanteExist(string tipoComprobante)
        {
            Dictionary<string, string> tiposComprobante = new()
            {
                { "01", "Factura" },
                { "03", "Liquidación de compra de bienes y prestación de servicios" },
                { "04", "Nota de Crédito" },
                { "05", "Nota de Débito" },
                { "06", "Guía de Remisión" },
                { "07", "Comprobante de Retención" },
            };

            return tiposComprobante[tipoComprobante] != null;
        }

        private static bool FormaPagoExist(string formaPago)
        {
            Dictionary<string, string> formaDePago = new()
            {
                { "01", "SIN UTILIZACION DEL SISTEMA FINANCIERO" },
                { "15", "COMPENSACIÓN DE DEUDAS" },
                { "16", "TARJETA DE DÉBITO" },
                { "17", "DINERO ELECTRÓNICO" },
                { "18", "TARJETA PREPAGO" },
                { "19", "TARJETA DE CRÉDITO" },
                { "20", "OTROS CON UTILIZACIÓN DEL SISTEMA FINANCIERO" },
                { "21", "ENDOSO DE TÍTULOS" },
            };

            return formaDePago[formaPago] != null;
        }

        private static bool CodigoImpuestosExist(int codigo)
        {
            Dictionary<int, string> tiposComprobante = new()
            {
                { 2, "IVA" },
                { 3, "ICE" },
                { 5, "IRBPNR" },
            };

            return tiposComprobante[codigo] != null;
        }

        private static bool TarifaImpuestos(int codigo)
        {
            Dictionary<int, string> tarifasDePago = new()
            {
                { 0, "0%" },
                { 2, "12%" },
                { 3, "14%" },
                { 4, "15%" },
                { 5, "5%" },
                { 6, "No objeto de Impuesto" },
                { 7, "Exento de IVA" },
                { 8, "IVA diferenciado" },
                { 10, "13%" },
            };

            return tarifasDePago[codigo] != null;
        }
    }
}