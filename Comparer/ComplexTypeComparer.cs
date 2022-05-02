using CsdlComparer.Comparer;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsdlComparer
{
    public class ComplexTypeComparer : ISchemaElementComparer<IEdmElement>
    {
        private IEdmModel currentModel;
        private IEdmModel updatedModel;

        public ComplexTypeComparer(IEdmModel currentModel, IEdmModel updatedModel)
        {
            this.currentModel = currentModel;
            this.updatedModel = updatedModel;
        }

        public IEnumerable<IElementComparisionResult> Compare(IEdmElement currentElement, IEdmElement updatedElement)
        {
            var currentComplexType = currentElement as IEdmComplexType;
            var updatedComplexType = updatedElement as IEdmComplexType;
            var diffs = new List<IElementComparisionResult>();

            if (currentComplexType is null)
                throw new ArgumentException($"current should be of type IEdmComplexType");
            if (updatedComplexType is null)
            {
                diffs.Add(new ElementComparisionResult(currentComplexType.Name, ElementType.ComplexType, new(currentComplexType.Name, null)));
                return diffs;
            }

            if (currentComplexType.Namespace != updatedComplexType.Namespace)
            {
                diffs.Add(new ElementComparisionResult(currentComplexType.Name, ElementType.ComplexType, new(currentComplexType.Namespace, updatedComplexType.Namespace)));
            }

            // find differences in properties
            diffs.AddRange(EnumeratePropertyDiffs(currentComplexType, updatedComplexType, out HashSet<IEdmProperty> visitedProperties));
            // find any properties that are defined in updated but not in current
            foreach (var newUpdatedEntityProperty in updatedComplexType.Properties().Where(x => visitedProperties.All(visited => visited.Name != x.Name)))
            {
                diffs.Add(new ElementComparisionResult(currentComplexType.Name, ElementType.Annotation, new(currentComplexType.Name, null)));
            }

            // find differences in annotations
            diffs.AddRange(EnumerateAnnotationDiffs(currentComplexType, updatedComplexType));
            return diffs;
        }
        private IEnumerable<IElementComparisionResult> EnumerateAnnotationDiffs(IEdmComplexType currentComplexType, IEdmComplexType updatedComplexType)
        {
            // read annotations for entity
            var currentAnnotations = currentModel.DirectValueAnnotations(currentComplexType);
            var updatedAnnotations = updatedModel.DirectValueAnnotations(updatedComplexType);

            var diffs = new List<IElementComparisionResult>();
            var visitedAnnotations = new HashSet<IEdmDirectValueAnnotation>();
            var annotationComparer = new DirectValueAnnotationComparer();

            foreach (var currentAnnotation in currentAnnotations)
            {
                visitedAnnotations.Add(currentAnnotation);
                // find corresponding annotation in the property on the updated
                var updatedAnnotationList = updatedAnnotations.Where(x => x.Name == currentAnnotation.Name);
                if (!updatedAnnotationList.Any())
                {
                    diffs.Add(new ElementComparisionResult(currentAnnotation.Name, ElementType.Annotation, new(currentAnnotation.Name, null)));
                }
                else
                {
                    var updatedAnnotation = updatedAnnotationList.First();
                    diffs.AddRange(annotationComparer.Compare(currentAnnotation, updatedAnnotation));
                }
            }

            // find any annotations that are defined in updated but not in current
            foreach (var newUpdatedEntityAnnotation in updatedAnnotations.Where(x => visitedAnnotations.All(visited => visited.Name != x.Name)))
            {
                diffs.Add(new ElementComparisionResult(newUpdatedEntityAnnotation.Name, ElementType.Annotation, new(null, newUpdatedEntityAnnotation.Value)));
            }

            return diffs;
        }

        private IEnumerable<IElementComparisionResult> EnumeratePropertyDiffs(IEdmComplexType currentComplexType, IEdmComplexType updatedComplexType, out HashSet<IEdmProperty> visitedProperties)
        {
            var diffs = new List<IElementComparisionResult>();
            visitedProperties = new HashSet<IEdmProperty>();
            var propertyComparer = new PropertyComparer();
            foreach (var currentEntityProperty in currentComplexType.Properties())
            {
                visitedProperties.Add(currentEntityProperty);
                // find matching property in updated entity and compare
                var updatedEntityProperty = updatedComplexType.FindProperty(currentEntityProperty.Name);

                if (updatedEntityProperty == null)
                {
                    diffs.Add(new ElementComparisionResult(currentEntityProperty.Name, ElementType.Property, new(currentEntityProperty.Name, null)));
                }
                else
                {
                    diffs.AddRange(propertyComparer.Compare(currentEntityProperty, updatedEntityProperty));
                }
            }
            return diffs;
        }
    }
}
