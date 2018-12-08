using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using RandomName.Wave;

public class HandsSystem : JobComponentSystem {

    [BurstCompile]
    struct HandsJob : IJobProcessComponentData<Position, Rotation, Hands, WavingFan> {

        public void Execute(ref Position position, ref Rotation rotation, [ReadOnly]ref Hands hands, [ReadOnly]ref WavingFan wavingFan)
        {
            var init = hands.InitRotationEuler;
            rotation.Value = quaternion.Euler(init.x, init.y, init.z + wavingFan.Value);
            position.Value.y = hands.InitPosition.y + wavingFan.Value * 3f;
        }

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new HandsJob() {};
        return job.Schedule(this, inputDeps);
    }

}
