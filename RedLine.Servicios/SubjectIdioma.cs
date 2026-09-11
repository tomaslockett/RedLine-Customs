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
        private List<IObserver> _observers = new List<IObserver>();
        private Dictionary<string, string> _traducciones = new Dictionary<string, string>();

        public string IdiomaActual { get; private set; } = "Español";

        private SubjectIdioma() { }

        public static SubjectIdioma Instancia
        {
            get
            {
                if (_instancia == null) _instancia = new SubjectIdioma();
                return _instancia;
            }
        }

        public void CargarTraducciones(string nombreIdioma, Dictionary<string, string> nuevasTraducciones)
        {
            IdiomaActual = nombreIdioma;
            _traducciones = nuevasTraducciones ?? new Dictionary<string, string>();
            Notificar();
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
            if (!_observers.Contains(observer)) _observers.Remove(observer);
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
