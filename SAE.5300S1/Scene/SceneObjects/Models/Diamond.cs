﻿using System.Diagnostics.Contracts;
using System.Numerics;
using SAE._5300S1.Utils.MathHelpers;
using SAE._5300S1.Utils.ModelHelpers;
using SAE._5300S1.Utils.SceneHelpers;
using Silk.NET.OpenGL;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;
using Texture = SAE._5300S1.Utils.ModelHelpers.Texture;

namespace SAE._5300S1.Scene.SceneObjects.Models; 

public class Diamond {
    public Mesh Mesh { get; set; }
    public Material Material { get; set; }
    private float _solarSystemMultiplier = 2;
    private float _rotationDegrees;

    private Texture _texture;
    private GL _gl;
    private string _textureName;
    private Matrix4x4 _matrix;
    private IModel _model;

    public Diamond(GL gl,
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
        Mesh = new Mesh(_gl, _model.Vertices, _model.Indices);
        _texture = new Texture(_gl, $"{_textureName}.jpg");
    }

    private bool _myBool = false;

    public unsafe void Render() {
        float angle = Time.TimeSinceStart * 0.5f;
        Mesh.Bind();
        Material.Use();
        _texture.Bind();
        _matrix = Matrix4x4.Identity;
        _matrix *= Matrix4x4.CreateRotationY(angle);
        _matrix *= Matrix4x4.CreateTranslation(-0.2f, 0, 0.25f);
        _matrix *= Matrix4x4.CreateScale(6f);

        Material.SetUniform("uModel", _matrix);
        Material.SetUniform("uView", Camera.Instance.GetViewMatrix());
        Material.SetUniform("uProjection", Camera.Instance.GetProjectionMatrix());
        Material.SetUniform("material.diffuse", 0.2f);
        Material.SetUniform("material.specular", 0.2f);
        Material.SetUniform("material.shininess", 500.0f);
        Material.SetUniform("light.viewPosition", Camera.Instance.Position);
        Material.SetUniform("light.position", Light.LightPosition2);
        Material.SetUniform("light.ambient", new Vector3(0.9f) * 1.0f);
        Material.SetUniform("light.diffuse", new Vector3(0.1f));
        Material.SetUniform("light.specular", new Vector3(1.0f));
        Material.SetUniform("useBlinnAlgorithm", _myBool ? 1 : 0);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, Mesh.IndicesLength);
    }
}