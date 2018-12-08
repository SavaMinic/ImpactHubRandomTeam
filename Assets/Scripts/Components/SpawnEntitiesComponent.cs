using Unity.Entities;
using UnityEngine;

public struct SpawnEntitiesComponent : IComponentData
{
	public int Count;
	
	public float InnerRadius;
	public float OuterRadius;
	public int Rows;
}
