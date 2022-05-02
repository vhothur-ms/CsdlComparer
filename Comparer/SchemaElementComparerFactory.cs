using CsdlComparer.Comparer;
using Microsoft.OData.Edm;
using System;

namespace CsdlComparer
{
    public class SchemaElementComparerFactory
    {
        public ISchemaElementComparer<IEdmElement> Get(IEdmModel currentModel, IEdmModel updatedModel, IEdmSchemaElement currentElement)
        {
            switch (currentElement.SchemaElementKind)
            {
                case EdmSchemaElementKind.TypeDefinition:
                    {
                        switch ((currentElement as IEdmSchemaType).TypeKind)
                        {
                            case EdmTypeKind.Complex:
                                return new ComplexTypeComparer(currentModel, updatedModel);
                            case EdmTypeKind.Entity:
                                return new EntityTypeComparer(currentModel, updatedModel);
                            case EdmTypeKind.Enum:
                                return new EnumTypeComparer();
                            default: throw new NotImplementedException();
                        }
                    }
                case EdmSchemaElementKind.Action:
                    return new ActionComparer();
                case EdmSchemaElementKind.Function:
                    return new FuntionComparer();
                case EdmSchemaElementKind.EntityContainer:
                    return new EntityContainerComparer();
                default: throw new NotImplementedException();
            }
        }
    }
}