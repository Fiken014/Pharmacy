using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PIV_PF_KendallBonillaMiranda;

namespace PIV_PF_KendallBonillaMiranda.Controllers
{
    public class ProductosController : Controller
    {
        private PIV_PF_ProyectoFinalEntities db = new PIV_PF_ProyectoFinalEntities();

        // GET: Products
        public ActionResult Index()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            return View(db.Productos.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productos product = db.Productos.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.TiposProductos = new SelectList(db.TiposProductos, "TipoID", "TipoNombre");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoID,Nombre,Descripcion,Precio,Stock,Estado,TipoID")] Productos product)
        {
            if (ModelState.IsValid)
            {
                // Verifica si el ProductoID ya existe
                if (db.Productos.Any(p => p.ProductoID == product.ProductoID))
                {
                    ModelState.AddModelError("ProductoID", "El ID del producto ya existe");
                    ViewBag.TiposProductos = new SelectList(db.TiposProductos, "TipoID", "TipoNombre", product.TipoID);
                    return View(product);
                }

                // Asigna el TipoID seleccionado
                int tipoId;
                if (!int.TryParse(Request.Form["TipoID"], out tipoId))
                {
                    ModelState.AddModelError("TipoID", "Seleccione un tipo de producto válido.");
                    ViewBag.TiposProductos = new SelectList(db.TiposProductos, "TipoID", "TipoNombre", product.TipoID);
                    return View(product);
                }
                product.TipoID = tipoId;

                db.Productos.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TiposProductos = new SelectList(db.TiposProductos, "TipoID", "TipoNombre", product.TipoID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productos product = db.Productos.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.TiposProductos = new SelectList(db.TiposProductos, "TipoID", "TipoNombre", product.TipoID);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoID,Nombre,Descripcion,Precio,Stock,Estado,TipoID")] Productos product)
        {
            if (ModelState.IsValid)
            {
                // Asigna el TipoID seleccionado
                int tipoId;
                if (!int.TryParse(Request.Form["TipoID"], out tipoId))
                {
                    ModelState.AddModelError("TipoID", "Seleccione un tipo de producto válido.");
                    ViewBag.TiposProductos = new SelectList(db.TiposProductos, "TipoID", "TipoNombre", product.TipoID);
                    return View(product);
                }
                product.TipoID = tipoId;

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TiposProductos = new SelectList(db.TiposProductos, "TipoID", "TipoNombre", product.TipoID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productos product = db.Productos.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Productos product = db.Productos.Find(id);

            // Check if there are any related order details
            if (db.DetallePedidos.Any(od => od.ProductoID == id))
            {
                ModelState.AddModelError("", "No se puede eliminar el producto porque hay detalles de pedido relacionados en la base de datos.");
                return View(product);
            }
            db.Productos.Remove(product);
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
