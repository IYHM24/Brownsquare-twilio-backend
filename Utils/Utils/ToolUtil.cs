using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utils
{
    public class ToolUtil
    {
        /// <summary>
        /// Metodo para validar el formato de un numero telefonico
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool ValidatePhoneNumber(string? phoneNumber, int numero_de_digitos)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            // Ejemplo simple de validación: verificar que el número tenga 10 dígitos
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
            return digitsOnly.Length == numero_de_digitos;
        }

        /// <summary>
        /// Metodo para generar un id unico
        /// </summary>
        /// <returns></returns>
        public static string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Generar firma HMAC-SHA1
        /// </summary>
        /// <returns></returns>
        public static string GenerateHmacSha1(string data, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(messageBytes);

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Generar la signature
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public static string GenerateTwilioSignatureChain(string url, Dictionary<string, string> body, string authToken)
        {
            // 1. Ordenar los parámetros por nombre
            var sorted = body.OrderBy(k => k.Key, StringComparer.Ordinal);

            // 2. Construir la cadena: URL + key + value
            var signatureData = new StringBuilder(url);

            foreach (var kv in sorted)
            {
                signatureData.Append(kv.Key);
                signatureData.Append(kv.Value);
            }

            // 3. Generar HMAC-SHA1 usando tu AuthToken
            var keyBytes = Encoding.UTF8.GetBytes(authToken);
            var dataBytes = Encoding.UTF8.GetBytes(signatureData.ToString());

            using var hmac = new HMACSHA1(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);

            // 4. Retornar en Base64
            return Convert.ToBase64String(hash);
        }
    }
}
