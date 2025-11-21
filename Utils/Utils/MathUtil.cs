using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class MathUtil
    {
        /// <summary>
        /// Metodo para calcular el precio sugerido según el tipo de pedido
        /// </summary>
        /// <param name="tipo_pedido"></param>
        /// <returns></returns>
        public static int PriceCalculator(string? tipo_pedido)
        {
            switch (tipo_pedido)
            {
                case "tow_flat_bed":
                    return 100;
                case "tow_wheel_lift":
                    return 500;
                case "jump_start":
                case "jump_start_msj": 
                    return 1000;
                case "flat_tire_with_spare_tire":
                case "flat_tire_with_spare_tire_msj":
                    return 2000;
                case "lock_out_key":
                case "lock_out_key_msj":
                    return 3000;
                case "delivery_gasoline":
                case "delivery_gasoline_msj":
                    return 4000;
                case "winch_out":
                case "winch_out_msj":
                    return 5000;
                default:
                    return 0;
            }

        }
    }
}
