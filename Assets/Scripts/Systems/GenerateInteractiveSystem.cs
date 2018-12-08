using RandomName.Wave;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace RandomName.Interactive
{
    public class GenerateInteractiveSystem : JobComponentSystem
    {
        struct FansGroup
        {
            public readonly int Length;
            public EntityArray Entities;
			
            public ComponentDataArray<WavingFan> Fans;
            public ComponentDataArray<Position> FansPositions;
            public ComponentDataArray<SeatingData> FansSeatings;
        }
        
        [Inject] private FansGroup _fansGroup;

        [BurstCompile]
        struct WavingJob : IJobParallelFor
        {
            public void Execute(int index)
            {
                
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_fansGroup.Length == 0)
            {
                return inputDeps;
            }
			
            var handle = new WavingJob
            {
                // schedule it and say how many entities are there
            }.Schedule(_fansGroup.Length, 32, inputDeps);
			
            return handle;
        }
    }
}