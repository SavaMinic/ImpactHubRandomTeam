using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using RandomName.Wave;

public class LegsSystem : JobComponentSystem {

    [BurstCompile]
    struct LegsJob : IJobProcessComponentData<Position, Rotation, Legs, WavingFan> {

        public void Execute(ref Position position, ref Rotation rotation, [ReadOnly]ref Legs legs, [ReadOnly]ref WavingFan wavingFan) {
            rotation.Value = quaternion.Euler(0, 0, wavingFan.Value);
            position.Value.y = legs.InitPosition.y + wavingFan.Value + wavingFan.Value / 2;
            position.Value.x = wavingFan.Value / 4;
        }

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new LegsJob() { };
        return job.Schedule(this, inputDeps);
    }

}
