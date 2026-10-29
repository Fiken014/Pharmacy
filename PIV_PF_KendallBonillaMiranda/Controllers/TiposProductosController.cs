using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PIV_PF_KendallBonillaMiranda;

namespace PIV_PF_KendallBonillaMiranda.Controllers
{
    public class TiposProductosController : Controller
    {
        private PIV_PF_ProyectoFinalEntities db = new PIV_PF_ProyectoFinalEntities();

        // GET: TiposProductos
        public ActionResult Index()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            return View(db.TiposProductos.ToList());
        }

        // GET: TiposProductos/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TiposProductos productType = db.TiposProductos.Find(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // GET: TiposProductos/Create
        public ActionResult Create()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            return View();
        }

        // POST: TiposProductos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TipoID,TipoNombre")] TiposProductos productType)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (ModelState.IsValid)
            {
                db.TiposProductos.Add(productType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productType);
        }

        // GET: TiposProductos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TiposProductos productType = db.TiposProductos.Find(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // POST: TiposProductos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TipoID,TipoNombre")] TiposProductos productType)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (ModelState.IsValid)
            {
                db.Entry(productType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productType);
        }

        // GET: TiposProductos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TiposProductos productType = db.TiposProductos.Find(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // POST: TiposProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            TiposProductos productType = db.TiposProductos.Find(id);

            // Check if there are any related products
            if (db.Productos.Any(p => p.TipoID == id))
            {
                ModelState.AddModelError("", "No se puede eliminar el tipo de producto porque hay productos relacionados en la base de datos.");
                return View(productType);
            }

            db.TiposProductos.Remove(productType);
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