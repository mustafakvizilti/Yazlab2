using System.Collections.Generic;
using Yazlab2WinForms.Models;

namespace Yazlab2WinForms.Abstract
{
    public interface IGraphService
    {
        // Popülerlik ve Merkezilik Analizleri
        List<Node> EnPopulerleriBul(Graph graph);
        List<dynamic> GetTop5Influencers(Graph graph);
    }
}