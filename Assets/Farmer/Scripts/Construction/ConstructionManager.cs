using System;
using System.Collections.Generic;
namespace Farmer
{
    public static class ConstructionManager
    {
        private static readonly Dictionary<string, List<ConstructionEntity>> ConstructionsById = new();

        public static event Action<ConstructionEntity> ConstructionRegistered;

        public static void Register(ConstructionEntity entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.Definition.id))
                return;

            if (!ConstructionsById.TryGetValue(entity.Definition.id, out var entities))
            {
                entities = new List<ConstructionEntity>();
                ConstructionsById.Add(entity.Definition.id, entities);
            }

            if (entities.Contains(entity))
                return;

            entities.Add(entity);
            ConstructionRegistered?.Invoke(entity);
        }

        public static void Unregister(ConstructionEntity entity)
        {
            if (entity == null || !ConstructionsById.TryGetValue(entity.Definition.id, out var entities))
                return;

            entities.Remove(entity);
            if (entities.Count == 0)
                ConstructionsById.Remove(entity.Definition.id);
        }

        public static IReadOnlyList<ConstructionEntity> GetByDefinitionId(string definitionId)
        {
            return ConstructionsById.TryGetValue(definitionId, out var entities)
                ? entities
                : Array.Empty<ConstructionEntity>();
        }
    }
}
