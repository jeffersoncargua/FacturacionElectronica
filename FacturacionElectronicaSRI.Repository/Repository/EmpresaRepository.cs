using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class EmpresaRepository : Repository<TblEmpresa>, IEmpresaRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _webHostingEnvironment;
        private readonly IMapper _mapper;
        protected Response _response;

        public EmpresaRepository(ApplicationDbContext db, IHostingEnvironment webHostingEnvironment, IMapper mapper)
            : base(db)
        {
            _db = db;
            _webHostingEnvironment = webHostingEnvironment;
            _mapper = mapper;
            this._response = new();
        }

        /*public async Task<Response> CreateEmpresaAsync(EmpresaDto empresaDto)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Ruc == empresaDto.Ruc || u.NombreComercial == empresaDto.NombreComercial, tracked: false);
                if (empresaDb == null)
                {
                    await this.CreateAsyn(_mapper.Map<TblEmpresa>(empresaDto));

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
        }*/

        public async Task<Response> CreateEmpresaAsync(EmpresaDto empresaDto)
        {
            try
            {
                string rutaLogo = string.Empty;
                string rutaCertificado = string.Empty;
                string rutaCarpeta = _webHostingEnvironment.ContentRootPath + @"\Archivos";

                var empresaDb = await this.GetAsync(u => u.Ruc == empresaDto.Ruc || u.NombreComercial == empresaDto.NombreComercial, tracked: false);
                if (empresaDb == null)
                {
                    if (empresaDto.LogoEmpresa != null)
                    {
                        // Se obtiene la extension del archivo
                        var extensionLogo = Path.GetExtension(empresaDto.LogoEmpresa.FileName);

                        string carpetaLogoEmpresa = Path.Combine(rutaCarpeta, @"LogoEmpresa");

                        if (!Directory.Exists(carpetaLogoEmpresa))
                        {
                            Directory.CreateDirectory(carpetaLogoEmpresa);
                        }

                        using var fileStreamLogo = new FileStream(Path.Combine(carpetaLogoEmpresa, empresaDto.NombreComercial + extensionLogo), FileMode.Create);
                        empresaDto.LogoEmpresa.CopyTo(fileStreamLogo);
                        rutaLogo = rutaCarpeta + @"\LogoEmpresa\" + empresaDto.NombreComercial + extensionLogo;
                    }

                    if (empresaDto.CertificadoEmpresa != null)
                    {
                        // Se obtiene la extension del archivo
                        var extensionCertificado = Path.GetExtension(empresaDto.CertificadoEmpresa.FileName);

                        string carpetaCertificados = Path.Combine(rutaCarpeta, @"CertificadosP12");

                        if (!Directory.Exists(carpetaCertificados))
                        {
                            Directory.CreateDirectory(carpetaCertificados);
                        }

                        using (var fileStreamCertificado = new FileStream(Path.Combine(carpetaCertificados, empresaDto.Ruc + extensionCertificado), FileMode.Create))
                        {
                            empresaDto.CertificadoEmpresa.CopyTo(fileStreamCertificado);
                        }

                        rutaCertificado = rutaCarpeta + @"\CertificadoP12\" + empresaDto.Ruc + extensionCertificado;
                    }

                    TblEmpresa empresa = new()
                    {
                        Ambiente = empresaDto.Ambiente,
                        Ruc = empresaDto.Ruc,
                        RazonSocial = empresaDto.RazonSocial,
                        NombreComercial = empresaDto.NombreComercial,
                        DireccionMatriz = empresaDto.DireccionMatriz,
                        EmailEmpresa = empresaDto.EmailEmpresa,
                        ObligadoLlevarContabilidad = empresaDto.ObligadoLlevarContabilidad,
                        PathCertificado = rutaCertificado,
                        Estado = empresaDto.Estado,
                        Contraseña = empresaDto.Contraseña,
                        PathLogo = rutaLogo,
                        TelefonoEmpresa = empresaDto.TelefonoEmpresa,
                    };

                    await CreateAsyn(empresa);

                    _response.IsSuccess = true;
                    _response.Message = "Registro exitoso.";
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

        public async Task<Response> GetAllEmpresaAsync(string? query = null)
        {
            try
            {
                var empresaDb = await this.GetAllAsync(u => u.Ruc.Contains(query ?? string.Empty) || u.NombreComercial.Contains(query ?? string.Empty));
                if (empresaDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<List<EmpresaViewDto>>(empresaDb);
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

        public async Task<Response> GetEmpresaAsync(int id, string? query = null)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Id == id || u.Ruc.Contains(query ?? string.Empty) || u.NombreComercial.Contains(query ?? string.Empty), tracked: false);
                if (empresaDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<EmpresaViewDto>(empresaDb);
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

        public async Task<Response> RemoveEmpresaAsync(int id)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Id == id, tracked: false);

                if (empresaDb != null)
                {
                    await this.DeleteAsync(empresaDb);

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

        /*//public async Task<Response> UpdatePathFileAsync(string ruc, string tipoArchivo, IFormFile? file = null)
        //{
        //    try
        //    {
        //        if (file != null)
        //        {
        //            var empresaDb = await this.GetAsync(u => u.Ruc == ruc);

        //            // Se obtiene la extension del archivo
        //            var extension = Path.GetExtension(file.FileName);

        //            if (empresaDb != null)
        //            {
        //                if (tipoArchivo.Equals("logo"))
        //                {
        //                    string carpetaLogoEmpresa = Path.Combine(_webHostingEnvironment.WebRootPath, @"LogoEmpresa");

        //                    if (!Directory.Exists(carpetaLogoEmpresa))
        //                    {
        //                        Directory.CreateDirectory(carpetaLogoEmpresa);
        //                    }

        //                    // En caso de que ya exista un logo archivado en el sistema
        //                    if (empresaDb.PathLogo != null)
        //                    {
        //                        var oldFIleLogo = Path.Combine(carpetaLogoEmpresa, empresaDb.PathLogo.TrimStart('\\'));

        //                        if (File.Exists(oldFIleLogo))
        //                        {
        //                            File.Delete(oldFIleLogo);
        //                        }
        //                    }

        //                    using var fileStream = new FileStream(Path.Combine(carpetaLogoEmpresa, empresaDb.Ruc + extension), FileMode.Create);
        //                    file.CopyTo(fileStream);

        //                    string rutaLogo = @"LogoEmpresa" + empresaDb.Ruc + extension;

        //                    TblEmpresa empresaConLogo = new()
        //                    {
        //                        Id = empresaDb.Id,
        //                        Ambiente = empresaDb.Ambiente,
        //                        Ruc = empresaDb.Ruc,
        //                        RazonSocial = empresaDb.RazonSocial,
        //                        NombreComercial = empresaDb.NombreComercial,
        //                        DireccionMatriz = empresaDb.DireccionMatriz,
        //                        EmailEmpresa = empresaDb.EmailEmpresa,
        //                        ObligadoLlevarContabilidad = empresaDb.ObligadoLlevarContabilidad,
        //                        PathCertificado = empresaDb.PathCertificado,
        //                        Estado = empresaDb.Estado,
        //                        Contraseña = empresaDb.Contraseña,
        //                        PathLogo = rutaLogo,
        //                        TelefonoEmpresa = empresaDb.TelefonoEmpresa,
        //                    };

        //                    _db.TblEmpresa.Update(empresaConLogo);
        //                    await _db.SaveChangesAsync();

        //                    _response.IsSuccess = true;
        //                    _response.Message = "Se actualizo el logo de la empresa";
        //                    _response.StatusCode = HttpStatusCode.NoContent;

        //                    return _response;
        //                }

        //                string carpetaCertificados = Path.Combine(_webHostingEnvironment.WebRootPath, @"CertificadosP12");

        //                if (!Directory.Exists(carpetaCertificados))
        //                {
        //                    Directory.CreateDirectory(carpetaCertificados);
        //                }

        //                // En caso de que ya exista un certificado archivado en el sistema
        //                if (empresaDb.PathCertificado != null)
        //                {
        //                    var oldFileCertificado = Path.Combine(carpetaCertificados, empresaDb.PathCertificado.TrimStart('\\'));

        //                    if (File.Exists(oldFileCertificado))
        //                    {
        //                        File.Delete(oldFileCertificado);
        //                    }
        //                }

        //                using (var fileStream = new FileStream(Path.Combine(_webHostingEnvironment.WebRootPath, @"CertificadosP12", empresaDb.Ruc + extension), FileMode.Create))
        //                {
        //                    file.CopyTo(fileStream);
        //                }

        //                var rutaCertificado = @"CertificadoP12" + empresaDb.Ruc + extension;

        //                TblEmpresa empresaConCertificado = new()
        //                {
        //                    Id = empresaDb.Id,
        //                    Ambiente = empresaDb.Ambiente,
        //                    Ruc = empresaDb.Ruc,
        //                    RazonSocial = empresaDb.RazonSocial,
        //                    NombreComercial = empresaDb.NombreComercial,
        //                    DireccionMatriz = empresaDb.DireccionMatriz,
        //                    EmailEmpresa = empresaDb.EmailEmpresa,
        //                    ObligadoLlevarContabilidad = empresaDb.ObligadoLlevarContabilidad,
        //                    PathCertificado = rutaCertificado,
        //                    Estado = empresaDb.Estado,
        //                    Contraseña = empresaDb.Contraseña,
        //                    PathLogo = empresaDb.PathLogo,
        //                    TelefonoEmpresa = empresaDb.TelefonoEmpresa,
        //                };

        //                _db.TblEmpresa.Update(empresaConCertificado);
        //                await _db.SaveChangesAsync();

        //                _response.IsSuccess = true;
        //                _response.Message = "Se actualizo el certificado de la empresa";
        //                _response.StatusCode = HttpStatusCode.NoContent;

        //                return _response;
        //            }

        //            _response.IsSuccess = false;
        //            _response.Message = "No existe la empresa";
        //            _response.StatusCode = HttpStatusCode.NotFound;

        //            return _response;
        //        }

        //        _response.IsSuccess = false;
        //        _response.Message = "No se pudo agregar el archivo";
        //        _response.StatusCode = HttpStatusCode.BadRequest;

        //        return _response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = $"Se produjo un error inesperado en el servidor. Error: {ex.Message}";
        //        _response.StatusCode = HttpStatusCode.InternalServerError;

        //        return _response;
        //    }
        //}

        //public async Task<Response> UpdateEmpresaAsync(int id, EmpresaDto empresaDto)
        //{
        //    try
        //    {
        //        var empresaDb = await this.GetAsync(u => u.Id == id, tracked: false);

        //        if (empresaDb != null)
        //        {
        //            TblEmpresa empresaUpdated = new()
        //            {
        //                Id = empresaDb.Id,
        //                Ruc = empresaDto.Ruc,
        //                RazonSocial = empresaDto.RazonSocial,
        //                NombreComercial = empresaDto.NombreComercial,
        //                DireccionMatriz = empresaDto.DireccionMatriz,
        //                ObligadoLlevarContabilidad = empresaDto.ObligadoLlevarContabilidad,
        //                Estado = empresaDto.Estado,
        //                PathCertificado = empresaDb.PathCertificado,
        //                Contraseña = empresaDto.Contraseña,
        //                Ambiente = empresaDto.Ambiente,
        //                PathLogo = empresaDb.PathLogo,
        //                EmailEmpresa = empresaDto.EmailEmpresa,
        //                TelefonoEmpresa = empresaDto.TelefonoEmpresa,
        //            };
        //            _db.TblEmpresa.Update(empresaUpdated);
        //            await _db.SaveChangesAsync();

        //            _response.IsSuccess = true;
        //            _response.Message = "Se actualizó el registro correctamente.";
        //            _response.StatusCode = HttpStatusCode.NoContent;

        //            return _response;
        //        }

        //        _response.IsSuccess = false;
        //        _response.Message = "Se actualizó el registro correctamente.";
        //        _response.StatusCode = HttpStatusCode.NotFound;

        //        return _response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = $"Se produjo un error en el servidor: {ex}";
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        return _response;
        //    }
        //} */

        public async Task<Response> UpdateEmpresaAsync(int id, EmpresaDto empresaDto)
        {
            try
            {
                var empresaDb = await this.GetAsync(u => u.Id == id, tracked: false);
                string rutaCarpeta = _webHostingEnvironment.ContentRootPath + "\\Archivos";

                if (empresaDb != null)
                {
                    var rutaLogo = empresaDb.PathLogo;
                    var rutaCertificado = empresaDb.PathCertificado;

                    if (empresaDto.LogoEmpresa != null)
                    {
                        if(empresaDb.PathLogo != null)
                        {
                            var oldPathLogo = Path.Combine(rutaCarpeta, empresaDb.PathLogo.TrimStart('\\'));

                            if (File.Exists(oldPathLogo))
                            {
                                File.Delete(oldPathLogo);
                            }
                        }

                        var extensionLogo = Path.GetExtension(empresaDto.LogoEmpresa.FileName);
                        using var fileStreamLogo = new FileStream(Path.Combine(rutaCarpeta, @"LogoEmpresa", empresaDto.NombreComercial + extensionLogo), FileMode.Create);
                        empresaDto.LogoEmpresa.CopyTo(fileStreamLogo);

                        rutaLogo = rutaCarpeta + @"\LogoEmpresa\" + empresaDto.NombreComercial + extensionLogo;
                    }

                    if (empresaDto.CertificadoEmpresa != null)
                    {
                        if (empresaDb.PathCertificado != null)
                        {
                            var oldPathCertificado = Path.Combine(rutaCarpeta, empresaDb.PathCertificado.TrimStart('\\'));

                            if (File.Exists(oldPathCertificado))
                            {
                                File.Delete(oldPathCertificado);
                            }
                        }

                        var extensionCertificado = Path.GetExtension(empresaDto.CertificadoEmpresa.FileName);
                        using var fileStreamCertificado = new FileStream(Path.Combine(rutaCarpeta, @"CertificadosP12", empresaDto.Ruc + extensionCertificado), FileMode.Create);
                        empresaDto.CertificadoEmpresa.CopyTo(fileStreamCertificado);

                        rutaCertificado = rutaCarpeta + @"\CertificadosP12\" + empresaDto.Ruc + extensionCertificado;
                    }

                    TblEmpresa empresaUpdated = new()
                    {
                        Id = empresaDb.Id,
                        Ruc = empresaDto.Ruc,
                        RazonSocial = empresaDto.RazonSocial,
                        NombreComercial = empresaDto.NombreComercial,
                        DireccionMatriz = empresaDto.DireccionMatriz,
                        ObligadoLlevarContabilidad = empresaDto.ObligadoLlevarContabilidad,
                        Estado = empresaDto.Estado,
                        PathCertificado = rutaCertificado,
                        Contraseña = empresaDto.Contraseña,
                        Ambiente = empresaDto.Ambiente,
                        PathLogo = rutaLogo,
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