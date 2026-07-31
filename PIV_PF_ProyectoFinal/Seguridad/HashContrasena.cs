using System;
using System.Security.Cryptography;

namespace PIV_PF_ProyectoFinal.Seguridad
{
    public static class HashContrasena
    {
        private const int TamanoSal = 16;
        private const int TamanoClave = 32;
        private const int Iteraciones = 100000;

        public static string Generar(string password)
        {
            using (var algoritmo = new Rfc2898DeriveBytes(password, TamanoSal, Iteraciones, HashAlgorithmName.SHA256))
            {
                var clave = algoritmo.GetBytes(TamanoClave);
                return string.Format("{0}.{1}.{2}",
                    Iteraciones,
                    Convert.ToBase64String(algoritmo.Salt),
                    Convert.ToBase64String(clave));
            }
        }

        public static bool Verificar(string password, string hashGuardado)
        {
            if (string.IsNullOrEmpty(hashGuardado))
                return false;

            var partes = hashGuardado.Split('.');
            if (partes.Length != 3)
                return false;

            int iteraciones = int.Parse(partes[0]);
            var sal = Convert.FromBase64String(partes[1]);
            var clave = Convert.FromBase64String(partes[2]);

            using (var algoritmo = new Rfc2898DeriveBytes(password, sal, iteraciones, HashAlgorithmName.SHA256))
            {
                var claveCalculada = algoritmo.GetBytes(clave.Length);
                return SonIguales(claveCalculada, clave);
            }
        }

        private static bool SonIguales(byte[] a, byte[] b)
        {
            uint diferencia = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diferencia |= (uint)(a[i] ^ b[i]);
            return diferencia == 0;
        }
    }
}
