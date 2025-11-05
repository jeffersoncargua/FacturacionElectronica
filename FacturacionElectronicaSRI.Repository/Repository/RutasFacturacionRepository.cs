using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class RutasFacturacionRepository : Repository<TblRutasXML>, IRutasFacturacionRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        protected Response _response;

        public RutasFacturacionRepository(ApplicationDbContext db, IMapper mapper)
            : base(db)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }

        public async Task<Response> CreateRutasFacturacionAsync(RutasFacturacionDto rutasFacturacionDto)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.ClaveAcceso == rutasFacturacionDto.ClaveAcceso, tracked: false);
                if (empresaDb != null)
                {
                    await this.CreateAsyn(_mapper.Map<TblRutasXML>(rutasFacturacionDto));

                    _response.IsSuccess = true;
                    _response.Message = "Registro Exitoso";
                    _response.StatusCode = HttpStatusCode.Created;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "Ya existe un registro con el codigo de acceso de la facturación.";
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

        public async Task<Response> GetAllRutasFacturacionAsync(string? query = null)
        {
            try
            {
                var rutasXmlDb = await this.GetAllAsync(u => u.IdEmpresa == Convert.ToInt32(query) || u.EstadoRecepcion == query, tracked: false);
                if (rutasXmlDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<List<TblRutasXML>>(rutasXmlDb);
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

        public async Task<Response> GetRutasFacturacionAsync(int id, string? query = null)
        {
            try
            {
                var rutaXmlDb = await this.GetAsync(u => u.Id == id || u.ClaveAcceso == query, tracked: false);
                if (rutaXmlDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<TblRutasXML>(rutaXmlDb);
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

        public async Task<Response> RemoveRutasFacturacionAsync(int id)
        {
            try
            {
                var rutaXmlDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (rutaXmlDb != null)
                {
                    await this.DeleteAsync(rutaXmlDb);

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

        public async Task<Response> UpdateRutasFacturacionAsync(int id, RutasFacturacionDto rutasFacturacionDto)
        {
            try
            {
                var rutaXmlDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (rutaXmlDb != null)
                {
                    TblRutasXML rutaXmlUpdated = new()
                    {
                        Id = rutaXmlDb.Id,
                        IdEmpresa = rutaXmlDb.IdEmpresa,
                        RutaGenerados = rutasFacturacionDto.RutaGenerados,
                        RutaFirmados = rutasFacturacionDto.RutaFirmados,
                        RutaAutorizados = rutasFacturacionDto.RutaAutorizados,
                        ClaveAcceso = rutaXmlDb.ClaveAcceso,
                        EstadoRecepcion = rutasFacturacionDto.EstadoRecepcion,
                        PathXMLPDF = rutasFacturacionDto.PathXMLPDF,
                    };

                    _db.TblRutasXML.Update(rutaXmlUpdated);
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