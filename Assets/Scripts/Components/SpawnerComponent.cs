using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct Spawner : ISharedComponentData {

	public int count;
	public float innerRadius;
	public float outerRadius;
	public float startingHeight;
	public int rows;
	public Mesh mesh;
	public Material material;

}

public class SpawnerComponent : SharedComponentDataWrapper<Spawner> { }