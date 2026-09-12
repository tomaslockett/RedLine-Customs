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
    public partial class BackupRestore : BasePage
    {
        private BLL_BackupRestore _bllBackupRestore = new BLL_BackupRestore();
        private BLL_DigitoVerificador blldv = new BLL_DigitoVerificador();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblEstado.Text = Traducir("msgSinOperacion");
                lblEstado.ForeColor = System.Drawing.Color.Black;
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

                lblEstado.Text = Traducir("msgBackupExito") + entradaUsuario;
                lblEstado.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblEstado.Text = Traducir("msgBackupError") + ex.Message;
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
                    lblEstado.Text = Traducir("msgArchivoInvalido");
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

                string errores = blldv.VerificarTodaLaBaseDeDatos();

                lblEstado.Text = Traducir("msgRestoreExito");
                lblEstado.ForeColor = System.Drawing.Color.Green;

                if (Session["Inconsistencia"] != null && (bool)Session["Inconsistencia"])
                {
                    Session["Inconsistencia"] = false;
                    blldv.RegistrarEventoIntegridadComprometida(errores);
                    SessionManager.Instancia.Logout();
                    Response.Redirect("LogIn.aspx");
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = Traducir("msgRestoreError") + ex.Message;
                lblEstado.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                if (!string.IsNullOrEmpty(rutaArchivoCompleta) && File.Exists(rutaArchivoCompleta))
                {
                    try { File.Delete(rutaArchivoCompleta); } catch { }
                }
            }
        }
    }
}