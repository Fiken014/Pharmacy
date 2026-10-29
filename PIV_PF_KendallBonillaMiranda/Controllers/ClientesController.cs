using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PIV_PF_KendallBonillaMiranda;

namespace PIV_PF_KendallBonillaMiranda.Controllers
{
    public class ClientesController : Controller
    {
        private PIV_PF_ProyectoFinalEntities db = new PIV_PF_ProyectoFinalEntities();

        // GET: Clientes
        public ActionResult Index()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            return View(db.Clientes.ToList());
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes customer = db.Clientes.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClienteID,PersonalID,NombreCompleto,Email")] Clientes customer)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(customer.NombreCompleto))
                {
                    ModelState.AddModelError("NombreCompleto", "Formato de nombre Invalido");
                    return View(customer);
                }

                // Check if email is valid
                if (!IsValidEmail(customer.Email))
                {
                    ModelState.AddModelError("Email", "Formato de correo Invalido");
                    return View(customer);
                }

                // Check if email already exists in the database
                if (db.Clientes.Any(c => c.Email == customer.Email))
                {
                    ModelState.AddModelError("Email", "Ya existe un Cliente con este correo");
                    return View(customer);
                }

                // Check if PersonalID already exists in the database
                if (db.Clientes.Any(c => c.PersonalID == customer.PersonalID))
                {
                    ModelState.AddModelError("PersonalID", "Ya existe un Cliente con ese PersonalID");
                    return View(customer);
                }

                db.Clientes.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        // Validate name format
        private bool IsValidName(string name)
        {
            var regex = new Regex(@"[0-9]", RegexOptions.IgnoreCase);
            var m = regex.Match(name);
            return !m.Success;
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes customer = db.Clientes.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClienteID,PersonalID,NombreCompleto,Email")] Clientes customer)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(customer.NombreCompleto))
                {
                    ModelState.AddModelError("NombreCompleto", "Nombre en un formato invalido.");
                    return View(customer);
                }
                if (!IsValidEmail(customer.Email))
                {
                    ModelState.AddModelError("Email", "Formato de correo Invalido ");
                    return View(customer);
                }

                // Check if email already exists in the database
                if (db.Clientes.Any(c => c.Email == customer.Email && c.ClienteID != customer.ClienteID))
                {
                    ModelState.AddModelError("Email", "Ya existe un Cliente con este correo.");
                    return View(customer);
                }

                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes customer = db.Clientes.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Clientes customer = db.Clientes.Find(id);

            // Check if there are any related orders
            if (db.Pedidos.Any(o => o.ClienteID == id))
            {
                ModelState.AddModelError("", "No se puede eliminar el cliente porque hay pedidos relacionados en la base de datos.");
                return View(customer);
            }

            db.Clientes.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}