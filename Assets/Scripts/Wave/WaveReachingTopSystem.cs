using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace RandomName.Wave
{
    public class WaveReachingTopSystem : JobComponentSystem
    {

        struct InteractiveFansGroup
        {
            public readonly int Length;
            public EntityArray Entities;

            [ReadOnly] public ComponentDataArray<WavingFan> Fans;
            public ComponentDataArray<InteractiveTag> InteractiveTags;
        }

        [Inject] private InteractiveFansGroup _fansGroup;

        [BurstCompile]
        struct WaveReachingTop : IJobParallelFor
        {   
            [ReadOnly] public ComponentDataArray<WavingFan> Fans;
            public ComponentDataArray<InteractiveTag> InteractiveTags;

            public float DeltaTime;

            public void Execute(int index)
            {
                var interactiveTag = InteractiveTags[index];
                if (interactiveTag.LookingForAttention < 0)
                    return;

                if (interactiveTag.IgnoreTime > 0)
                {
                    interactiveTag.IgnoreTime -= DeltaTime;
                    InteractiveTags[index] = interactiveTag;
                    if (interactiveTag.IgnoreTime > 0)
                    {
                        return;
                    }
                }
                
                var fanAmount = Fans[index].Value * 100f;
                // check if any wave is makes fan fully stand
                if (interactiveTag.WaveReachedTop <= 0 && fanAmount > 0.8f)
                {
                    interactiveTag.WaveReachedTop = 1;
                    InteractiveTags[index] = interactiveTag;
                }
                // if he already stand and now wave have passed
                else if (interactiveTag.WaveReachedTop >= 1 && fanAmount <= 0)
                {
                    interactiveTag.WavePassed = 1;
                    InteractiveTags[index] = interactiveTag;
                }
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_fansGroup.Length == 0)
            {
                return inputDeps;
            }
			
            var handle = new WaveReachingTop
            {
                Fans = _fansGroup.Fans,
                InteractiveTags = _fansGroup.InteractiveTags,
                DeltaTime = Time.deltaTime,
                // schedule it and say how many entities are there
            }.Schedule(_fansGroup.Length, 32, inputDeps);
			
            return handle;
        }
    }
}