using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
            waveTestSystem.Enabled = false;

            for (int level = 0; level < 15; level++)
            {
                //for (int j = 0; j < 2; j++)
                //{
                    AddWave(level, Random.Range(-90, 90), 2f, float3.zero);
                //}
            }
        }

        #endregion

        #region Public

        public void AddWave(int level, int startAngle, float speed, float3 center)
        {
            var wave = manager.Instantiate(WavePrefab);
            manager.SetComponentData(wave, new Wave
            {
                Angle = startAngle, Speed = speed, Level = level, Center = center,
                Radius = GameSettings.I.GetRadiusPerLevel(level)
            });
        }

        #endregion
    }
}