using RedLine.Be.Interfaces;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class BackupRestore : System.Web.UI.Page, IObserver
    {
        private BLL_BackupRestore _bllBackupRestore = new BLL_BackupRestore();
        private BLL_DigitoVerificador blldv = new BLL_DigitoVerificador();

        protected void Page_Load(object sender, EventArgs e)
        {
            SubjectIdioma.Instancia.AgregarObserver(this);

            if (!IsPostBack)
            {
                ActualizarIdioma(SubjectIdioma.Instancia.IdiomaActual);
                lblEstado.Text = SubjectIdioma.Instancia.Traducir("msgSinOperacion");
                lblEstado.ForeColor = System.Drawing.Color.Black;
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            SubjectIdioma.Instancia.QuitarObserver(this);
        }

        public void ActualizarIdioma(string nuevoIdioma)
        {
            lblTitulo.Text = SubjectIdioma.Instancia.Traducir("lblTituloBackup");
            lblSubtituloBackup.Text = SubjectIdioma.Instancia.Traducir("lblSubtituloBackup");
            btnGenerar.Text = SubjectIdioma.Instancia.Traducir("btnGenerarBackup");
            lblSubtituloRestore.Text = SubjectIdioma.Instancia.Traducir("lblSubtituloRestore");
            btnRestaurar.Text = SubjectIdioma.Instancia.Traducir("btnRestaurarBackup");
            lblBotonSeleccionar.Text = SubjectIdioma.Instancia.Traducir("lblBotonSeleccionar");
            lblSinArchivo.Text = SubjectIdioma.Instancia.Traducir("lblSinArchivo");

            if (string.IsNullOrWhiteSpace(lblEstado.Text) ||
                lblEstado.Text == SubjectIdioma.Instancia.Traducir("msgSinOperacion"))
            {
                lblEstado.Text = SubjectIdioma.Instancia.Traducir("msgSinOperacion");
            }
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                string entradaUsuario = txtRutaBackup.Text.Trim();

                if (string.IsNullOrWhiteSpace(entradaUsuario) || entradaUsuario.Equals(@"C:\", StringComparison.OrdinalIgnoreCase) || entradaUsuario.Equals("C:", StringComparison.OrdinalIgnoreCase))
                {
                    entradaUsuario = @"C:\RedlineBackups\";
                }
                else if (!entradaUsuario.EndsWith(@"\"))
                {
                    entradaUsuario += @"\";
                }

                if (!Directory.Exists(entradaUsuario))
                {
                    Directory.CreateDirectory(entradaUsuario);
                }

                _bllBackupRestore.RealizarBackup(entradaUsuario);

                lblEstado.Text = SubjectIdioma.Instancia.Traducir("msgBackupExito") + entradaUsuario;
                lblEstado.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblEstado.Text = SubjectIdioma.Instancia.Traducir("msgBackupError") + ex.Message;
                lblEstado.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnRestaurar_Click(object sender, EventArgs e)
        {
            string rutaArchivoCompleta = string.Empty;

            try
            {
                if (!fileUploadRestore.HasFile || !Path.GetExtension(fileUploadRestore.FileName).Equals(".bak", StringComparison.OrdinalIgnoreCase))
                {
                    lblEstado.Text = SubjectIdioma.Instancia.Traducir("msgArchivoInvalido");
                    lblEstado.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string carpetaTemporal = @"C:\RedlineBackups\";

                if (!Directory.Exists(carpetaTemporal))
                    Directory.CreateDirectory(carpetaTemporal);

                string nombreArchivo = fileUploadRestore.FileName;
                rutaArchivoCompleta = Path.Combine(carpetaTemporal, nombreArchivo);

                fileUploadRestore.SaveAs(rutaArchivoCompleta);

                _bllBackupRestore.RealizarRestore(rutaArchivoCompleta);

                string Errores = blldv.VerificarTodaLaBaseDeDatos();

                lblEstado.Text = SubjectIdioma.Instancia.Traducir("msgRestoreExito");
                lblEstado.ForeColor = System.Drawing.Color.Green;

                if (Session["Inconsistencia"] != null && (bool)Session["Inconsistencia"])
                {
                    Session["Inconsistencia"] = false;
                    blldv.RegistrarEventoIntegridadComprometida(Errores);
                    SessionManager.Instancia.Logout();
                    Response.Redirect("LogIn.aspx");
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = SubjectIdioma.Instancia.Traducir("msgRestoreError") + ex.Message;
                lblEstado.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                if (!string.IsNullOrEmpty(rutaArchivoCompleta) && File.Exists(rutaArchivoCompleta))
                {
                    try
                    {
                        File.Delete(rutaArchivoCompleta);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}