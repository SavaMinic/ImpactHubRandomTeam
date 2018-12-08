using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RandomName.Wave
{
    public class WaveGenerator : MonoBehaviour
    {
        #region Static
		
        public static WaveGenerator I { get; private set; }
		
        #endregion

        #region Public fields

        public GameObject WavePrefab;

        #endregion

        #region Fields

        private EntityManager manager;
        private WaveTestSystem waveTestSystem;

        #endregion

        #region Mono

        void Awake()
        {
            I = this;
        }
		
        void Start()
        {
            manager = World.Active.GetOrCreateManager<EntityManager>();

            waveTestSystem = World.Active.GetExistingManager<WaveTestSystem>();
            waveTestSystem.Enabled = true;
            
            AddWave(0, 0, 5f, float3.zero);
            AddWave(0, 90, 5f, float3.zero);
            AddWave(0, 0, -3f, float3.zero);
            AddWave(0, 0, -1f, float3.zero);
            AddWave(0, 180, 2f, float3.zero);
        }

        #endregion

        #region Public

        public void AddWave(int level, int startAngle, float speed, float3 center)
        {
            var wave = manager.Instantiate(WavePrefab);
            manager.SetComponentData(wave, new Wave
            {
                Angle = startAngle, Speed = speed, Level = level, Center = center
            });
        }

        #endregion
    }
}