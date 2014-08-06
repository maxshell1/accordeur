using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libAccordeur
{
    public interface IPitchDetector
    {
        float DetectPitch(float[] buffer, int frames);
    }
}
