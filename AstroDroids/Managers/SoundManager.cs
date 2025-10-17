using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Managers
{
    public class SoundManager
    {
        static bool initialized;
        public static void Initialize()
        {
            if (initialized) return;

            initialized = true;
        }
    }
}
