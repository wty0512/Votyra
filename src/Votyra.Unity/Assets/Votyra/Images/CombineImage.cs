﻿using System;
using Votyra.Models;
using UnityEngine;
using Votyra.Utils;

namespace Votyra.Images
{
    public class CombineImage2i : IImage2i
    {
        public IImage2i ImageA { get; private set; }
        public IImage2i ImageB { get; private set; }
        public Operations Operation { get; private set; }

        public enum Operations
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public CombineImage2i(IImage2i imageA, IImage2i imageB, Operations operation)
        {
            ImageA = imageA;
            ImageB = imageB;
            Operation = operation;
            RangeZ = ImageA.RangeZ + ImageB.RangeZ;
        }
        public Range2i RangeZ { get; private set; }

        public int Sample(Vector2i point)
        {
            int a = ImageA.Sample(point);
            int b = ImageB.Sample(point);
            switch (Operation)
            {
                case Operations.Add:
                    return a + b;
                case Operations.Subtract:
                    return a - b;
                case Operations.Multiply:
                    return a * b;
                case Operations.Divide:
                    return a / b;
                default:
                    return 0;
            }
        }
    }
}