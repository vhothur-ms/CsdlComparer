using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace CsdlComparer.Comparer
{
    public class ElementComparisionResult : IElementComparisionResult
    {
        public ElementComparisionResult(string name, ElementType type, (object, object) values)
        {
            this.Name = name;
            this.Type = type;
            this.Values = values;
        }
        public string Name { get; private set; }
        public ElementType Type { get; private set; }
        public (object, object) Values { get; private set; }
        public override string ToString()
        {
            return $"Name-{Name}, ElementType-{Type}, OldValue-{Values.Item1}, NewValue-{Values.Item2}";
        }
    }
}
