using Redline.Be;
using RedLine.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class personalizarAuto : System.Web.UI.Page
    {
        BLL_Auto bllAut = new BLL_Auto();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id = int.Parse(Request.QueryString["id"]);

                AutoBase auto = bllAut.DevolverAuto(id);

                lblMarca.InnerText = auto.Marca;

                lblModelo.InnerText = auto.Modelo;

                lblAnio.InnerText = auto.Anio.ToString();

                lblPrecio.InnerText = "$ " + auto.PrecioBase.ToString();

                img.Src = auto.ImagenUrl;
            }
        }
    }
}