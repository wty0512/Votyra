﻿using System;
using Votyra.Models;
using Votyra.Images;
using Votyra.Unity.Images;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Unity.GroupSelectors
{
    public interface IGroupVisibilityContext : IContext
    {
        Vector3 CameraPosition { get; }
        IReadOnlyList<Plane> CameraPlanes { get; }
        IReadOnlyPooledList<Vector3> CameraFrustumCorners { get; }
        Matrix4x4 CameraLocalToWorldMatrix { get; }
        Matrix4x4 ParentContainerWorldToLocalMatrix { get; }
        Bounds GroupBounds { get; }
        Rect2i InvalidatedArea_worldSpace { get; }
        IReadOnlySet<Vector2i> ExistingGroups { get; }
        Vector2i CellInGroupCount { get; }
    }
}