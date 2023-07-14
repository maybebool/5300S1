﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

namespace SAE._5300S1
{
    public class Mesh : IDisposable
    {
        public Mesh(GL gl, float[] vertices, uint[] indices, List<Texture> textures)
        {
            GL = gl;
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
            SetupMesh();
        }

        public uint IndicesLength => (uint)Vertices.Length;
        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }
        public IReadOnlyList<Texture> Textures { get; private set; }
        public VertexArrayObject<float, uint> VAO { get; set; }
        public BufferObject<float> VBO { get; set; }
        public BufferObject<uint> EBO { get; set; }
        public GL GL { get; }

        public unsafe void SetupMesh()
        {
            EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
            VBO = new BufferObject<float>(GL, Vertices, BufferTargetARB.ArrayBuffer);
            VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);
            VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            VAO.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
            VAO.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        }

        public void Bind() {
            VAO.Bind();
            foreach (var texture in Textures) {
                texture.Bind();
            }
        }

        public void Dispose()
        {
            Textures = null;
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }
    }
}