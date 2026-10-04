using RedLine.Be.Entidades;
using RedLine.Be.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace RedLine.Servicios
{
    public class SubjectIdioma : ISubject
    {
        private static SubjectIdioma _instancia;
        private readonly List<IObserver> _observers = new List<IObserver>();

        private const string SESSION_KEY_IDIOMA = "IdiomaActual_Nombre";
        private const string SESSION_KEY_TRADUCCIONES = "IdiomaActual_Diccionario";

        private SubjectIdioma() { }

        public static SubjectIdioma Instancia
        {
            get
            {
                if (_instancia == null) _instancia = new SubjectIdioma();
                return _instancia;
            }
        }

        public string IdiomaActual
        {
            get
            {
                if (HttpContext.Current?.Session != null && HttpContext.Current.Session[SESSION_KEY_IDIOMA] != null)
                {
                    return HttpContext.Current.Session[SESSION_KEY_IDIOMA].ToString();
                }
                return "Español";
            }
            private set
            {
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[SESSION_KEY_IDIOMA] = value;
                }
            }
        }

        private Dictionary<string, string> Traducciones
        {
            get
            {
                if (HttpContext.Current?.Session != null && HttpContext.Current.Session[SESSION_KEY_TRADUCCIONES] is Dictionary<string, string> dict)
                {
                    return dict;
                }
                return new Dictionary<string, string>();
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[SESSION_KEY_TRADUCCIONES] = value;
                }
            }
        }

        public void CargarTraducciones(string nombreIdioma, Dictionary<string, string> nuevasTraducciones)
        {
            IdiomaActual = nombreIdioma;
            Traducciones = nuevasTraducciones ?? new Dictionary<string, string>();
            Notificar();
        }

        public string Traducir(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave)) return string.Empty;

            var dicc = Traducciones;
            if (dicc != null && dicc.TryGetValue(clave, out string texto) && !string.IsNullOrWhiteSpace(texto))
            {
                return texto;
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
            var listaActual = new List<IObserver>(_observers);
            foreach (var observer in listaActual)
            {
                observer.ActualizarIdioma(IdiomaActual);
            }
        }
    }
}
