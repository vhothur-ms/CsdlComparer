using CsdlComparer.Comparer;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsdlComparer
{
    public class FuntionComparer : ISchemaElementComparer<IEdmElement>
    {
        public IEnumerable<IElementComparisionResult> Compare(IEdmElement currentElement, IEdmElement updatedElement)
        {
            var currentFunction = currentElement as IEdmFunction;
            var updatedFunction = updatedElement as IEdmFunction;
            var diffs = new List<IElementComparisionResult>();

            if (currentFunction is null)
                throw new ArgumentException($"current should be of type IEdmFunction");
            if (updatedFunction is null)
            {
                diffs.Add(new ElementComparisionResult(currentFunction.Name, ElementType.Element, new(currentFunction.Name, null)));
                return diffs;
            }

            if (currentFunction.IsBound != updatedFunction.IsBound)
            {
                diffs.Add(new ElementComparisionResult(currentFunction.Name, ElementType.Element, new(currentFunction.IsBound, updatedFunction.IsBound)));
            }

            if (currentFunction.ReturnType != updatedFunction.ReturnType)
            {
                diffs.Add(new ElementComparisionResult(currentFunction.Name, ElementType.Element, new(currentFunction.ReturnType, updatedFunction.ReturnType)));
            }

            foreach (var currentParameter in currentFunction.Parameters)
            {
                var updatedParameter = updatedFunction.Parameters.Where(x => x.Name == currentParameter.Name).FirstOrDefault();
                if (updatedParameter == null)
                {
                    diffs.Add(new ElementComparisionResult(currentFunction.Name, ElementType.Element, new(currentParameter.Name, null)));
                }
                else if (currentParameter.Type != updatedParameter.Type)
                {
                    diffs.Add(new ElementComparisionResult(currentFunction.Name, ElementType.Element, new(currentParameter.Type, updatedParameter.Type)));
                }
            }

            return diffs;
        }
    }
}