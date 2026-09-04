using RedLine.Be.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Web;


namespace RedLine.Servicios
{
    public class SubjectIdioma : ISubject
    {
        private static SubjectIdioma _instancia;
        private List<IObserver> _observers = new List<IObserver>();
        private Dictionary<string, string> _traducciones;
        public string IdiomaActual { get; private set; } = "Español";

        private SubjectIdioma()
        {
            CargarIdioma(IdiomaActual);
        }

        public static SubjectIdioma Instancia
        {
            get
            {
                if (_instancia == null) _instancia = new SubjectIdioma();
                return _instancia;
            }
        }

        public void CargarIdioma(string idioma)
        {
            try
            {
                string ruta = HttpContext.Current.Server.MapPath($"~/Idiomas/{idioma}.json");
                if (File.Exists(ruta))
                {
                    string json = File.ReadAllText(ruta);
                    _traducciones = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    IdiomaActual = idioma;
                    Notificar();
                }
            }
            catch (Exception)
            {
                _traducciones = new Dictionary<string, string>();
            }
        }

        public string Traducir(string clave)
        {
            if (_traducciones != null && _traducciones.ContainsKey(clave))
            {
                return _traducciones[clave];
            }
            return $"[{clave}]";
        }

        public void AgregarObserver(IObserver observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
        }

        public void QuitarObserver(IObserver observer)
        {
            if (_observers.Contains(observer)) _observers.Remove(observer);
        }

        public void Notificar()
        {
            foreach (var observer in _observers)
            {
                observer.ActualizarIdioma(IdiomaActual);
            }
        }
    }
}
