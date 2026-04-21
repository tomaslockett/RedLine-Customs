using System;

namespace RedLine.Be
{
    public abstract class AutoDecorator : Auto
    {
        protected Auto _auto; 

        public AutoDecorator(Auto auto)
        {
            _auto = auto;
            this.Id = auto.Id;
            this.Marca = auto.Marca;
            this.Modelo = auto.Modelo;
            this.Stock = auto.Stock;
            this.ImageUrl = auto.ImageUrl;
        }

        public override double GetPrecio() => _auto.GetPrecio();
        public override int GetVelocidad() => _auto.GetVelocidad();
        public override int GetAceleracion() => _auto.GetAceleracion();
        public override string GetDescripcion() => _auto.GetDescripcion();
    }
}