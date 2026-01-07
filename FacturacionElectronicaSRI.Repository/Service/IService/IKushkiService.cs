using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Data.Model.Kushki.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;

namespace FacturacionElectronicaSRI.Repository.Service.IService
{
    public interface IKushkiService
    {
        Task<Response> GenerarTokenKushki(RequestTokenDto requestTokenDto);

        Task<Response> GenerarPagoKushki(VentaDto ventaDto, List<ShoppingCartDto> productsInCart, ClienteDto clienteDto, string comprobanteNum);
    }
}