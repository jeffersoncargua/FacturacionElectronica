using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class EmpresaRepository : Repository<TblEmpresa>, IEmpresaRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        protected Response _response;

        public EmpresaRepository(ApplicationDbContext db, IMapper mapper)
            : base(db)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }

        public async Task<Response> CreateEmpresaAsync(EmpresaDto empresaDto)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Ruc == empresaDto.Ruc || u.NombreComercial == empresaDto.NombreComercial);
                if (empresaDb == null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Registro Exitoso";
                    _response.StatusCode = HttpStatusCode.Created;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "Ya existe un registro con el ruc o nombre comercial";
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

        public async Task<Response> GetAllEmpresaAsync(string? query = null, string? includeProperties = null)
        {
            try
            {
                var empresaDb = await this.GetAllAsync(u => u.Ruc.Contains(query ?? string.Empty) || u.NombreComercial.Contains(query ?? string.Empty));
                if (empresaDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<List<EmpresaDto>>(empresaDb);
                    _response.StatusCode = HttpStatusCode.OK;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "No encontró el registro solicitado";
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

        public async Task<Response> GetEmpresaAsync(int id, string? query = null, string? includeProperties = null)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Id == id || u.Ruc.Contains(query ?? string.Empty) || u.NombreComercial.Contains(query ?? string.Empty));
                if (empresaDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<EmpresaDto>(empresaDb);
                    _response.StatusCode = HttpStatusCode.OK;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "No encontró el registro solicitado";
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

        public async Task<Response> RemoveEmpresaAsync(int id)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (empresaDb != null)
                {
                    await this.DeleteAsync(empresaDb);
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

        public async Task<Response> UpdateEmpresaAsync(int id, EmpresaDto empresaDto)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (empresaDb != null)
                {
                    TblEmpresa empresaUpdated = new()
                    {
                        Id = empresaDb.Id,
                        Ruc = empresaDto.Ruc,
                        RazonSocial = empresaDto.RazonSocial,
                        NombreComercial = empresaDto.NombreComercial,
                        DireccionMatriz = empresaDto.DireccionMatriz,
                        ObligadoLlevarContabilidad = empresaDto.ObligadoLlevarContabilidad,
                        Estado = empresaDto.Estado,
                        PathCertificado = empresaDto.PathCertificado,
                        Contraseña = empresaDto.Contraseña,
                        Ambiente = empresaDto.Ambiente,
                        PathLogo = empresaDto.PathLogo,
                        EmailEmpresa = empresaDto.EmailEmpresa,
                        TelefonoEmpresa = empresaDto.TelefonoEmpresa,
                    };
                    _db.TblEmpresa.Update(empresaUpdated);
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