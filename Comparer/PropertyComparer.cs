using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace CsdlComparer.Comparer
{
    public class PropertyComparer : ISchemaElementComparer<IEdmElement>
    {
        public IEnumerable<IElementComparisionResult> Compare(IEdmElement currentProperty, IEdmElement updatedProperty)
        {
            var currentEntityProperty = currentProperty as IEdmProperty;
            var updatedEntityProperty = updatedProperty as IEdmProperty;
            var diffs = new List<IElementComparisionResult>();

            if (currentEntityProperty is null)
                throw new ArgumentException($"current should be of type IEdmProperty");
            if (updatedEntityProperty is null)
            {
                diffs.Add(new ElementComparisionResult(currentEntityProperty.Name, ElementType.Element, new(currentEntityProperty.Name, null)));
                return diffs;
            }

            
            if (currentEntityProperty.PropertyKind != updatedEntityProperty.PropertyKind)
            {
                diffs.Add(new ElementComparisionResult(currentEntityProperty.Name, ElementType.PropertyKind, new(currentEntityProperty.PropertyKind, updatedEntityProperty.PropertyKind)));
            }

            if (currentEntityProperty.Type.FullName() != updatedEntityProperty.Type.FullName())
            {
                diffs.Add(new ElementComparisionResult(currentEntityProperty.Name, ElementType.PropertyType, new(currentEntityProperty.Type, updatedEntityProperty.Type)));
            }
            
            if (string.Compare(updatedEntityProperty.ToTraceString(), currentEntityProperty.ToTraceString()) != 0)
            {
                diffs.Add(new ElementComparisionResult(currentEntityProperty.Name, ElementType.Property, new(currentEntityProperty.ToTraceString(), updatedEntityProperty.ToTraceString())));
            }

            return diffs;
        }
    }
}
