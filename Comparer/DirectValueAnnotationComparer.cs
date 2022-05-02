using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsdlComparer.Comparer
{
    public class DirectValueAnnotationComparer : ISchemaElementComparer<IEdmElement>
    {
        public IEnumerable<IElementComparisionResult> Compare(IEdmElement current, IEdmElement updated)
        {
            var currentAnnotation = current as IEdmDirectValueAnnotation;
            var updatedAnnotation = updated as IEdmDirectValueAnnotation;
            var diffs = new List<IElementComparisionResult>();

            if (currentAnnotation is null)
                throw new ArgumentException($"current should be of type IEdmDirectValueAnnotation");
            if (updatedAnnotation is null)
            {
                diffs.Add(new ElementComparisionResult(currentAnnotation.Name, ElementType.Element, new(currentAnnotation.Name, null)));
                return diffs;
            }

            if (currentAnnotation.NamespaceUri != updatedAnnotation.NamespaceUri)
            {
                diffs.Add(new ElementComparisionResult(currentAnnotation.Name, ElementType.AnnotationNamespaceUrl, new(currentAnnotation.NamespaceUri, updatedAnnotation.NamespaceUri)));
            }
            //if (currentAnnotation.Value != updatedAnnotation.Value)
            //{
            //    diffs.Add(new ElementComparisionResult(currentAnnotation.Name, ElementType.AnnotationValue, new(currentAnnotation.Value, updatedAnnotation.Value)));
            //}
            var currentAnnotationType = currentAnnotation.Value.GetType();
            foreach (var propertyInfo in currentAnnotationType.GetProperties())
            {
                var currentAnnotationPropertyValue = propertyInfo.GetValue(currentAnnotation.Value);
                var updatedAnnotationPropertyValue = propertyInfo.GetValue(updatedAnnotation.Value);
                if (updatedAnnotationPropertyValue == null)
                {
                    diffs.Add(new ElementComparisionResult(propertyInfo.Name, ElementType.Annotation, new(currentAnnotationPropertyValue, updatedAnnotationPropertyValue)));
                }
                else if (currentAnnotationPropertyValue.ToString() != updatedAnnotationPropertyValue.ToString())
                {
                    diffs.Add(new ElementComparisionResult(propertyInfo.Name, ElementType.Annotation, new(currentAnnotationPropertyValue, updatedAnnotationPropertyValue)));
                }
            }

            var newPropertiesInUpdated = updatedAnnotation.Value.GetType().GetProperties().Where(updatedProp => currentAnnotationType.GetProperties().All(currentProp => updatedProp.Name != currentProp.Name));
            foreach (var newUpdatedProperty in newPropertiesInUpdated)
            {
                var updatedAnnotationPropertyValue = newUpdatedProperty.GetValue(updatedAnnotation.Value);
                diffs.Add(new ElementComparisionResult(newUpdatedProperty.Name, ElementType.Annotation, new(null, updatedAnnotationPropertyValue)));
            }

            return diffs;
        }
    }
}
