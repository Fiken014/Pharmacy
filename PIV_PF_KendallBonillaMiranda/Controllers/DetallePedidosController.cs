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
    public class DetallePedidosController : Controller
    {
        private PIV_PF_ProyectoFinalEntities db = new PIV_PF_ProyectoFinalEntities();

        // GET: DetallePedidos
        public ActionResult Index()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var detallePedidos = db.DetallePedidos.Include(d => d.Pedidos).Include(d => d.Productos);
            return View(detallePedidos.ToList());
        }

        // GET: DetallePedidos/Details/5
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
            DetallePedidos detallePedido = db.DetallePedidos.Find(id);
            if (detallePedido == null)
            {
                return HttpNotFound();
            }
            return View(detallePedido);
        }

        // GET: DetallePedidos/Create
        public ActionResult Create()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            ViewBag.PedidoID = new SelectList(db.Pedidos, "PedidoID", "MetodoPago");
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.Estado == "Disponible"), "ProductoID", "Nombre");
            return View();
        }

        // POST: DetallePedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DetallePedidoID,PedidoID,ProductoID,Cantidad")] DetallePedidos detallePedido)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (ModelState.IsValid)
            {
                db.DetallePedidos.Add(detallePedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PedidoID = new SelectList(db.Pedidos, "PedidoID", "MetodoPago", detallePedido.PedidoID);
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.Estado == "Disponible"), "ProductoID", "Nombre", detallePedido.ProductoID);
            return View(detallePedido);
        }

        // GET: DetallePedidos/Edit/5
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
            DetallePedidos detallePedido = db.DetallePedidos.Find(id);
            if (detallePedido == null)
            {
                return HttpNotFound();
            }

            ViewBag.PedidoID = new SelectList(db.Pedidos, "PedidoID", "MetodoPago", detallePedido.PedidoID);
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.Estado == "Disponible"), "ProductoID", "Nombre", detallePedido.ProductoID);
            return View(detallePedido);
        }

        // POST: DetallePedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DetallePedidoID,PedidoID,ProductoID,Cantidad")] DetallePedidos detallePedido)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (ModelState.IsValid)
            {
                db.Entry(detallePedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PedidoID = new SelectList(db.Pedidos, "PedidoID", "MetodoPago", detallePedido.PedidoID);
            ViewBag.ProductoID = new SelectList(db.Productos.Where(p => p.Estado == "Disponible"), "ProductoID", "Nombre", detallePedido.ProductoID);
            return View(detallePedido);
        }

        // GET: DetallePedidos/Delete/5
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
            DetallePedidos detallePedido = db.DetallePedidos.Find(id);
            if (detallePedido == null)
            {
                return HttpNotFound();
            }
            return View(detallePedido);
        }

        // POST: DetallePedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            DetallePedidos detallePedido = db.DetallePedidos.Find(id);
            db.DetallePedidos.Remove(detallePedido);
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