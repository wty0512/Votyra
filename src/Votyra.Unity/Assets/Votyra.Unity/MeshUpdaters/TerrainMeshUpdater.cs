﻿using System.Collections.Generic;
using Votyra.Profiling;
using Votyra.Unity.Utils;
using UnityEngine;
using Votyra.Models;
using System.Linq;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using System;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Unity.MeshUpdaters
{
    public class TerrainMeshUpdater : IMeshUpdater
    {
        private SetDictionary<Vector2i, MeshFilter> _meshFilters = new SetDictionary<Vector2i, MeshFilter>();

        public IReadOnlySet<Vector2i> ExistingGroups => _meshFilters;

        public TerrainMeshUpdater()
        {
        }

        public void UpdateMesh(IMeshContext options, IReadOnlyDictionary<Vector2i, ITerrainMesh2i> terrainMeshes, IEnumerable<Vector2i> toKeepGroups)
        {
            if (terrainMeshes != null)
            {
                using (options.ProfilerFactory("Setting Mesh", this))
                {
                    var toDeleteGroups = _meshFilters.Keys.Except(terrainMeshes.Keys).Except(toKeepGroups).ToList();

                    int meshIndex = 0;
                    foreach (var terrainMesh in terrainMeshes)
                    {
                        MeshFilter meshFilter;
                        if (!_meshFilters.TryGetValue(terrainMesh.Key, out meshFilter))
                        {
                            if (toDeleteGroups.Count > 0)
                            {
                                int toDeleteIndex = toDeleteGroups.Count - 1;
                                var toDeleteKey = toDeleteGroups[toDeleteIndex];
                                meshFilter = _meshFilters[toDeleteKey];
                                _meshFilters.Remove(toDeleteKey);
                                _meshFilters[terrainMesh.Key] = meshFilter;

                                toDeleteGroups.RemoveAt(toDeleteIndex);
                            }
                            else
                            {
                                meshFilter = CreateMeshObject(options);
                                _meshFilters[terrainMesh.Key] = meshFilter;
                            }
                        }

                        ITerrainMesh2i triangleMesh = terrainMesh.Value;
                        UpdateMesh(triangleMesh, meshFilter.sharedMesh);

                        var collider = meshFilter.gameObject.GetComponent<MeshCollider>();
                        collider.sharedMesh = null;
                        collider.sharedMesh = meshFilter.sharedMesh;

                        meshIndex++;
                    }

                    foreach (var toDeleteGroup in toDeleteGroups)
                    {
                        _meshFilters[toDeleteGroup].gameObject.Destroy();
                        _meshFilters.Remove(toDeleteGroup);
                    }
                }
            }
        }

        private void UpdateMesh(ITerrainMesh2i triangleMesh, Mesh mesh)
        {
            if (triangleMesh is FixedTerrainMesh2)
            {
                UpdateMesh(triangleMesh as FixedTerrainMesh2, mesh);
            }
            else if (triangleMesh is IPooledTerrainMesh)
            {
                UpdateMesh((triangleMesh as IPooledTerrainMesh).Mesh, mesh);
            }
            else if (triangleMesh != null)
            {
                Debug.LogError($"Unsuported ITriangleMesh implementation '{triangleMesh.GetType().Name}'");
            }
        }

        private void UpdateMesh(FixedTerrainMesh2 triangleMesh, Mesh mesh)
        {
            bool recomputeTriangles = mesh.vertexCount != triangleMesh.PointCount;
            if (recomputeTriangles)
            {
                mesh.Clear();
            }

            mesh.vertices = triangleMesh.Vertices;
            mesh.SetNormalsOrRecompute(triangleMesh.Normals);
            mesh.uv = triangleMesh.UV;
            if (recomputeTriangles)
            {
                mesh.SetTriangles(triangleMesh.Indices, 0, false);
            }
            mesh.bounds = triangleMesh.MeshBounds;
        }

        private MeshFilter CreateMeshObject(IMeshContext options)
        {
            string name = string.Format("group_{0}", _meshFilters.Count);
            var tile = options.GameObjectFactory != null ? options.GameObjectFactory() : new GameObject();
            tile.name = name;
            tile.hideFlags = HideFlags.DontSave;

            var meshFilter = tile.GetOrAddComponent<MeshFilter>();
            tile.AddComponentIfMissing<MeshRenderer>();
            tile.AddComponentIfMissing<MeshCollider>();

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.mesh = new Mesh();
            }
            var mesh = meshFilter.sharedMesh;
            mesh.MarkDynamic();

            return meshFilter;
        }
    }
}