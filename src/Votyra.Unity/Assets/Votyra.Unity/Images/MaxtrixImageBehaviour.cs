﻿using System.Collections.Generic;
using System.Linq;
using Votyra.Common.Models;
using Votyra.Images;
using Votyra.Models;
using UnityEngine;

namespace Votyra.Unity.Images
{
    internal class MaxtrixImageBehaviour : MonoBehaviour, IImage2iProvider
    {
        public Texture2D InitialValueTexture;
        public float InitialValueScale;

        private Matrix<int> _editableMatrix;

        private readonly List<LockableMatrix<int>> _readonlyMatrices = new List<LockableMatrix<int>>();
        
        private MatrixImage _image = null;
        public IImage2i CreateImage()
        {
            if (_fieldsChanged)
            {
                _fieldsChanged = false;

                Debug.LogFormat("Update readonlyCount:{0} fieldsChanged:{1}", _readonlyMatrices.Count, _fieldsChanged);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix<int>(_editableMatrix.size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                for (int x = 0; x < _editableMatrix.size.x; x++)
                {
                    for (int y = 0; y < _editableMatrix.size.y; y++)
                    {
                        readonlyMatrix[x, y] = _editableMatrix[x, y];
                    }
                }

                _image = new MatrixImage(readonlyMatrix);
            }
            return _image;
        }
        
        private bool _fieldsChanged = false;

        private void Start()
        {
            if (InitialValueTexture == null)
            {
                var size = new Vector2i(10, 10);
                _editableMatrix = new Matrix<int>(size);
            }
            else
            {
                var texture = InitialValueTexture;
                var size = new Vector2i(texture.width, texture.height);
                _editableMatrix = new Matrix<int>(size);

                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        _editableMatrix[x, y] = (int)(texture.GetPixel(x, y).grayscale * InitialValueScale);
                        if (_editableMatrix[x, y] != 0)
                        {

                        }
                    }
                }
            }
            _fieldsChanged = true;
        }

        private void Update()
        {
        }

        public void SetByOffsetValue(Vector2i pos, int value)
        {
            SetValue(pos, _editableMatrix[pos] + value);
        }

        public void SetValue(Vector2i pos, int value)
        {
            
                Debug.LogWarningFormat("Setting value at {0} to {1} readonlyCount:{2} fieldsChanged:{3}", pos,value, _readonlyMatrices.Count, _fieldsChanged);

            _editableMatrix[pos] = value;

            _fieldsChanged = true;
        }

    }
}