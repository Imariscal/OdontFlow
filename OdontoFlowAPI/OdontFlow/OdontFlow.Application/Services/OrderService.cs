using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Order.CreateOrderDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.Order.UpdateOrderDTO;
using CreateCommand = OdontFlow.Application.BussinesProcess.Order.Command.CreateOrderCommand;
using UpdateCommand = OdontFlow.Application.BussinesProcess.Order.Command.UpdateOrderCommand;
using DeleteCommand = OdontFlow.Application.BussinesProcess.Order.Command.DeleteOrderCommand;
using GetQuery = OdontFlow.Application.BussinesProcess.Order.Query.GetOrdersQuery;
using GetByIdQuery = OdontFlow.Application.BussinesProcess.Order.Query.GetOrderByIdQuery;
using GetByFilterQuery = OdontFlow.Application.BussinesProcess.Order.Query.GetOrdersByFilterQuery;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Application.BussinesProcess.Order.Command;
using VetCV.HtmlRendererCore.PdfSharpCore;
using PdfSharpCore;
using System.Text;
using OdontFlow.Application.BussinesProcess.Lab.Query;
using OdontFlow.Domain.ViewModel.StationWork;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using OdontFlow.Catalogos.Retail.Application.BusinessProcess.Base;
using OdontFlow.Domain.DTOs.Order;

namespace OdontFlow.Application.Services;

public class OrderService(IMediator mediator) : IOrderService
{
    public async Task<PagedResult<ViewModel>> GetAsync(GetPagedOrdersQuery query)
    {
        var handler = mediator.GetQueryHandler<GetQuery, PagedResult<ViewModel>>();
        return await handler.Handle(new GetQuery(query));
    }

    public async Task<ViewModel> GetByIdAsync(Guid id)
    {
        var handler = mediator.GetQueryHandler<GetByIdQuery, ViewModel>();
        return await handler.Handle(new GetByIdQuery(id));
    }

    public async Task<PagedResult<ViewModel>> GetByFilterAsync(string? search, int page, int pageSize)
    {
        var handler = mediator.GetQueryHandler<GetByFilterQuery, PagedResult<ViewModel>>();
        return await handler.Handle(new GetByFilterQuery { Search = search, Page = page, PageSize = pageSize });
    }

    public async Task<ViewModel> CreateAsync(CreateDTO input)
    {
        var handler = mediator.GetCommandHandler<CreateCommand, ViewModel>();
        var orderCreated = await handler.Handle(new CreateCommand(input));
        var handlerConfirm = mediator.GetCommandHandler<ConfirmOrderCommand, ViewModel>();
        await handlerConfirm.Handle(new ConfirmOrderCommand(orderCreated.Id));
        return orderCreated;
    }

    public async Task<ViewModel> UpdateAsync(UpdateDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdateCommand, ViewModel>();
        return await handler.Handle(new UpdateCommand(input));
    }

    public async Task<ViewModel> DeleteAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeleteCommand, ViewModel>();
        return await handler.Handle(new DeleteCommand(id));
    }

    public async Task<ViewModel> ConfirmOrderAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<ConfirmOrderCommand, ViewModel>();
        return await handler.Handle(new ConfirmOrderCommand(id));
    }

    public async Task<ViewModel> DeleteOrdenItemAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeleteOrderItemCommand, ViewModel>();
        return await handler.Handle(new DeleteOrderItemCommand(id));
    }

    public async Task<ViewModel> DeliverOrdenAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeliveryOrderCommand, ViewModel>();
        return await handler.Handle(new DeliveryOrderCommand(id));
    }

    public async Task<byte[]> GenerateRemisionZirconiaPdf(Guid id)
    {
        var handler = mediator.GetQueryHandler<GetByIdQuery, ViewModel>();
        var order = await handler.Handle(new GetByIdQuery(id));

        var subtotal = order.Items.Sum(i => i.UnitCost * i.Quantity);
        var iva = order.ApplyInvoice ? subtotal * 0.16M : 0.00M;
        var total = subtotal + iva;
        var pagos = order.Payments.Sum(p => p.Amount);
        var saldo = total - pagos;

        var trabajos = string.Join("<br/>", order.Items.Select(i => i.ProductName));
        var ubicaciones = string.Join(", ",
            order.Items
                .SelectMany(i => i.Teeth ?? new List<PType>())
                .Select(t => GetToothCode(t))
        );

        var sb = new StringBuilder();

        sb.Append($@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            font-size: 10pt;
            margin: 40px;
        }}
        .logo-section {{
            width: 100%;
        }}
        .logo-section td {{
            vertical-align: top;
        }}
        .order-number {{
            padding-top: 4px;
            font-weight: bold;
        }}
        .lab-info {{
            text-align: right;
            font-size: 10pt;
            margin: 0px;
            width: 70%;
        }}
        .totals {{
            font-size: 10pt;
            margin-top: 8px;
        }}
        .totals td {{
            padding: 2px 6px;
        }}
        .data-section {{
            margin-top: 10px;
            margin-bottom: 15px;
        }}
        .data-section td {{
            padding-bottom: 5px;
        }}
        .gray {{
            background-color: #ccc;
            font-weight: bold;
        }}
        .firma {{
            margin-top: 60px;
            text-align: left;
        }}
        .firma-line {{
            margin-top: 30px;
            border-top: 1px solid black;
            width: 60%;
            margin-left: auto;
            margin-right: auto;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
        }}
        .trabajos-table td {{
            border: 1px solid #000;
            padding: 6px;
        }}
    </style>
