using System;

namespace RedLine.Be
{
    public class AleronDeportivo : AutoDecorator
    {
        public AleronDeportivo(Auto auto) : base(auto) { }

        public override double GetPrecio() => _auto.GetPrecio() + 5000;
        public override string GetDescripcion() => _auto.GetDescripcion() + " + Alerón de Carbono";
        public override int GetVelocidad() => _auto.GetVelocidad() + 2;
    }

    public class SuspensionCompeticion : AutoDecorator
    {
        public SuspensionCompeticion(Auto auto) : base(auto) { }

        public override double GetPrecio() => _auto.GetPrecio() + 8000;
        public override string GetDescripcion() => _auto.GetDescripcion() + " + Suspensión de Competición";
        public override int GetAceleracion() => _auto.GetAceleracion() + 5;
    }

    public class RuedasPersonalizadas : AutoDecorator
    {
        public RuedasPersonalizadas(Auto auto) : base(auto) { }

        public override double GetPrecio() => _auto.GetPrecio() + 3500;
        public override string GetDescripcion() => _auto.GetDescripcion() + " + Llantas de Aleación";
    }

    public class PinturaEspecial : AutoDecorator
    {
        private string _color;

        public PinturaEspecial(Auto auto, string color) : base(auto)
        {
            _color = color;
        }

        public override double GetPrecio() => _auto.GetPrecio() + 4500;

        public override string GetDescripcion() => $"{_auto.GetDescripcion()} + Pintura {_color}";
    }

    public class TurboKompressor : AutoDecorator
    {
        public TurboKompressor(Auto auto) : base(auto) { }

        public override double GetPrecio() => _auto.GetPrecio() + 18500;

        public override string GetDescripcion() => _auto.GetDescripcion() + " + Sistema Turbo";

        public override int GetAceleracion() => _auto.GetAceleracion() + 25;
    }
 }
