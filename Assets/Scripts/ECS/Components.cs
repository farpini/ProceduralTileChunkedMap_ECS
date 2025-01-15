using BlobHashMaps;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public struct ControllerComponent : IComponentData
{
    public ControllerState State;
    public bool OnRectSelecting;
    public int4 Rect;
    public int ModelCount;
    public int ModelSelectedId;
    public int FloorSelectedTextureId;
    public int2 StartTile;
}

public class RefGameObject : IComponentData
{
    public Map Map;
    public RectView RectView;
}

[InternalBufferCapacity(64)]
public struct RectChunkEntityBuffer : IBufferElementData
{
    public static implicit operator RectChunkData (RectChunkEntityBuffer e) { return e.Value; }
    public static implicit operator RectChunkEntityBuffer (RectChunkData e) { return new RectChunkEntityBuffer { Value = e }; }

    public RectChunkData Value;
}

public struct RectChunkData
{
    public int2 chunkPosition;
    public int4 chunkRect;
}

public struct TerrainComponent : IComponentData
{
}

public struct MapComponent : IComponentData
{
    public int2 TileDimension;
    public int TileWidth;
    public int2 ChunkDimension;
    public int ChunkWidth;
    public int MaxHeight;
    public int MaxDepth;
    public int Roughness;

    public float HalfTileWidth => TileWidth * 0.5f;
    public float2 UnitDimension => new float2 (TileDimension.x, TileDimension.y) * TileWidth;
    public bool Validate => (ChunkWidth != 0 && (TileDimension.x % ChunkWidth == 0) && (TileDimension.y % ChunkWidth == 0));

    public int GetTileIndexFromTilePosition (int2 tilePosition)
    {
        return tilePosition.x * TileDimension.y + tilePosition.y;
    }

    public int2 GetChunkFromTilePosition (int2 tilePosition)
    {
        return new int2(tilePosition.x / ChunkWidth, tilePosition.y / ChunkWidth);
    }

    public int GetChunkIndexFromChunkPosition (int2 chunkPosition)
    {
        return chunkPosition.x * ChunkDimension.y + chunkPosition.y;
    }

    public bool IsTileIndexValid (int tileIndex)
    {
        return tileIndex >= 0 && (tileIndex < (TileDimension.x * TileDimension.y));
    }

    public int GetHeigthMapIndexFromHeightMapPosition (int2 heighMapPosition)
    {
        return heighMapPosition.x * (TileDimension.y + 1) + heighMapPosition.y;
    }
}

public struct MapTileComponent : IComponentData, IDisposable
{
    public NativeArray<TileData> TileData;
    public NativeArray<int> TileHeightMap;

    public void Dispose ()
    {
        if (TileData.IsCreated) TileData.Dispose();
        if (TileHeightMap.IsCreated) TileHeightMap.Dispose();
    }
}

public struct TileData
{
    public int terrainType;
    public int modelType;
    public int terrainLevel;
    public float terrainHeight;
}

public enum TileTerrainType
{
    Flat = 0,
    Saddle_0 = 1,
    Saddle_1 = 2,
    Ramp_0 = 3,
    Ramp_1 = 4,
    Ramp_2 = 5,
    Ramp_3 = 6,
    H1_0 = 7,
    H1_1 = 8,
    H1_2 = 9,
    H1_3 = 10,
    H3_0 = 11,
    H3_1 = 12,
    H3_2 = 13,
    H3_3 = 14,
    Steep_0 = 15,
    Steep_1 = 16,
    Steep_2 = 17,
    Steep_3 = 18
}

public enum ControllerState
{
    None, CreateModel, RemoveModel, LowerTerrain, RaiseTerrain, LevelTerrain, GenerateTerrain
}

public struct RendererPrefabEntities : IComponentData
{
    public Entity chunkModelRenderer;
    public Entity chunkTileRenderer;
    public Entity tilePrefab;
}

public struct ModelDataEntityBuffer : IBufferElementData
{
    public static implicit operator Entity (ModelDataEntityBuffer e) { return e.Value; }
    public static implicit operator ModelDataEntityBuffer (Entity e) { return new ModelDataEntityBuffer { Value = e }; }

    public Entity Value;
}

[InternalBufferCapacity(64)]
public struct ChunkRendererEntityBuffer : IBufferElementData
{
    public static implicit operator Entity (ChunkRendererEntityBuffer e) { return e.Value; }
    public static implicit operator ChunkRendererEntityBuffer (Entity e) { return new ChunkRendererEntityBuffer { Value = e }; }

    public Entity Value;
}

[Serializable]
public class MeshChunkData : IComponentData, IDisposable 
{
    public Entity entity;
    public int meshModelId;
    public int2 chunkPosition;
    public NativeHashMap<int, int> mapping; // modelIndex -> modelId
    public NativeHashMap<int, int> invMapping; // modelId -> modelIndex

    public void Dispose ()
    {
        if (mapping.IsCreated) mapping.Dispose();
    }
}

public struct ModelDataComponent : IComponentData
{
    public int modelId;
}

public struct MeshBlobInfoComponent : ISharedComponentData
{
    public BlobAssetReference<MeshBlobInfo> meshInfoBlob;
    public int vertexCount;
    public int indexCount;
    public int vertexAttributeDimension;
}

public struct MeshBlobInfo
{
    public BlobArray<char> meshName;
    public BlobArray<VertexAttributeDescriptor> attributes;
}

public struct MeshBlobDataComponent : ISharedComponentData
{
    public BlobAssetReference<MeshBlobData> meshDataBlob;
}

public struct MeshBlobData
{
    public BlobArray<float3> vertexes;
    public BlobArray<uint> indexes;
}

public struct MeshBlobTileTerrainMappingComponent : ISharedComponentData
{
    public BlobAssetReference<BlobHashMap<int4, int>> mapping;
}


public struct TerrainHeightMapSettings
{
    public float noiseScale;
    public float frequency;
    public float lacunarity;
    public int octaves;
    public float weight;
    public float falloffSteepness;
    public float falloffOffset;
}