using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
