using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using MovieTicketsPurchase.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketsPurchase.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(this._orderService.GetAllOrders(userId));
        }

        public IActionResult Details(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(this._orderService.GetOrderDetails(userId, id));
        }

        public IActionResult GenerateInvoice(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._orderService.GetOrderDetails(userId, id);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var template = DocumentModel.Load(templatePath);
            template.Content.Replace("[[User]]", result.User.ToString());
            template.Content.Replace("[[OrderId]]", result.Id.ToString());
            StringBuilder sb = new StringBuilder();
            int totalQuantity = 0;
            int totalPrice = 0;
            foreach (var item in result.TicketsInOrder)
            {
                totalQuantity += item.Quantity;
                totalPrice += item.Quantity * item.SelectedTicket.Price;
                sb.AppendLine("Movie: " + item.SelectedTicket.MovieName + ", Quantity: " + item.Quantity + ", Price: $" + item.SelectedTicket.Price);
            }
            template.Content.Replace("[[TicketList]]", sb.ToString().Trim());
            template.Content.Replace("[[Quantity]]", totalQuantity.ToString());
            template.Content.Replace("[[Price]]", totalPrice.ToString());
            var stream = new MemoryStream();
            template.Save(stream, new PdfSaveOptions());
            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "OrderInvoice.pdf");
        }
    }
}