</head>
<body>

    <table class='logo-section'>
        <tr>
            <td style='width: 30%;'>
                <img src='https://shop.gammadents.com/cdn/shop/files/gammadents-logo-blanco.png?v=1737670581&width=300' width='120' />
 
            </td>
            <td class='lab-info'>
                <strong>GAMMADENTS</strong><br/>
                LOMA BLANCA 2900 A, COL. DEPORTIVO OBISPADO<br/>
                MONTERREY N.L, C.P. 64040  <br/>
                Tel: (81) 8333-5157<br/>
                Laboratorio@gammadents.com
            </td>
        </tr>
	    <tr>
   	           <td style='width: 30%;'> 
                 <div class='order-number'>Orden: {order.Barcode}</div>
               </td>
               <td class='lab-info'>       
               </td>
	    <tr>
    </table>

   <table class='data-section' style='border-top:1px solid black'>
        <tr>
            <td style='vertical-align: top; padding-top: 15px;'><strong>Dr:</strong> {order.RequesterName}</td>
            <td style='vertical-align: top; padding-top: 15px;'><strong>Paciente:</strong>  {order.PatientName}</td>
 	        <td style='vertical-align: top; padding-top: 15px;'><strong>Sub Total:</strong></td>
	        <td style='vertical-align: top; padding-top: 15px;'>${subtotal:F2}</td> 
        </tr>
        <tr>
            <td style='vertical-align: top;  '><strong>Alta :</strong>{order.CreationDate:dd/MM/yyyy}</td>
            <td style='vertical-align: top; '><strong>Entrega:</strong>{order.CommitmentDate:dd/MM/yyyy}</td>
	        <td><strong>Iva:</strong></td>
	        <td>${iva:F2}</td>
        </tr>
        <tr>
            <td colspan='2'></td>           	  
	        <td><strong>Total:</strong></td>
	        <td>${total:F2}</td>
        </tr>
       <tr>
            <td colspan='2'> </td>           	  
	        <td><strong>Pagos:</strong></td>
	        <td>${pagos:F2}</td>
        </tr>
        <tr>
            <td colspan='2'> </td>           	  
	        <td><strong>Saldo:</strong></td>
            <td>${saldo:F2}</td>
        </tr>
    </table>
     
    <!-- TABLA DE TRABAJOS Y UBICACIONES -->
    <table class='trabajos-table'>
        <thead>
            <tr class='gray'>
                <td style='width: 70%;'>Trabajos</td>
                <td>Ubicaciones</td>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>{string.Join("<br/>", order.Items.Select(i => i.ProductName))}</td>
                <td>{string.Join(", ", order.Items.SelectMany(i => i.Teeth ?? new List<PType>()).Select(t => GetToothCode(t)))}</td>
            </tr>
        </tbody>
    </table>

    <div class='firma'>
        <div class='firma-line'></div>
        Nombre y Firma
    </div>

