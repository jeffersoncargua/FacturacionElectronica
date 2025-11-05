using AutoMapper;
using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Data.Model.Producto.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository;

namespace Facturacion_Electronica.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<TblEmpresa, EmpresaDto>().ReverseMap();
            CreateMap<TblEmpresa, EmpresaViewDto>().ReverseMap();
            CreateMap<TblProductos, ProductoDto>().ReverseMap();
            CreateMap<TblCliente, ClienteDto>().ReverseMap();
            CreateMap<TblComprobanteVenta, ComprobanteVentaDto>().ReverseMap();
            CreateMap<TblDetalleVenta, DetalleVentaDto>().ReverseMap();
            CreateMap<TblRutasXML, RutasFacturacionDto>().ReverseMap();
            CreateMap<ApiResponse, Response>().ReverseMap();
        }
    }
}