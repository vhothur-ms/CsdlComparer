using CsdlComparer.Comparer;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsdlComparer
{
    public class ActionComparer : ISchemaElementComparer<IEdmElement>
    {
        public IEnumerable<IElementComparisionResult> Compare(IEdmElement currentElement, IEdmElement updatedElement)
        {
            var currentAction = currentElement as IEdmAction;
            var updatedAction = updatedElement as IEdmAction;
            var diffs = new List<IElementComparisionResult>();

            if (currentAction is null)
                throw new ArgumentException($"current should be of type IEdmAction");
            if (updatedAction is null)
            {
                diffs.Add(new ElementComparisionResult(currentAction.Name, ElementType.Action, new(currentAction.Name, null)));
                return diffs;
            }

            if (currentAction.IsBound != updatedAction.IsBound)
            {
                diffs.Add(new ElementComparisionResult(currentAction.Name, ElementType.Action, new($"IsBound-{currentAction.IsBound}", $"IsBound-{updatedAction.IsBound}")));
            }

            if (currentAction.ReturnType != updatedAction.ReturnType)
            {
                diffs.Add(new ElementComparisionResult(currentAction.Name, ElementType.Action, new($"ReturnType-{currentAction.ReturnType}", $"ReturnType-{updatedAction.ReturnType}")));
            }

            foreach(var currentParameter in currentAction.Parameters)
            {
                var updatedParameter = updatedAction.Parameters.Where(x => x.Name == currentParameter.Name).FirstOrDefault();
                if (updatedParameter == null)
                {
                    diffs.Add(new ElementComparisionResult(currentAction.Name, ElementType.Action, new(currentParameter.Name, null)));
                }
                else if (currentParameter.Type != updatedParameter.Type)
                {
                    diffs.Add(new ElementComparisionResult(currentAction.Name, ElementType.Action, new($"{currentParameter.Name}-{currentParameter.Type}", $"{updatedParameter.Name}-{updatedParameter.Type}")));
                }
            }

            return diffs;
        }
    }
}