using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Vector3Int coord;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private List<KeyValuePair<Vector3, StaticBlock>> blocksMap = new List<KeyValuePair<Vector3, StaticBlock>>();

    private void SetUp(Material mat)
    {
        GameObject go = new GameObject($"Chunk ({coord.x}, {coord.y}, {coord.z}) mesh");
        go.transform.parent = transform.parent.transform;

        meshFilter = go.GetComponent<MeshFilter>();
        meshRenderer = go.GetComponent<MeshRenderer>();
        meshCollider = go.GetComponent<MeshCollider>();

        if (meshFilter == null)
        {
            meshFilter = go.gameObject.AddComponent<MeshFilter>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = go.gameObject.AddComponent<MeshRenderer>();
        }

        if (meshCollider == null)
        {
            meshCollider = go.gameObject.AddComponent<MeshCollider>();
        }

        meshCollider.enabled = false;
        meshCollider.enabled = true;


        meshRenderer.material = mat;

        go.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        gameObject.layer = LayerMask.NameToLayer("Terrain");
        transform.tag = "Terrain";
    }

    public void SetMesh(Mesh mesh)
    {
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

        meshCollider.enabled = false;
        meshCollider.enabled = true;
    }

    public void AddBlock(ref Vector3 position,ref Vector3 blockSize)
    {
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.size = blockSize;
        collider.center = new Vector3(position.x * collider.size.x + collider.size.x / 2, position.y * collider.size.y, position.z * collider.size.z + collider.size.z / 2);

        StaticBlock block = new StaticBlock();
        block.SetUp(collider, new Vector3Int(1, 1, 1));

        blocksMap.Add(new KeyValuePair<Vector3, StaticBlock>(position, block));
    }

    public StaticBlock GetBlock(BoxCollider collider)
    {
        foreach (var block in blocksMap)
        {
            if (block.Value.GetCollider() == collider)
            {
                return block.Value;
            }
        }
        return null;
    }

    public bool EnableBlock(ref Vector3 position)
    {
        foreach (var block in blocksMap)
        {
            if (block.Key == position)
            {
                block.Value.Enable();
                return true;
            }
        }
        return false;
    }

    public bool DisableBlock(ref Vector3 position)
    {
        foreach (var block in blocksMap)
        {
            if (block.Key == position)
            {
                block.Value.Disable();
                return true;
            }
        }
        return false;
    }

    public static Vector3 CalculateBlockPosition(BoxCollider collider)
    {

        return new Vector3((collider.center.x - collider.size.x / 2)/ collider.size.x, collider.center.y / collider.size.y, (collider.center.z - collider.size.z / 2) / collider.size.z);
    }

    public Vector3Int GetCoords()
    {
        return coord;
    }

    public static class ChunkFabric
    {
        public static Chunk Create(Vector3Int coord, Material mat, Transform parent)
        {
            GameObject chunk = new GameObject($"Chunk ({coord.x}, {coord.y}, {coord.z})");
            chunk.transform.parent = parent;
            Chunk newChunk = chunk.AddComponent<Chunk>();

            newChunk.coord = coord;
            newChunk.SetUp(mat);

            return newChunk;
        }
    }
}
