using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using MiProyectoJ8.Data;
 
using MiProyectoJ8.Models;
using Paragraph = iTextSharp.text.Paragraph;
using Document = iTextSharp.text.Document;
using Xceed.Words.NET;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System.Drawing;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;

namespace MiProyectoJ8.Controllers
{
    [Authorize]
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeadsController(ApplicationDbContext context)
        {
            _context = context;
        }
       
        // GET: Leads
        public async Task<IActionResult> GetLeads()
        {
              return _context.SistemaLead != null ? 
                          View(await _context.SistemaLead.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.SistemaLead'  is null.");
        }

        // GET: Leads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SistemaLead == null)
            {
                return NotFound();
            }

            var sistemaLeadEntity = await _context.SistemaLead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sistemaLeadEntity == null)
            {
                return NotFound();
            }

            return View(sistemaLeadEntity);
        }

        // GET: Leads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Edad,Email,Dirección,Telefono,Fechadecreacion,FechaModificacion,EstadoUsuario,pais,Intereses,Rol,FuenteWeb")] SistemaLeadEntity sistemaLeadEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sistemaLeadEntity);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El usuario se ha registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(sistemaLeadEntity);
        }

        // GET: Leads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SistemaLead == null)
            {
                return NotFound();
            }

            var sistemaLeadEntity = await _context.SistemaLead.FindAsync(id);
            if (sistemaLeadEntity == null)
            {
                return NotFound();
            }
            return View(sistemaLeadEntity);
        }

        // POST: Leads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Edad,Email,Dirección,Telefono,Fechadecreacion,FechaModificacion,EstadoUsuario,pais,Intereses,Rol,FuenteWeb")] SistemaLeadEntity sistemaLeadEntity)
        {
            if (id != sistemaLeadEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sistemaLeadEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SistemaLeadEntityExists(sistemaLeadEntity.Id))
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
            return View(sistemaLeadEntity);
        }
       
        // GET: Leads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SistemaLead == null)
            {
                return NotFound();
            }

            var sistemaLeadEntity = await _context.SistemaLead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sistemaLeadEntity == null)
            {
                return NotFound();
            }

            return View(sistemaLeadEntity);
        }

        // POST: Leads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SistemaLead == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SistemaLead'  is null.");
            }
            var sistemaLeadEntity = await _context.SistemaLead.FindAsync(id);
            if (sistemaLeadEntity != null)
            {
                _context.SistemaLead.Remove(sistemaLeadEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["NombreSortParm"] = sortOrder == "nombre" ? "nombre_desc" : "nombre";
            ViewData["EdadSortParm"] = sortOrder == "edad" ? "edad_desc" : "edad";
            ViewData["FechaSortParm"] = sortOrder == "fecha" ? "fecha_desc" : "fecha";
            ViewData["TelefonoSortParm"] = sortOrder == "telefono" ? "telefono_desc" : "telefono";
            ViewData["DireccionSortParm"] = sortOrder == "direccion" ? "direccion_desc" : "direccion";
            ViewData["CurrentFilter"] = searchString;

            var leads = from l in _context.SistemaLead
                        select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                leads = leads.Where(l => l.Nombre.Contains(searchString)
                                       || l.Email.Contains(searchString)
                                       || l.Dirección.Contains(searchString)
                                       || l.Telefono.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "nombre":
                    leads = leads.OrderBy(l => l.Nombre);
                    break;
                case "nombre_desc":
                    leads = leads.OrderByDescending(l => l.Nombre);
                    break;
                case "edad":
                    leads = leads.OrderBy(l => l.Edad);
                    break;
                case "edad_desc":
                    leads = leads.OrderByDescending(l => l.Edad);
                    break;
                case "fecha":
                    leads = leads.OrderBy(l => l.Fechadecreacion);
                    break;
                case "fecha_desc":
                    leads = leads.OrderByDescending(l => l.Fechadecreacion);
                    break;
                case "telefono":
                    leads = leads.OrderBy(l => l.Telefono);
                    break;
                case "telefono_desc":
                    leads = leads.OrderByDescending(l => l.Telefono);
                    break;
                case "direccion":
                    leads = leads.OrderBy(l => l.Dirección);
                    break;
                case "direccion_desc":
                    leads = leads.OrderByDescending(l => l.Dirección);
                    break;
                default:
                    leads = leads.OrderBy(l => l.Nombre);
                    break;
            }

            int pageSize = 10;
            int page = pageNumber ?? 1;
            return View(await leads.AsNoTracking().ToPagedListAsync(page, pageSize));
        }




        // CSV
        public IActionResult ExportToCSV(int id)
        {
            var lead = _context.SistemaLead.FirstOrDefault(l => l.Id == id);
            var csvData = ToCSV(new List<SistemaLeadEntity> { lead });
            return File(new System.Text.UTF8Encoding().GetBytes(csvData), "text/csv", $"mi_archivo_{id}.csv");
        }

        private string ToCSV(List<SistemaLeadEntity> leads)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,Apellido,Edad,Email,Dirección,Telefono,Fechadecreacion,FechaModificacion,EstadoUsuario,pais,Intereses,Rol,FuenteWeb");
            foreach (var lead in leads)
            {
                sb.AppendLine($"{lead.Id},{lead.Nombre},{lead.Apellido},{lead.Edad},{lead.Email},{lead.Dirección},{lead.Telefono},{lead.Fechadecreacion},{lead.FechaModificacion},{lead.EstadoUsuario},{lead.pais},{lead.Intereses},{lead.Rol},{lead.FuenteWeb}");
            }
            return sb.ToString();
        }

        private string ToCSV(SistemaLeadEntity lead)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,Apellido,Edad,Email,Dirección,Telefono,Fechadecreacion,FechaModificacion,EstadoUsuario,pais,Intereses,Rol,FuenteWeb");
            sb.AppendLine($"{lead.Id},{lead.Nombre},{lead.Apellido},{lead.Edad},{lead.Email},{lead.Dirección},{lead.Telefono},{lead.Fechadecreacion},{lead.FechaModificacion},{lead.EstadoUsuario},{lead.pais},{lead.Intereses},{lead.Rol},{lead.FuenteWeb}");
            return sb.ToString();
        }

        // CSV
        public IActionResult ExportAllToCSV()
        {
            var leads = _context.SistemaLead.ToList();
            var csvData = ToCSV(leads);
            return File(new System.Text.UTF8Encoding().GetBytes(csvData), "text/csv", "mi_archivo_.csv");
        }



        // JSON
        public IActionResult ExportToJson(int id)
        {
            var lead = _context.SistemaLead.FirstOrDefault(l => l.Id == id);

            if (lead == null)
            {
                // Manejo de error si el lead no existe
                return NotFound();
            }

            // Serializar el lead a formato JSON
            string jsonData = JsonConvert.SerializeObject(lead, Formatting.Indented);

            // Especificar el tipo de contenido
            string contentType = "application/json";

            // Devolver el archivo JSON como resultado de la acción
            return File(new System.Text.UTF8Encoding().GetBytes(jsonData), contentType, $"mi_archivo_{id}.json");
        }

 





        // Método para exportar la información de un usuario a un archivo PDF
        public IActionResult ExportToPDF(int id)
        {
            var lead = _context.SistemaLead.FirstOrDefault(l => l.Id == id);

            if (lead == null)
            {
                // Manejo de error si el lead no existe
                return NotFound();
            }

            // Crear el documento PDF
            Document document = new Document();

            // Crear el archivo PDF y el escritor
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Abrir el documento
            document.Open();

            // Agregar un párrafo adicional    
            document.Add(new Paragraph("Documento ROL."));

            // Agregar los datos del lead al documento
            document.Add(new Paragraph($"Nombre: {lead.Nombre}"));
            document.Add(new Paragraph($"Dirección: {lead.Dirección}"));
            document.Add(new Paragraph($"pais: {lead.pais}"));
            document.Add(new Paragraph($"EstadoUsuario: {lead.EstadoUsuario}"));
            document.Add(new Paragraph($"Intereses: {lead.Intereses}"));
            document.Add(new Paragraph($"Rol: {lead.Rol}"));
            

            // Agregar más campos según tus necesidades
            document.Add(new Paragraph("")); // Agregar una línea en blanco al final

            // Cerrar el documento
            document.Close();

            // Obtener los bytes del archivo PDF generado
            byte[] fileData = memoryStream.ToArray();

            // Especificar el tipo de contenido
            string contentType = "application/pdf";

            // Devolver el archivo PDF como resultado de la acción
            return File(fileData, contentType, $"mi_archivo_{id}.pdf");
        }




        // Método para exportar la información de un usuario a un archivo Word
        public IActionResult ExportToWord(int id)
        {
            var lead = _context.SistemaLead.FirstOrDefault(l => l.Id == id);

            if (lead == null)
            {
                // Manejo de error si el lead no existe
                return NotFound();
            }

            // Crear el documento Word
            using (var document = DocX.Create($"lead_{id}.docx"))
            {

                // Agregar un párrafo inicial con instrucciones o información
                document.InsertParagraph("CUENTA USUARIO").Bold().Color(Color.Red).FontSize(26d);

                document.InsertParagraph("Este Documento contiene caracterizticas que le gustan al usuario y Informacion Adicional / Creacion").FontSize(15d);
                document.InsertParagraph("").SpacingAfter(20d);
                // Agregar los datos del lead al documento         
                document.InsertParagraph($"Nombre: {lead.Nombre}").Bold().Color(Color.Blue).FontSize(16d);
                document.InsertParagraph($"Apellido: {lead.Apellido}").Bold().Color(Color.Blue).FontSize(16d);
                document.InsertParagraph($"Edad: {lead.Edad}").Bold().Color(Color.Blue).FontSize(16d);
                document.InsertParagraph($"Email: {lead.Email}").Bold().Color(Color.Blue).FontSize(16d);
                document.InsertParagraph($"Telefono: {lead.Telefono}").Bold().Color(Color.Blue).FontSize(16d)   ;
                document.InsertParagraph($"Fechadecreacion: {lead.Fechadecreacion}").Bold().Color(Color.Blue).FontSize(16d);
                document.InsertParagraph($"FechaModificacion: {lead.FechaModificacion}").Bold().Color(Color.Blue).FontSize(16d);
                document.InsertParagraph($"Intereses: {lead.Intereses}").Bold().Color(Color.Blue).FontSize(16d);
                document.InsertParagraph($"FuenteWeb: {lead.FuenteWeb}").Bold().Color(Color.Blue).FontSize(16d);
                // Agregar más campos según tus necesidades
                document.InsertParagraph(""); // Agregar una línea en blanco al final
                // Guardar el documento
                document.Save();
            }

            // Leer los bytes del archivo Word generado
            var fileData = System.IO.File.ReadAllBytes($"lead_{id}.docx");

            // Especificar el tipo de contenido
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            // Devolver el archivo Word como resultado de la acción
            return File(fileData, contentType, $"mi_archivo_{id}.docx");
        }



        private bool SistemaLeadEntityExists(int id)
        {
          return (_context.SistemaLead?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
