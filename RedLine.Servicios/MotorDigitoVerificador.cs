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
        private BigInteger[] _sumasVerticalesParciales;
        private List<string> _nombresColumnas;

        private List<string> _dvhMatrizFilas;

        public MotorDigitoVerificador(List<string> columnas)
        {
            _nombresColumnas = columnas;
            _sumasVerticalesParciales = new BigInteger[columnas.Count];
            _dvhMatrizFilas = new List<string>();
        }

        public string ProcesarFila(string idFila, string[] valoresFila)
        {
            BigInteger sumaParcialFila = 0;

            for (int col = 0; col < _nombresColumnas.Count; col++)
            {
                string hex = Hashing.Sha256(valoresFila[col] ?? "");
                BigInteger valor = BigInteger.Parse("00" + hex, NumberStyles.HexNumber);

                sumaParcialFila += valor;
                _sumasVerticalesParciales[col] += valor;
            }

            string hexFila = Hashing.Sha256(sumaParcialFila.ToString());

            _dvhMatrizFilas.Add($"{idFila}:{hexFila}");

            return hexFila;
        }

        public (string DVH, string DVV) ObtenerResultadoFinal()
        {
            List<string> dvvPartes = new List<string>();

            for (int col = 0; col < _nombresColumnas.Count; col++)
            {
                string hexCol = Hashing.Sha256(_sumasVerticalesParciales[col].ToString());
                dvvPartes.Add($"{_nombresColumnas[col]}:{hexCol}");
            }

            return (string.Join("|", _dvhMatrizFilas), string.Join("|", dvvPartes));
        }
    }
}
