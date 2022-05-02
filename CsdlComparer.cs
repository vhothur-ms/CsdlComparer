using CsdlComparer.Comparer;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsdlComparer
{
    public class CsdlComparer
    {
        private SchemaElementComparerFactory comparerfactory;

        public CsdlComparer(SchemaElementComparerFactory factory)
        {
            this.comparerfactory = factory;
        }

        public List<IElementComparisionResult> Compare(string current, string updated)
        {
            var currentCsdl = new EdmModelReader().Get(current);
            var updatedCsdl = new EdmModelReader().Get(updated);

            return CompareCsdl(currentCsdl, updatedCsdl);
        }
        
        private List<IElementComparisionResult> CompareCsdl(IEdmModel current, IEdmModel updated)
        {
            var comparsionResult = new List<IElementComparisionResult>();
            foreach(var currentSchemaElement in current.SchemaElements)
            {
                var updatedSchemaElement = updated.SchemaElements.FirstOrDefault(x => x.Name == currentSchemaElement.Name);
                if (updatedSchemaElement != null)
                {
                    comparsionResult.AddRange(this.comparerfactory.Get(current, updated, currentSchemaElement).Compare(currentSchemaElement, updatedSchemaElement));
                }
                else
                {
                    Console.WriteLine($"Missing element name - {currentSchemaElement.Name}, type - {currentSchemaElement.SchemaElementKind}");
                }
                /*
                if (currentSchemaElement.SchemaElementKind == EdmSchemaElementKind.TypeDefinition)
                {
                    var currentElement = (currentSchemaElement as IEdmType);
                    var updatedElement = updated.FindDeclaredType(currentSchemaElement.FullName());
                    comparsionResult.AddRange(this.comparerfactory.Get(current, updated, currentSchemaElement.SchemaElementKind).Compare(currentElement, updatedElement));
                }
                else if (currentSchemaElement.SchemaElementKind == EdmSchemaElementKind.Action)
                    Console.WriteLine($"Element namespace - {currentSchemaElement.Namespace} name - {currentSchemaElement.Name}, parameters - {(currentSchemaElement as IEdmAction).Parameters}");
                else if (currentSchemaElement.SchemaElementKind == EdmSchemaElementKind.Function)
                    Console.WriteLine($"Element namespace - {currentSchemaElement.Namespace} name - {currentSchemaElement.Name}, parameters - {(currentSchemaElement as IEdmAction).Parameters}");
                else if (currentSchemaElement.SchemaElementKind == EdmSchemaElementKind.EntityContainer)
                    Console.WriteLine($"Element namespace - {currentSchemaElement.Namespace} name - {currentSchemaElement.Name}, elements - {(currentSchemaElement as IEdmEntityContainer).Elements}");
                */
            }
            return comparsionResult;
        }
    }
}
