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
    struct LegsJob : IJobProcessComponentData<Position, Rotation, Legs, WavingFan>
    {

        public float debug1;
        public float debug2;
        public void Execute(ref Position position, ref Rotation rotation, [ReadOnly]ref Legs hands, [ReadOnly]ref WavingFan wavingFan)
        {
            position.Value.y = hands.InitPosition.y + wavingFan.Value * 300f;
            rotation.Value = math.mul(hands.InitRotation,
                quaternion.Euler(math.lerp(debug1, debug2, 2 * wavingFan.Value), 0, 0));
        }

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new LegsJob() { debug1 = GameSettings.I.debug1, debug2 = GameSettings.I.debug2};
        return job.Schedule(this, inputDeps);
    }

}
