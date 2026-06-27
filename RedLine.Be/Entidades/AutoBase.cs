using System;

namespace Redline.Be
{
    public class AutoBase
    {
        public int ID { get; set; }

        public string CodigoVehiculo { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public int Anio { get; set; }

        public decimal PrecioBase { get; set; }

        public string Tipo { get; set; }

        public int Potencia { get; set; }

        public int VelocidadMaxima { get; set; }

        public decimal Aceleracion0a100 { get; set; }

        public string DescripcionGeneral { get; set; }

        public byte[] ImagenBinaria { get; set; }

        //Lois
        //Agregue campo stock
        public int Stock {  get; set; }

        public AutoBase()
        {

        }

        public AutoBase(
            int id,
            string codigoVehiculo,
            
            string marca,
            string modelo,
            int anio,
            decimal precioBase,
            int stock,
            string tipo,
            int potencia,
            int velocidadMaxima,
            decimal aceleracion0a100,
            string descripcionGeneral,
            byte[] imagenBinaria)
        {
            ID = id;
            CodigoVehiculo = codigoVehiculo;
            Marca = marca;
            Stock = stock;
            Modelo = modelo;
            Anio = anio;
            PrecioBase = precioBase;
            Tipo = tipo;
            Potencia = potencia;
            VelocidadMaxima = velocidadMaxima;
            Aceleracion0a100 = aceleracion0a100;
            DescripcionGeneral = descripcionGeneral;
            ImagenBinaria = imagenBinaria;
        }
    }
}