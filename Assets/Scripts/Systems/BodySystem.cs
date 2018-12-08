using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;
using RandomName.Wave;

public class BodySystem : JobComponentSystem {

    [BurstCompile]
    struct BodyJob : IJobProcessComponentData<Position, Body, WavingFan> {

        public void Execute(ref Position position, [ReadOnly]ref Body body, [ReadOnly]ref WavingFan wavingFan) {
            position.Value.y = body.InitPosition.y + wavingFan.Value * 100f;
        }

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new BodyJob() {};
        return job.Schedule(this, inputDeps);
    }

}
