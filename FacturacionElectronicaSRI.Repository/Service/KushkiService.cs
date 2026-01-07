using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Data.Model.Kushki.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace FacturacionElectronicaSRI.Repository.Service
{
    public class KushkiService : IKushkiService
    {
        private readonly ApplicationDbContext _db;
        private readonly KushkiConfig _kushkiConfig;
        internal Response _response;

        public KushkiService(KushkiConfig kushkiConfig, ApplicationDbContext db)
        {
            _kushkiConfig = kushkiConfig;
            _db = db;
            this._response = new();
        }

        /// <summary>
        /// Este metodo permite realizar la transaccion del pago de la venta por medio de tarjeta de credito o debito.
        /// </summary>
        /// <param name="ventaDto">Es el parametro cque contiene la informacion para realizar el cobro con kushki.</param>
        /// <param name="productsInCart">Son los productos que se quiere su informacion para generar el cobro.</param>
        /// <param name="clienteDto">Es la informacion del cliente para realizar el cobro con kushki.</param>
        /// <param name="comprobanteNum">Es el numero del comprobante que nos sirve de referencia para la venta.</param>
        /// <returns>Retorna una respuesta afirmativa o negativa para continuar con el proceso de la facturacion electronica.</returns>
        public async Task<Response> GenerarPagoKushki(VentaDto ventaDto, List<ShoppingCartDto> productsInCart, ClienteDto clienteDto, string comprobanteNum)
        {
            try
            {
                if (string.IsNullOrEmpty(ventaDto.Token))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = $"Token incorrecto o nulo";

                    return _response;
                }

                List<Product> listProducts = new();

                var documento = ventaDto.IdentificacionCliente.Length == 10 ? "CI" : "RUC";

                var nombres = clienteDto.Nombres.Split(" ");

                foreach (var item in productsInCart)
                {
                    var producto = new Product()
                    {
                        id = item.CodigoPrincipal,
                        title = item.Descripcion,
                        price = (int)(1000 * item.PrecioUnitario),
                        sku = item.CodigoPrincipal,
                        quantity = item.Cantidad,
                    };

                    listProducts.Add(producto);
                }

                object requestCharge = ventaDto.IsDeferred
                    ? new RequestDeferredChargeDto()
                    {
                        token = ventaDto.Token,
                        amount = new Amount()
                        {
                            subtotalIva = (double)ventaDto.SubTotal15,
                            subtotalIva0 = 0.00,
                            ice = 0.00,
                            iva = (double)ventaDto.TotalIVA,
                            currency = "USD",
                        },
                        metadata = new Metadata()
                        {
                            Referencia = comprobanteNum,
                        },
                        contactDetails = new ContactDetails()
                        {
                            documentNumber = documento,
                            documentType = ventaDto.IdentificacionCliente,
                            email = clienteDto.Email,
                            firstName = nombres[0],
                            lastName = nombres[1],
                            phoneNumber = $"+593{clienteDto.Telefono.Substring(1)}" ?? " ",
                        },
                        orderDetails = new OrderDetails()
                        {
                            siteDomain = "example.com",
                            shippingDetails = new ShippingDetails()
                            {
                                name = clienteDto.Nombres,
                                phone = $"+593{clienteDto.Telefono.Substring(1)}" ?? " ",
                                address1 = clienteDto.Direccion,
                                address2 = " ",
                                city = "Quito",
                                region = "Pichincha",
                                country = "Ecuador",
                            },
                            billingDetails = new BillingDetails()
                            {
                                name = clienteDto.Nombres,
                                phone = $"+593{clienteDto.Telefono.Substring(1)}" ?? " ",
                                address1 = clienteDto.Direccion,
                                address2 = " ",
                                city = "Quito",
                                region = "Pichincha",
                                country = "Ecuador",
                            },
                        },
                        productDetails = new ProductDetails()
                        {
                            product = listProducts,
                        },
                        fullResponse = "v2",
                        deferred = new Deferred()
                        {
                            graceMonths = "0",
                            creditType = "04",
                            months = ventaDto.Plazos,
                        },
                    }
                    : new RequestChargeDto()
                    {
                        token = ventaDto.Token,
                        amount = new Amount()
                        {
                            subtotalIva = (double)ventaDto.SubTotal15,
                            subtotalIva0 = 0.00,
                            ice = 0.00,
                            iva = (double)ventaDto.TotalIVA,
                            currency = "USD",
                        },
                        metadata = new Metadata()
                        {
                            Referencia = comprobanteNum,
                        },
                        contactDetails = new ContactDetails()
                        {
                            documentNumber = documento,
                            documentType = ventaDto.IdentificacionCliente,
                            email = clienteDto.Email,
                            firstName = nombres[0],
                            lastName = nombres[1],
                            phoneNumber = $"+593{clienteDto.Telefono.Substring(1)}" ?? " ",
                        },
                        orderDetails = new OrderDetails()
                        {
                            siteDomain = "example.com",
                            shippingDetails = new ShippingDetails()
                            {
                                name = clienteDto.Nombres,
                                phone = $"+593{clienteDto.Telefono.Substring(1)}" ?? " ",
                                address1 = clienteDto.Direccion,
                                address2 = " ",
                                city = "Quito",
                                region = "Pichincha",
                                country = "Ecuador",
                            },
                            billingDetails = new BillingDetails()
                            {
                                name = clienteDto.Nombres,
                                phone = $"+593{clienteDto.Telefono.Substring(1)}" ?? " ",
                                address1 = clienteDto.Direccion,
                                address2 = " ",
                                city = "Quito",
                                region = "Pichincha",
                                country = "Ecuador",
                            },
                        },
                        productDetails = new ProductDetails()
                        {
                            product = listProducts,
                        },
                        fullResponse = "v2",
                    };

                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://api-uat.kushkipagos.com");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Private-Merchant-Id", _kushkiConfig.PrivateMerchantId);
                var json = JsonConvert.SerializeObject(requestCharge);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var requestPayKushki = new HttpRequestMessage(HttpMethod.Post, "/card/v1/charges") { Content = data };

                HttpResponseMessage response = client.Send(requestPayKushki);
                client.Dispose();
                var content = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ResponseKushkiPayment>(content);

                if (response.IsSuccessStatusCode && result!.details.transactionStatus == "APPROVAL")
                {
                    TblKushkiPayment kushkiPayment = new()
                    {
                        subtotalIva = result.details.amount.subtotalIva,
                        subtotalIva0 = result.details.amount.subtotalIva0,
                        ice = result.details.amount.ice,
                        iva = result.details.amount.iva,
                        currency = result.details.amount.currency,
                        approvalCode = result.details.approvalCode,
                        approvedTransactionAmount = result.details.approvedTransactionAmount,
                        bank = result.details.binInfo.bank,
                        bindCard = result.details.binInfo.bindCard,
                        cardCountry = result.details.binInfo.cardCountry,
                        lastFourDigits = result.details.binInfo.lastFourDigits,
                        type = result.details.binInfo.type,
                        cardHolderName = result.details.cardHolderName,
                        created = result.details.created,
                        merchantId = result.details.merchantId,
                        merchantName = result.details.merchantName,
                        paymentBrand = result.details.paymentBrand,
                        processorBankName = result.details.processorBankName,
                        requestAmount = result.details.requestAmount,
                        responseCode = result.details.responseCode,
                        responseText = result.details.responseText,
                        transactionId = result.details.transactionId,
                        transactionReference = result.details.transactionReference,
                        transactionStatus = result.details.transactionStatus,
                        transactionType = result.details.transactionType,
                        ticketNumber = result.ticketNumber,
                    };

                    _db.TblKushkiPayment.Add(kushkiPayment);
                    await _db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Message = $"{result.details.responseText}";

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = $"{result!.details.responseText}. Verifique si su tarjeta tiene fondos o crédito. Presionando el icono del carrito de compras e inténtelo nuevamente.";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";

                return _response;
            }
        }

        /// <summary>
        /// Este metodo permite generar un token para continuar con el pago en Kushki.
        /// </summary>
        /// <param name="requestTokenDto">Son los parametros necesarios para realizar la solicitud del token para el pago con kushki.</param>
        /// <returns>Retorna una respuesta afirmativa o negativa si se realizo correctamente la solicitud de kushki.</returns>
        public async Task<Response> GenerarTokenKushki(RequestTokenDto requestTokenDto)
        {
            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://api-uat.kushkipagos.com"); // Esta URI es para el modo de prueba
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Public-Merchant-Id", _kushkiConfig.PublicMerchantId);

                var json = JsonConvert.SerializeObject(requestTokenDto);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/card/v1/tokens")
                {
                    Content = data,
                };

                // HttpResponseMessage response = await client.PostAsync("/card/v1/tokens", data);
                HttpResponseMessage response = client.Send(requestMessage); // La peticion se la debe realizar sincronicamente para obtener un resultado
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseTokenKushki>(content);

                if (response.IsSuccessStatusCode)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Message = $"Se genero el token de pago de kushki";
                    _response.Result = result!.token;

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = $"No se pudo generar el token de pago de kushki";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";

                return _response;
            }
        }
    }
}