</body>
</html>");

        var html = sb.ToString();
        using var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
        using var ms = new MemoryStream();
        pdf.Save(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateRemisionGammaPdf(Guid id)
    {
        var handler = mediator.GetQueryHandler<GetByIdQuery, ViewModel>();
        var order = await handler.Handle(new GetByIdQuery(id));

        var subtotal = order.Items.Sum(i => i.UnitCost * i.Quantity);
        var iva = order.ApplyInvoice ? subtotal * 0.16M : 0.00M;
        var total = subtotal + iva;
        var pagos = order.Payments.Sum(p => p.Amount);
        var saldo = total - pagos;

        var sb = new StringBuilder();

        sb.Append($@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            font-size: 10pt;
            margin: 20px 40px;
        }}
        .product-table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }}
        .product-table th, .product-table td {{
            border: 1px solid black;
            padding: 6px 8px;
        }}
        .product-table th {{
            background-color: #ccc;
            text-align: center;
        }}
        .product-table td {{
            text-align: right;
        }}
        .product-table td:first-child,
        .product-table td:nth-child(2) {{
            text-align: left;
        }}
        .totals {{
            text-align: right;
            margin-top: 15px;
            font-size: 10pt;
        }}
        .totals div {{
            margin-bottom: 3px;
        }}
        .signature {{
            margin-top: 60px;
            text-align: center;
        }}
        .signature-line {{
            border-top: 1px solid black;
            width: 100%;
            margin: 30px 0 5px 0;
        }}
    </style>
</head>
<body>

 
    
    <!-- Encabezado con imagen y remisión -->
    <table width='100%' style='margin-bottom: 10px;'>
        <tr>
            <td style='width: 20%; vertical-align: top;'>
                
            </td>
            <td style='width: 45%; vertical-align: top; font-size: 10pt; line-height: 1.4;'>
 
                <strong>GAMMADENTS</strong><br/>
                LOMA BLANCA 2900 A, COL. DEPORTIVO OBISPADO<br/>
                MONTERREY N.L, C.P. 64040  <br/>
                Tel: (81) 8333-5157<br/>
   
        
            </td>
            <td style='width: 35%; border: 1px solid black; padding: 6px 10px; font-size: 10pt;'>
                <div style='margin-bottom: 10px;'><strong>REMISIÓN</strong><br />{order.Barcode}</div>
                <div><strong>FECHA ENTREGA</strong><br />{order.DeliveryDate:dd/MM/yyyy}</div>
            </td>
        </tr>
    </table>

    <!-- Datos del cliente -->
    <div style='border: 1px solid black; padding: 8px 10px; margin-top: 10px;'>
        <div><strong>NOMBRE:</strong> {order.Client.Name}</div>
        <div><strong>TELÉFONO:</strong> {order.Client.Phone1}</div>
        <div><strong>PACIENTE:</strong> {order.PatientName}</div>
    </div>

    <!-- Tabla de productos -->
    <table class='product-table'>
        <thead>
            <tr>
                <th>Cant.</th>
                <th>Descripción</th>
                <th>P. Unitario</th>
                <th>Importe</th>
            </tr>
        </thead>
        <tbody>");

        foreach (var item in order.Items)
        {
            var importe = item.Quantity * item.UnitCost;
            sb.Append($@"
            <tr>
                <td>{item.Quantity}</td>
                <td>{item.ProductName}</td>
                <td>${item.UnitCost:F2}</td>
                <td>${importe:F2}</td>
            </tr>");
        }

        sb.Append($@"
        </tbody>
    </table>

    <!-- Totales -->
    <div class='totals'>
        <div><strong>Total :</strong> ${subtotal:F2}</div>
        <div><strong>Iva :</strong> ${iva:F2}</div>
        <div><strong>Anticipo:</strong> ${pagos:F2}</div>
        <div><strong>Saldo :</strong> ${saldo:F2}</div>
    </div>

    <!-- Firma -->
    <div class='signature'>
        <div class='signature-line'></div>
        Firma Cliente
    </div>

    <div> 					 
	    Copia Laboratorio
	</div>		 
</body>
</html>");

        var html = sb.ToString();

        using var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
        using var ms = new MemoryStream();
        pdf.Save(ms);
        return ms.ToArray();
    }


    public async Task<byte[]> GeneratePdf(Guid id)
    {
        var handler = mediator.GetQueryHandler<GetByIdQuery, ViewModel>();
        var order = await handler.Handle(new GetByIdQuery(id));

        var handlerWorkOrders = mediator.GetQueryHandler<GetStationWorkDetailsByOrderQuery, IEnumerable<StationWorkDetailViewModel>>();
        var productWorks = await handlerWorkOrders.Handle(new GetStationWorkDetailsByOrderQuery(id));

        var elementos = new List<string>();
        if (order.Bite) elementos.Add("Bite");
        if (order.Models) elementos.Add("Models");
        if (order.Casts) elementos.Add("Casts");
        if (order.Spoons) elementos.Add("Spoons");
        if (order.Attachments) elementos.Add("Attachments");
        if (order.Analogs) elementos.Add("Analogs");
        if (order.Screws) elementos.Add("Screws");
        if (!string.IsNullOrWhiteSpace(order.Others))
            elementos.Add($"Otros: {order.Others}");

        var elementosText = string.Join(", ", elementos);

        var productWorkMap = productWorks
            .GroupBy(p => p.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.OrderNumber).FirstOrDefault() ?? ""
            );

        var sb = new StringBuilder();

        sb.Append($@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            font-size: 12pt;
            margin: 20px;
        }}
        .title {{
            font-size: 18pt;
            font-weight: bold;
            text-align: center;
            margin-bottom: 20px;
            margin-top: 20px;
        }}
        .info-table, .product-table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 15px;
        }}
        .info-table td, .info-table th, .product-table td, .product-table th {{
            border: 1px solid black;
            padding: 6px 10px;
        }}
        .product-table th {{
            background-color: #ccc;
            text-align: left;
        }}
        .barcode-img {{
            margin-top: 5px;
        }}
    </style>
