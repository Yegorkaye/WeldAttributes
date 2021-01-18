using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace WeldAttributes
{
    static class AttributeWriter
    {
        private enum AssemblyType
        {
            Mark,
            SubAssembly,
        }

        public static void Write()
        {
            var welds = GetSelectesWelds();
            foreach (var weld in welds)
            {
                var firstPart = weld.MainObject as Part;
                var secondPart = weld.SecondaryObject as Part;

                var firstAssembly = firstPart.GetAssembly();
                var secondAssembly = secondPart.GetAssembly();

                var assType = firstAssembly.CompareTo(secondAssembly) ? AssemblyType.Mark : AssemblyType.SubAssembly;
                var isMark = assType == AssemblyType.Mark;

                var actualAssembly = isMark ? firstAssembly : firstAssembly.GetAssembly();

                var drawingNumber = "";
                var assPos = "";
                var drawingPropName = "";
                var assPosPropName = "ASSEMBLY_POS";

                if (isMark)
                    drawingPropName = "DWG_N";
                else
                    drawingPropName = "SA_DWG_N";

                firstPart.GetUserProperty(drawingPropName, ref drawingNumber);
                actualAssembly.GetReportProperty(assPosPropName, ref assPos);

                weld.SetUserProperty("W_DWG", drawingNumber);
                weld.SetUserProperty("W_ASSEMBLY", assPos);
            }
        }

        private static List<BaseWeld> GetSelectesWelds()
        {
            var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var selectedObjects = selector.GetSelectedObjects();
            var result = new List<BaseWeld>();

            while (selectedObjects.MoveNext())
            {
                var currentObject = selectedObjects.Current;
                if (currentObject is BaseWeld currentWeld)
                {
                    result.Add(currentWeld);
                }
            }

            return result;
        }
    }
}
