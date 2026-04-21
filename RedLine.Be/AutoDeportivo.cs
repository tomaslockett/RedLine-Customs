using System;

namespace RedLine.Be
{
    public class AutoDeportivo : Auto
    {
        private double _precioBase;
        private int _velocidadBase;
        private int _aceleracionBase;

        public AutoDeportivo(int id, string marca, string modelo, double precio, int vel, int ace, int stock, string img)
        {
            this.Id = id;
            this.Marca = marca;
            this.Modelo = modelo;
            this.Stock = stock;
            this.ImageUrl = img;

            this._precioBase = precio;
            this._velocidadBase = vel;
            this._aceleracionBase = ace;
        }

        public override double GetPrecio() => _precioBase;
        public override int GetVelocidad() => _velocidadBase;
        public override int GetAceleracion() => _aceleracionBase;

        public override string GetDescripcion()
        {
            return $"{Marca} {Modelo}";
        }
    }
}