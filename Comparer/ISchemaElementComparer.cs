using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace CsdlComparer.Comparer
{
    // T can be one of types as defined in https://docs.microsoft.com/en-us/dotnet/api/microsoft.odata.edm.edmtypekind?view=odata-edm-7.0
    public interface ISchemaElementComparer<T> where T: IEdmElement
    {
        IEnumerable<IElementComparisionResult> Compare(T current, T updated);
    }
}
