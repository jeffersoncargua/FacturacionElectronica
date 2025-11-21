using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class ClienteRepository : Repository<TblCliente>, IClienteRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        protected Response _response;

        public ClienteRepository(ApplicationDbContext db, IMapper mapper)
            : base(db)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }

        public async Task<Response> CreateClienteAsync(ClienteDto clienteDto)
        {
            try
            {
                var clienteDb = await this.GetAsync(u => u.Identificacion == clienteDto.Identificacion, tracked: false);
                if (clienteDb == null)
                {
                    await this.CreateAsyn(_mapper.Map<TblCliente>(clienteDto));

                    _response.IsSuccess = true;
                    _response.Message = "Registro Exitoso";
                    _response.StatusCode = HttpStatusCode.Created;
                    return _response;
                }

                _response.IsSuccess = false;
                _response.Message = "Ya existe un registro con el numero de identificacion";
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

        public async Task<Response> GetAllClienteAsync(string? query = null)
        {
            try
            {
                var clienteDb = await this.GetAllAsync(u => u.Email.Contains(query ?? string.Empty) || u.Nombres.Contains(query ?? string.Empty));
                if (clienteDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<List<ClienteDto>>(clienteDb);
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

        public async Task<Response> GetClienteAsync(int id, string? query = null)
        {
            try
            {
                TblCliente clienteDb = query != null ?
                    await this.GetAsync(u => u.Identificacion == query || u.Email == query, tracked: false)
                    : await this.GetAsync(u => u.Id == id, tracked: false);

                if (clienteDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<ClienteDto>(clienteDb);
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

        public async Task<Response> RemoveClienteAsync(int id)
        {
            try
            {
                var clienteDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (clienteDb != null)
                {
                    await this.DeleteAsync(clienteDb);

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

        public async Task<Response> UpdateClienteAsync(int id, ClienteDto clienteDto)
        {
            try
            {
                var clienteDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (clienteDb != null)
                {
                    TblCliente clienteUpdated = new()
                    {
                        Id = clienteDb.Id,
                        Identificacion = clienteDb.Identificacion,
                        Direccion = clienteDto.Direccion,
                        Nombres = clienteDto.Nombres,
                        Telefono = clienteDto.Telefono,
                        Email = clienteDto.Email,
                    };
                    _db.TblCliente.Update(clienteUpdated);
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