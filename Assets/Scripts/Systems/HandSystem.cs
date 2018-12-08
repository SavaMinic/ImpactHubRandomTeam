using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;
using RandomName.Wave;

public class HandSystem : JobComponentSystem {

    [BurstCompile]
    struct HandJob : IJobProcessComponentData<Position, Rotation, Hand, WavingFan> {

        public void Execute(ref Position position, ref Rotation rotation, [ReadOnly]ref Hand hand, [ReadOnly]ref WavingFan wavingFan) {
            position.Value.y = hand.InitPosition.y + wavingFan.Value;
        }

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new HandJob() {};
        return job.Schedule(this, inputDeps);
    }

}
