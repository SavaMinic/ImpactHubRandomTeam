using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RandomName.Wave
{
    public class FanTestGenerator : MonoBehaviour
    {
        #region Public fields

        public GameObject FanArmsPrefab;
        public GameObject FanBodyPrefab;

        #endregion
        
        private EntityManager manager;

        private void Start()
        {
            manager = World.Active.GetOrCreateManager<EntityManager>();
            
            AddFans(10000, 3, 50f);
        }

        public void AddFans(int amount, float baseRadius, float maxRange)
        {
            NativeArray<Entity> entities = new NativeArray<Entity>(amount, Allocator.Temp);
            manager.Instantiate(FanArmsPrefab, entities);
            
            NativeArray<Entity> bodyEntities = new NativeArray<Entity>(amount, Allocator.Temp);
            manager.Instantiate(FanBodyPrefab, bodyEntities);

            for (int i = 0; i < amount; i++)
            {
                float xVal = baseRadius * Random.Range(-maxRange, maxRange);
                float zVal = baseRadius * Random.Range(-maxRange, maxRange);
                var pos = new float3(xVal, 0f, zVal);
                
                manager.SetComponentData(entities[i], new Position { Value = pos });
                manager.SetComponentData(entities[i], new Hands() { InitPosition = pos });
                
                manager.SetComponentData(bodyEntities[i], new Position { Value = pos });
                manager.SetComponentData(bodyEntities[i], new Body() { InitPosition = pos });
            }
            entities.Dispose();
            bodyEntities.Dispose();
        }
    }
}