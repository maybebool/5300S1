﻿using System.Diagnostics.Contracts;
using System.Numerics;
using SAE._5300S1.Utils.MathHelpers;
using SAE._5300S1.Utils.ModelHelpers;
using SAE._5300S1.Utils.SceneHelpers;
using Silk.NET.OpenGL;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;
using Texture = SAE._5300S1.Utils.ModelHelpers.Texture;


namespace SAE._5300S1.Scene.SceneObjects.Models; 

public class Icosahedron {
    public Mesh Mesh { get; set; }
    public Material Material { get; set; }
    private float _solarSystemMultiplier = 2;
    private float _rotationDegrees;
    private const float Speed = 10;
    //public Transform Transform { get; set; }

    private Texture _texture;
    private GL _gl;
    private string _textureName;
    private Matrix4x4 _matrix;
    private IModel _model;

    public Icosahedron(GL gl,
        string textureName,
        Material material,
        IModel model) {
        _model = model;
        _textureName = textureName;
        Material = material;
        _gl = gl;
        Init();
    }

    private void Init() {
        Mesh = new Mesh(_gl, _model.Vertices , _model.Indices);
        _texture = new Texture(_gl, $"{_textureName}.jpg");

    }

    public unsafe void Render() {

        float t = MathF.Sin(Time.TimeSinceStart * 1);
        float angle = Time.TimeSinceStart * 1;
        Mesh.Bind();
        Material.Use();
        _texture.Bind();
        _matrix = Matrix4x4.Identity;
        _matrix *= Matrix4x4.CreateRotationY(angle);
        _matrix *= Matrix4x4.CreateRotationX(angle);
        _matrix *= Matrix4x4.CreateTranslation(2, 0, 0);
        _matrix *= Matrix4x4.CreateScale(1f);
        
        Material.SetUniform("uModel", _matrix);
        Material.SetUniform("uView", Camera.Instance.GetViewMatrix());
        Material.SetUniform("uProjection", Camera.Instance.GetProjectionMatrix());
        Material.SetUniform("viewPos", Camera.Instance.Position);
        Material.SetUniform("material.diffuse", 0.5f);
        Material.SetUniform("material.specular", 1f);
        Material.SetUniform("material.shininess", 1f);
        Material.SetUniform("light.position", Light.LightPosition);
        Material.SetUniform("light.ambient", new Vector3(0.7f) * new Vector3(1f));
        Material.SetUniform("light.diffuse", new Vector3(1.7f));
        Material.SetUniform("light.specular", new Vector3(1.0f));

        _gl.DrawArrays(PrimitiveType.Triangles, 0, Mesh.IndicesLength);
        
    }
}