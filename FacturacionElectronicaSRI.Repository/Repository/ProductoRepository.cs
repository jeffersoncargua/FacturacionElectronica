using AutoMapper;
using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Producto.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class ProductoRepository : Repository<TblProductos>, IProductoRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _webHostingEnviroment;
        private readonly ApplicationURL _appUrl;
        protected Response _response;

        public ProductoRepository(ApplicationDbContext db, IMapper mapper, IHostingEnvironment webHostingEnviroment, ApplicationURL appUrl)
            : base(db)
        {
            _db = db;
            _mapper = mapper;
            _webHostingEnviroment = webHostingEnviroment;
            _appUrl = appUrl;
            this._response = new();
        }

        public async Task<Response> CreateProductoAsync(ProductoDto productoDto)
        {
            try
            {
                // var rutaCarpeta = _webHostingEnviroment.ContentRootPath + "\\Archivos";
                var rutaCarpeta = _webHostingEnviroment.WebRootPath;
                var productoDb = await this.GetAsync(u => u.CodigoPrincipal == productoDto.CodigoPrincipal || u.CodigoAuxiliar == productoDto.CodigoAuxiliar || u.Descripcion.Equals(productoDto.Descripcion), tracked: false);
                if (productoDb == null)
                {
                    if (productoDto.File != null)
                    {
                        string carpetaImagen = Path.Combine(rutaCarpeta, @"Imagen");

                        if (!Directory.Exists(carpetaImagen))
                        {
                            Directory.CreateDirectory(carpetaImagen);
                        }

                        var extension = Path.GetExtension(productoDto.File.FileName);

                        using var fileStream = new FileStream(Path.Combine(carpetaImagen, productoDto.CodigoAuxiliar + extension), FileMode.Create);
                        productoDto.File.CopyTo(fileStream);

                        // string pathImage = @"\Imagen\" + productoDto.CodigoPrincipal + extension;
                        string pathImage = _appUrl.Url + @"/Imagen/" + productoDto.CodigoPrincipal + extension;

                        productoDto.PathImagen = pathImage;
                    }

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

        public async Task<Response> GetCantProductoAsync()
        {
            try
            {
                var productosDb = await this.GetAllAsync();
                if (productosDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido la cantidad total de registros ";
                    _response.Result = productosDb.Count;
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

        public async Task<Response> GetAllProductoAsync(string? query = null, int pageSize = 0, int pageNumber = 0)
        {
            try
            {
                var productosDb = await this.GetAllAsync(u => u.CodigoPrincipal == query || u.CodigoAuxiliar == query || u.Descripcion.Contains(query ?? string.Empty), pageSize: pageSize, pageNumber: pageNumber);
                if (productosDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se han obtenido los registros solicitados";
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
                TblProductos productoDb = query != null
                    ? await this.GetAsync(u => u.CodigoPrincipal == query || u.CodigoAuxiliar == query || u.Descripcion.Contains(query ?? string.Empty), tracked: false)
                    : await this.GetAsync(u => u.Id == id, tracked: false);

                if (productoDb != null)
                {
                    _response.IsSuccess = true;
                    _response.Message = "Se ha obtenido el registro solicitado";
                    _response.Result = _mapper.Map<ProductoDto>(productoDb);
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
                    productoDto.PathImagen = productoDb.PathImagen; // Se almacena el path de la imagen en caso de que no haya un archivo

                    if (productoDto.File != null)
                    {
                        // var rutaCarpeta = _webHostingEnviroment.ContentRootPath + "\\Archivos";
                        var rutaCarpeta = _webHostingEnviroment.WebRootPath;
                        var extension = Path.GetExtension(productoDto.File.FileName);
                        var carpetaImagen = Path.Combine(rutaCarpeta, @"Imagen");

                        if (!Directory.Exists(carpetaImagen))
                        {
                            Directory.CreateDirectory(carpetaImagen);
                        }

                        if (productoDto.PathImagen != null)
                        {
                            var oldPath = Path.Combine(rutaCarpeta, productoDto.PathImagen.Trim('\\'));
                            File.Delete(productoDto.PathImagen);
                        }

                        using var fileStream = new FileStream(Path.Combine(carpetaImagen, productoDto.CodigoPrincipal + extension), FileMode.Create);
                        productoDto.File.CopyTo(fileStream);

                        // var pathImage = rutaCarpeta + @"\Imagen\" + productoDto.CodigoPrincipal + extension;
                        var pathImage = _appUrl.Url + @"/Imagen/" + productoDto.CodigoPrincipal + extension;
                        productoDto.PathImagen = pathImage;
                    }

                    TblProductos productoUpdated = new()
                    {
                        Id = productoDb.Id,
                        PathImagen = productoDto.PathImagen,
                        CodigoPrincipal = productoDto.CodigoPrincipal,
                        CodigoAuxiliar = productoDto.CodigoAuxiliar,
                        Descripcion = productoDto.Descripcion,
                        PrecioUnitario = productoDto.PrecioUnitario,
                        Estado = productoDto.Estado,
                        Cantidad = productoDto.Cantidad,
                        Descuento = productoDto.Descuento,
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
