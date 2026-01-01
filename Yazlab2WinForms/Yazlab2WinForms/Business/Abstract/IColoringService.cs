using System.Collections.Generic;
using System.Drawing;
using Yazlab2WinForms.Models;

namespace Yazlab2WinForms.Abstract
{
    public interface IColoringService
    {
        // Welsh-Powell renklendirme
        Dictionary<string, Color> ApplyWelshPowell(Graph graph);
    }
}