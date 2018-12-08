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

        public GameObject FanPrefab;

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
            manager.Instantiate(FanPrefab, entities);

            for (int i = 0; i < amount; i++)
            {
                float xVal = baseRadius * Random.Range(-maxRange, maxRange);
                float zVal = baseRadius * Random.Range(-maxRange, maxRange);
                manager.SetComponentData(entities[i], new Position { Value = new float3(xVal, 0f, zVal) });
                manager.SetComponentData(entities[i], new Scale { Value = new float3(1f, 1f, 1f) });
            }
            entities.Dispose();
        }
    }
}