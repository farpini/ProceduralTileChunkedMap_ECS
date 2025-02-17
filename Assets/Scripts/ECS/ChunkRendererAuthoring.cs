﻿/*
 * Written by Fernando Arpini Ferretto
 * https://github.com/farpini/ProceduralTileChunkedMap_ECS
 */

using NUnit.Framework.Internal;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ChunkRendererAuthoring : MonoBehaviour
{
    private class ChunkRenderer : Baker<ChunkRendererAuthoring>
    {
        public override void Bake (ChunkRendererAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
        }
    }
}