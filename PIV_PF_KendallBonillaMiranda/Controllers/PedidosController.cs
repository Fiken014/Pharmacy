using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PIV_PF_KendallBonillaMiranda;

namespace PIV_PF_KendallBonillaMiranda.Controllers
{
    public class PedidosController : Controller
    {
        private PIV_PF_ProyectoFinalEntities db = new PIV_PF_ProyectoFinalEntities();

        // GET: Pedidos
        public ActionResult Index()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var pedidos = db.Pedidos.Include(p => p.Clientes).ToList();
            return View(pedidos);
        }

        // GET: Pedidos/Details/5
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
            Pedidos pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // GET: Pedidos/Create
        public ActionResult Create()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Obtener los clientes y los productos disponibles
            ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "NombreCompleto");
            ViewBag.Productos = new SelectList(db.Productos.Where(p => p.Estado == "Disponible"), "ProductoID", "Nombre");

            return View();
        }

        // POST: Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pedidos pedido, List<SelectedProduct> selectedProducts)
        {
            // Verificación del total antes de guardar
            System.Diagnostics.Debug.WriteLine($"ClienteID: {pedido.ClienteID}");
            System.Diagnostics.Debug.WriteLine($"FechaPedido: {pedido.FechaPedido}");
            System.Diagnostics.Debug.WriteLine($"SubTotal: {pedido.SubTotal}");
            System.Diagnostics.Debug.WriteLine($"Total: {pedido.Total}");
            System.Diagnostics.Debug.WriteLine($"MetodoPago: {pedido.MetodoPago}");

            // Redondear los valores a enteros antes de guardar
            pedido.SubTotal = Math.Round(pedido.SubTotal);
            pedido.Total = Math.Round(pedido.Total);

            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Asegúrate de que los valores no sean 0 aquí
                    if (pedido.SubTotal == 0 || pedido.Total == 0)
                    {
                        ModelState.AddModelError("", "El SubTotal o Total no pueden ser 0.");
                        // Regresa a la vista si hay un error
                        ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "NombreCompleto", pedido.ClienteID);
                        ViewBag.Productos = new SelectList(db.Productos.Where(p => p.Estado == "Disponible"), "ProductoID", "Nombre");
                        return View(pedido);
                    }

                    // Guardar el pedido
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();

                    int lastPedidoId = pedido.PedidoID;

                    // Guardar los detalles de los productos
                    if (selectedProducts != null)
                    {
                        foreach (var selectedProduct in selectedProducts)
                        {
                            var producto = db.Productos.Find(selectedProduct.ProductoID);
                            if (producto != null)
                            {
                                producto.Stock -= selectedProduct.Cantidad;
                                if (producto.Stock <= 0)
                                {
                                    producto.Estado = "Agotado";
                                }

                                var detallePedido = new DetallePedidos
                                {
                                    PedidoID = lastPedidoId,
                                    ProductoID = producto.ProductoID,
                                    Cantidad = selectedProduct.Cantidad,
                                };
                                db.DetallePedidos.Add(detallePedido);
                            }
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el pedido: " + ex.Message);
            }

            ViewBag.ClienteID = new SelectList(db.Clientes, "ClienteID", "NombreCompleto", pedido.ClienteID);
            ViewBag.Productos = new SelectList(db.Productos.Where(p => p.Estado == "Disponible"), "ProductoID", "Nombre");
            return View(pedido);
        }


        // GET: Pedidos/Delete/5
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
            Pedidos pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            Pedidos pedido = db.Pedidos.Find(id);

            if (pedido != null)
            {
                var detalles = db.DetallePedidos.Where(d => d.PedidoID == id).ToList();
                foreach (var detalle in detalles)
                {
                    db.DetallePedidos.Remove(detalle);
                }

                db.Pedidos.Remove(pedido);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        // GET: /Pedidos/GetProductPrice
        [HttpGet]
        public ActionResult GetProductPrice(string id)
        {
            if (id == null)
            {
                return Json(new { error = "ID de Producto no proporcionado." }, JsonRequestBehavior.AllowGet);
            }

            Productos producto = db.Productos.Find(id);
            if (producto != null)
            {
                return Json(new { price = producto.Precio }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "Producto no encontrado." }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Pedidos/GetProductStock
        [HttpGet]
        public ActionResult GetProductStock(string id)
        {
            if (id == null)
            {
                return Json(new { error = "ID de Producto no proporcionado." }, JsonRequestBehavior.AllowGet);
            }

            Productos producto = db.Productos.Find(id);
            if (producto != null)
            {
                return Json(new { stock = producto.Stock }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "Producto no encontrado." }, JsonRequestBehavior.AllowGet);
            }
        }

        public class SelectedProduct
        {
            public string ProductoID { get; set; }
            public int Cantidad { get; set; }
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
