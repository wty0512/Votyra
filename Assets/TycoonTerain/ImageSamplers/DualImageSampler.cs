﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DualImageSampler : MonoBehaviour, IImageSampler
{
    public HeightData Sample(IImage2i image, Vector2i offset, float time)
    {
        offset = offset + offset;

        int x0y0 = image.Sample(offset, time);
        int x0y1 = image.Sample(new Vector2i(offset.x, offset.y+1), time);
        int x1y0 = image.Sample(new Vector2i(offset.x+1, offset.y ), time);
        int x1y1 = image.Sample(new Vector2i(offset.x+1, offset.y + 1), time);

        return new HeightData(x0y0, x0y1, x1y0, x1y1);
    }
}
