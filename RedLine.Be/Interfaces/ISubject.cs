using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be.Interfaces
{
    public interface ISubject
    {
        void AgregarObserver(IObserver observer);
        void QuitarObserver(IObserver observer);
        void Notificar();
    }
}
