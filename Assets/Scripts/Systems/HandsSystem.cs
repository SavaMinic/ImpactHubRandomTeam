using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using RandomName.Wave;
using UnityEngine;

public class HandsSystem : JobComponentSystem {

    [BurstCompile]
    struct HandsJob : IJobProcessComponentData<Rotation, Position, Hands, WavingFan>
    {   
        public void Execute(ref Rotation rotation, [ReadOnly] ref Position position, [ReadOnly]ref Hands hands, [ReadOnly]ref WavingFan wavingFan)
        {
            rotation.Value = Quaternion.Euler(math.lerp(0f, -90f, wavingFan.Value), 0, 0);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new HandsJob();
        return job.Schedule(this, inputDeps);
    }

}
