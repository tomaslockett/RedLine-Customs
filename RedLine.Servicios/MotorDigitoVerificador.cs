using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RedLine.Servicios
{
    public class MotorDigitoVerificador
    {
        private BigInteger _sumaTotalHorizontal = 0;
        private BigInteger[] _sumasVerticalesParciales;
        private int _cantidadColumnas;

        // Al instanciar el motor, le decimos cuántas columnas tiene la tabla
        public MotorDigitoVerificador(int cantidadColumnas)
        {
            _cantidadColumnas = cantidadColumnas;
            _sumasVerticalesParciales = new BigInteger[cantidadColumnas];
        }

        // Este método recibe solo un array de textos (la fila de la BD ya leída)
        // Se llama una vez por cada fila que exista en la tabla.
        public void ProcesarFila(string[] valoresFila)
        {
            BigInteger sumaParcialFila = 0;

            for (int col = 0; col < _cantidadColumnas; col++)
            {
                string hex = Hashing.Sha256(valoresFila[col] ?? "");
                BigInteger valor = BigInteger.Parse("00" + hex, NumberStyles.HexNumber);

                sumaParcialFila += valor;
                _sumasVerticalesParciales[col] += valor; // Vamos acumulando el vertical
            }

            string hexFila = Hashing.Sha256(sumaParcialFila.ToString());
            _sumaTotalHorizontal += BigInteger.Parse("00" + hexFila, NumberStyles.HexNumber);
        }

        // Una vez que le pasaste todas las filas, le pedís el resultado final
        public (string DVH, string DVV) ObtenerResultadoFinal()
        {
            BigInteger sumaTotalVertical = 0;

            for (int col = 0; col < _cantidadColumnas; col++)
            {
                string hexCol = Hashing.Sha256(_sumasVerticalesParciales[col].ToString());
                sumaTotalVertical += BigInteger.Parse("00" + hexCol, NumberStyles.HexNumber);
            }

            return (_sumaTotalHorizontal.ToString("X"), sumaTotalVertical.ToString("X"));
        }
    }
}
