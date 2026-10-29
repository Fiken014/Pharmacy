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
    public class UsuariosController : Controller
    {
        private PIV_PF_ProyectoFinalEntities db = new PIV_PF_ProyectoFinalEntities();

        // GET: Usuarios
        public ActionResult Index()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Only the administrator can see the users when logged in
            Usuarios currentUser = db.Usuarios.Find(Session["UsuarioID"]);
            if (currentUser.TipoUsuario != "Administrador")
            {
                return RedirectToAction("Login", "Usuarios");
            }

            return View(db.Usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Only the administrator can see the users when logged in
            Usuarios currentUser = db.Usuarios.Find(Session["UsuarioID"]);
            if (currentUser.TipoUsuario != "Administrador")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios user = db.Usuarios.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Only the administrator can create new users
            Usuarios currentUser = db.Usuarios.Find(Session["UsuarioID"]);
            if (currentUser.TipoUsuario != "Administrador")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var user = new Usuarios
            {
                Estado = "Activo",
                Contrasena = GeneratePassword()
            };

            return View(user);
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UsuarioID,PersonalID,NombreCompleto,Email,Contrasena,TipoUsuario,Estado")] Usuarios user)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(user.NombreCompleto))
                {
                    ModelState.AddModelError("NombreCompleto", "Nombre en un formato invalido.");
                    return View(user);
                }
                if (!IsValidEmail(user.Email))
                {
                    ModelState.AddModelError("Email", "Email en un formato invalido.");
                    return View(user);
                }

                // Check if email already exists in the database
                if (db.Usuarios.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese Email.");
                    return View(user);
                }

                // Check if PersonalID already exists in the database
                if (db.Usuarios.Any(u => u.PersonalID == user.PersonalID))
                {
                    ModelState.AddModelError("PersonalID", "Ya existe un usuario con ese ID Personal");
                    return View(user);
                }

                db.Usuarios.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // Validate email address format
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

        // Generate a random alphanumeric password
        private string GeneratePassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Only the administrator can edit users
            Usuarios currentUser = db.Usuarios.Find(Session["UsuarioID"]);
            if (currentUser.TipoUsuario != "Administrador")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios user = db.Usuarios.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UsuarioID,PersonalID,NombreCompleto,Email,Contrasena,TipoUsuario,Estado")] Usuarios user)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(user.NombreCompleto))
                {
                    ModelState.AddModelError("NombreCompleto", "Nombre con un formato invalido.");
                    return View(user);
                }
                if (!IsValidEmail(user.Email))
                {
                    ModelState.AddModelError("Email", "Email en un formato invalido.");
                    return View(user);
                }

                // Check if email already exists in the database
                if (db.Usuarios.Any(u => u.Email == user.Email && u.PersonalID != user.PersonalID))
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con este Email.");
                    return View(user);
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Only the administrator can delete users
            Usuarios currentUser = db.Usuarios.Find(Session["UsuarioID"]);
            if (currentUser.TipoUsuario != "Administrador")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios user = db.Usuarios.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuarios user = db.Usuarios.Find(id);
            db.Usuarios.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Usuarios/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuarios user)
        {
            var foundUser = db.Usuarios.FirstOrDefault(u => u.Email == user.Email && u.Contrasena == user.Contrasena && u.Estado == "Activo");
            if (foundUser != null)
            {
                // User is authenticated, create login session
                Session["UsuarioID"] = foundUser.UsuarioID;
                Session["NombreCompleto"] = foundUser.NombreCompleto;
                Session["TipoUsuario"] = foundUser.TipoUsuario;

                // Redirect to appropriate page based on user type
                if (foundUser.TipoUsuario == "Administrador")
                {
                    return RedirectToAction("Index", "Usuarios");
                }
                else if (foundUser.TipoUsuario == "Empleado")
                {
                    return RedirectToAction("Index", "Clientes");
                }
                else if (foundUser.TipoUsuario == "Contabilidad")
                {
                    return RedirectToAction("Index", "Pedidos");
                }
            }

            // Invalid credentials, add error message to view model
            ModelState.AddModelError("", "Correo electrónico, contraseña no válidos o usuario inactivo");
            return View(user);
        }

        // GET: Usuarios/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
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