</head>
<body>
    <table>
        <tr>
            <td style='width:70%'>
                <div class='title'>Detalles de la Orden : {order.Barcode}</div>
            </td>
            <td style='width:30%'>
              <img class='barcode-img' src='data:image/png;base64,{GenerateBarcodeBase64(order.Barcode)}' alt='{order.Barcode}' /> 
            </td>
        </tr>
    </table>


    <table class='info-table'>
        <tr>
            <td><strong>Fecha Alta:</strong> {order.CreationDate:dd/MM/yyyy HH:mm}</td>
            <td><strong>Fecha Compromiso:</strong> {order.CommitmentDate:dd/MM/yyyy}</td>
        </tr>
        <tr>
            <td><strong>Dr :</strong> {order.Client.Name}</td>
            <td><strong>Paciente:</strong> {order.PatientName}</td>
        </tr>
        <tr>
            <td colspan='2'><strong>Observaciones:</strong> {order.Observations}</td>
        </tr>
        <tr>
            <td><strong>Fase Solicitada:</strong>  </td>
            <td><strong>Elementos:</strong> {elementosText}</td>
        </tr>
        <tr>
            <td colspan='2'><strong>Color:</strong> {order.Color}</td>
        </tr>
    </table>

    <table class='product-table'>
        <thead>
            <tr>
                <th>Producto</th>
                <th>Piezas</th>
                <th>Orden de Trabajo</th>
            </tr>
        </thead>
        <tbody>");

        foreach (var item in order.Items)
        {
            var piezas = item.Teeth != null && item.Teeth.Any() ? string.Join(", ", item.Teeth) : "";
            var ordenTrabajo = productWorkMap.TryGetValue(item.ProductId, out var orden) ? orden : "";
            var barcodeBase64 = !string.IsNullOrWhiteSpace(ordenTrabajo) ? GenerateBarcodeBase64(ordenTrabajo) : "";

            sb.Append($@"
            <tr>
                <td>{item.ProductName}</td>
                <td>{piezas}</td>
                <td><img class='barcode-img' src='data:image/png;base64,{barcodeBase64}' alt='{ordenTrabajo}' /></td>
            </tr>");
        }

        sb.Append(@"
        </tbody>
    </table>
</body>
</html>");

        var html = sb.ToString();
        using var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
        using var ms = new MemoryStream();
        pdf.Save(ms);
        return ms.ToArray();
    }

    private string GenerateBarcodeBase64(string text)
    {
        var writer = new ZXing.BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 30,
                Width = 180,
                Margin = 0
            }
        };

        var pixelData = writer.Write(text);

        using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
        var bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, pixelData.Width, pixelData.Height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppRgb);

        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
        bitmap.UnlockBits(bitmapData);

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        return Convert.ToBase64String(ms.ToArray());
    }

    private string GetToothCode(PType tooth)
    {
        return tooth switch
        {
            PType.SUPERIOR => "SUPERIOR",
            PType.INFERIOR => "INFERIOR",
            _ => $"P{(int)tooth}"
        };
    }

}

