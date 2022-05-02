using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsdlComparer.Comparer
{
    public class EntityContainerComparer : ISchemaElementComparer<IEdmElement>
    {
        public IEnumerable<IElementComparisionResult> Compare(IEdmElement current, IEdmElement updated)
        {
            var currentFunction = current as IEdmEntityContainer;
            var updatedFunction = updated as IEdmEntityContainer;
            var diffs = new List<IElementComparisionResult>();

            if (currentFunction is null)
                throw new ArgumentException($"current should be of type IEdmEntityContainer");
            if (updatedFunction is null)
            {
                diffs.Add(new ElementComparisionResult(currentFunction.Name, ElementType.Element, new(currentFunction.Name, null)));
                return diffs;
            }
            return diffs;
        }
    }
}
