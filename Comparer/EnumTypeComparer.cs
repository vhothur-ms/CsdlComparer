using CsdlComparer.Comparer;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsdlComparer
{
    public class EnumTypeComparer : ISchemaElementComparer<IEdmElement>
    {
        public IEnumerable<IElementComparisionResult> Compare(IEdmElement currentElement, IEdmElement updatedElement)
        {
            var currentEnum = currentElement as IEdmEnumType;
            var updatedEnum = updatedElement as IEdmEnumType;
            var diffs = new List<IElementComparisionResult>();

            if (currentEnum is null)
                throw new ArgumentException($"current should be of type IEdmEnumType");
            if (updatedEnum is null)
            {
                diffs.Add(new ElementComparisionResult(currentEnum.Name, ElementType.Enum, new(currentEnum.Name, null)));
                return diffs;
            }

            foreach(var currentEnumMember in currentEnum.Members)
            {
                var updatedEnumMember = updatedEnum.Members.Where(x => x.Name == currentEnumMember.Name).FirstOrDefault();
                if (updatedEnumMember == null)
                {
                    diffs.Add(new ElementComparisionResult(currentEnum.Name, ElementType.Enum, new(currentEnumMember.Name, null)));
                }
                else if (currentEnumMember.Value.Value != updatedEnumMember.Value.Value)
                {
                    diffs.Add(new ElementComparisionResult(currentEnum.Name, ElementType.Enum, new(currentEnumMember.Value.Value, updatedEnumMember.Value.Value)));
                }
            }

            return diffs;
        }
    }
}
