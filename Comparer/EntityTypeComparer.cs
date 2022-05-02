using CsdlComparer.Comparer;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsdlComparer
{
    public class EntityTypeComparer : ISchemaElementComparer<IEdmElement>
    {
        private IEdmModel currentModel;
        private IEdmModel updatedModel;

        public EntityTypeComparer(IEdmModel currentModel, IEdmModel updatedModel)
        {
            this.currentModel = currentModel;
            this.updatedModel = updatedModel;
        }

        public IEnumerable<IElementComparisionResult> Compare(IEdmElement currentElement, IEdmElement updatedElement)
        {
            var currentEntity = currentElement as IEdmEntityType;
            var updatedEntity = updatedElement as IEdmEntityType;
            var diffs = new List<IElementComparisionResult>();

            if (currentEntity is null)
                throw new ArgumentException($"current should be of type IEdmEntityType");
            if (updatedEntity is null)
            {
                diffs.Add(new ElementComparisionResult(currentEntity.Name, ElementType.Element, new(currentEntity.Name, null)));
                return diffs;
            }

            // find differences in high level properties
            if (currentEntity.Namespace != updatedEntity.Namespace)
            {
                diffs.Add(new ElementComparisionResult(currentEntity.Name, ElementType.Element, new(currentEntity.Namespace, updatedEntity.Namespace)));
            }

            // find differences in properties
            diffs.AddRange(EnumeratePropertyDiffs(currentEntity, updatedEntity, out HashSet<IEdmProperty> visitedProperties));
            // find any properties that are defined in updated but not in current
            foreach(var newUpdatedEntityProperty in updatedEntity.Properties().Where(x => visitedProperties.All(visited => visited.Name != x.Name)))
            {
                diffs.Add(new ElementComparisionResult(currentEntity.Name, ElementType.Annotation, new(currentEntity.Name, null)));
            }

            // find differences in annotations
            diffs.AddRange(EnumerateAnnotationDiffs(currentEntity, updatedEntity));
            return diffs;
        }

        private IEnumerable<IElementComparisionResult> EnumerateAnnotationDiffs(IEdmEntityType currentEntity, IEdmEntityType updatedEntity)
        {
            // read annotations for entity
            var currentAnnotations = currentModel.DirectValueAnnotations(currentEntity);
            var updatedAnnotations = updatedModel.DirectValueAnnotations(updatedEntity);

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

        private IEnumerable<IElementComparisionResult> EnumeratePropertyDiffs(IEdmEntityType currentEntity, IEdmEntityType updatedEntity, out HashSet<IEdmProperty> visitedProperties)
        {
            var diffs = new List<IElementComparisionResult>();
            visitedProperties = new HashSet<IEdmProperty>();
            var propertyComparer = new PropertyComparer();
            foreach (var currentEntityProperty in currentEntity.Properties())
            {
                visitedProperties.Add(currentEntityProperty);
                // find matching property in updated entity and compare
                var updatedEntityProperty = updatedEntity.FindProperty(currentEntityProperty.Name);

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
