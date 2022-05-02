using Microsoft.OData.Edm;

namespace CsdlComparer.Comparer
{
    public interface IElementComparisionResult
    {
        string Name { get; }
        ElementType Type { get; }
        (object, object) Values { get; }
    }
}
