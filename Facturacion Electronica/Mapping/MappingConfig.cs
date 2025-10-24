using AutoMapper;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;

namespace Facturacion_Electronica.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<TblEmpresa, EmpresaDto>().ReverseMap();
        }
    }
}