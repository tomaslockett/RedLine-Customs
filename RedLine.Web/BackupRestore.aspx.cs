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
    public partial class BackupRestore : System.Web.UI.Page
    {
        private BLL_BackupRestore _bllBackupRestore = new BLL_BackupRestore();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblEstado.Text = "No se realizó ninguna operación.";
                lblEstado.ForeColor = System.Drawing.Color.Black;
            }
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtRutaBackup.Text))
                {
                    lblEstado.Text = "Error: Debe ingresar o pegar una ubicación en el cuadro de texto antes de generar la copia de seguridad.";
                    lblEstado.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string carpetaDestino = txtRutaBackup.Text.Trim();
                if (!carpetaDestino.EndsWith(@"\"))
                    carpetaDestino += @"\";

                _bllBackupRestore.RealizarBackup(carpetaDestino);

                lblEstado.Text = "Copia de seguridad generada con éxito.";
                lblEstado.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al generar backup: " + ex.Message;
                lblEstado.ForeColor = System.Drawing.Color.Red;
            }
        }
        BLL_DigitoVerificador blldv = new BLL_DigitoVerificador();
        protected void btnRestaurar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileUploadRestore.HasFile)
                {
                    lblEstado.Text = "Error: Debe seleccionar un archivo .bak para poder restaurar.";
                    lblEstado.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                string Errores = blldv.VerificarTodaLaBaseDeDatos();
                string carpetaTemporal = Server.MapPath("~/App_Data/Backups/");

                if (!Directory.Exists(carpetaTemporal))
                    Directory.CreateDirectory(carpetaTemporal);

                string nombreArchivo = fileUploadRestore.FileName;
                string rutaArchivoCompleta = Path.Combine(carpetaTemporal, nombreArchivo);

                fileUploadRestore.SaveAs(rutaArchivoCompleta);
                _bllBackupRestore.RealizarRestore(rutaArchivoCompleta);

                if (File.Exists(rutaArchivoCompleta))
                    File.Delete(rutaArchivoCompleta);

                lblEstado.Text = "Base de datos restaurada con éxito. El sistema se ha actualizado.";
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
                lblEstado.Text = "Error al restaurar: " + ex.Message;
                lblEstado.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}