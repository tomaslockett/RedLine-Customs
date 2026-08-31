using RedLine.Dal.Mappers;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RedLine.Bll
{
    public class BLL_Evento : AbstractBLL<int, Evento>
    {
        public BLL_Evento() : base(new DAL_Evento()) { }

        public void Registrar(string usuario, ModulosEventos modulo, string actividad, int criticidad = 1)
        {
            string moduloFormateado = Regex.Replace(modulo.ToString(), "([a-z])([A-Z])", "$1 $2");

            Evento nuevo = new Evento(usuario, moduloFormateado, actividad, criticidad);
            this.Insertar(nuevo);
        }

        public List<Evento> ListarTodo()
        {
            return this.Listar();
        }
    
        public byte[] ExportarBitacoraXmlBytes()
        {
            List<Evento> eventos = this.ListarTodo();

            XElement xmlRoot = new XElement("Bitacora");

            foreach (var item in eventos)
            {
                XElement eventoXml = new XElement("Evento",
                    new XElement("ID", item.ID),
                    new XElement("Usuario", item.Usuario ?? string.Empty),
                    new XElement("Fecha", item.Fecha.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("Modulo", item.Modulo ?? string.Empty),
                    new XElement("Actividad", item.Actividad ?? string.Empty),
                    new XElement("Criticidad", item.Criticidad)
                );

                xmlRoot.Add(eventoXml);
            }

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                xmlRoot
            );

            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
