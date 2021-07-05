using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicketsPurchase.Domain.DomainModels;
using MovieTicketsPurchase.Domain.DTO;
using MovieTicketsPurchase.Services.Interface;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketsPurchase.Web.Controllers
{
    [Authorize(Roles = "StandardUser,Admin")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: Tickets
        public IActionResult Index(string searchDate)
        {
            ViewData["CurrentFilter"] = searchDate;
            var allTickets = this._ticketService.GetAllTickets();
            if (!String.IsNullOrEmpty(searchDate))
            {
                DateTime date = DateTime.Parse(searchDate);
                allTickets = allTickets.Where(z => z.ShowTime.Date == date.Date).ToList();
            }
            return View(allTickets);
        }

        public IActionResult ExportTickets(string genre)
        {
            var allTickets = this._ticketService.GetAllTickets();
            string result = "Genre not specified. Displaying all tickets.";
            if (!String.IsNullOrEmpty(genre))
            {
                result = "Displaying tickets for movies of genre: " + genre;
                allTickets = allTickets.Where(z => z.MovieGenre.ToLower().Equals(genre.ToLower())).ToList();
                if (allTickets.Count == 0)
                {
                    result = "There are no tickets for movies of genre: " + genre + ". Displaying all tickets.";
                    allTickets = this._ticketService.GetAllTickets();
                }
            }
            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Tickets");
                worksheet.Cell(1, 1).Value = result;
                worksheet.Cell(3, 1).Value = "Movie";
                worksheet.Cell(3, 2).Value = "Description";
                worksheet.Cell(3, 3).Value = "Genre";
                worksheet.Cell(3, 4).Value = "Showtime";
                worksheet.Cell(3, 5).Value = "Ticket Price (USD)";
                for (int i = 0; i < allTickets.Count; i++)
                {
                    var item = allTickets[i];
                    worksheet.Cell(i + 4, 1).Value = item.MovieName;
                    worksheet.Cell(i + 4, 2).Value = item.MovieDescription;
                    worksheet.Cell(i + 4, 3).Value = item.MovieGenre;
                    worksheet.Cell(i + 4, 4).Value = item.ShowTime;
                    worksheet.Cell(i + 4, 5).Value = item.Price;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,MovieImage,MovieName,MovieDescription,MovieGenre,ShowTime,Price")] Ticket ticket)
        {
            if (ModelState.IsValid)
            { 
                this._ticketService.CreateNewTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,MovieImage,MovieName,MovieDescription,MovieGenre,ShowTime,Price")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._ticketService.UpdeteExistingTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddToCart(Guid? id)
        {
            var model = this._ticketService.GetCartInfo(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart([Bind("Id", "Quantity")] AddToCartDto item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._ticketService.AddToCart(item, userId);
            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }
            return View(item);
        }

        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsForTicket(id) != null;
        }
    }
}
