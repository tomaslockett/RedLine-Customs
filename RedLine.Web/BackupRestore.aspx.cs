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

                lblEstado.Text = $"Copia de seguridad generada con éxito en: {entradaUsuario}";
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
            string rutaArchivoCompleta = string.Empty;

            try
            {
                if (!fileUploadRestore.HasFile || !Path.GetExtension(fileUploadRestore.FileName).Equals(".bak", StringComparison.OrdinalIgnoreCase))
                {
                    lblEstado.Text = "Error: Debe seleccionar un archivo .bak válido para poder restaurar.";
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