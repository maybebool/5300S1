﻿using System.Numerics;
using Silk.NET.OpenGL;

namespace SAE._5300S1.Utils.ModelHelpers {
    
    public class Shader : IDisposable {
        private uint _handle;
        private GL _gl;

        protected Shader(GL gl, string vertexFileName, string fragmentFileName) {
            _gl = gl;

            var vertex = LoadShader(ShaderType.VertexShader, AppContext.BaseDirectory + $"shaders/{vertexFileName}");
            var fragment = LoadShader(ShaderType.FragmentShader,
                AppContext.BaseDirectory + $"shaders/{fragmentFileName}");
            _handle = _gl.CreateProgram();
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0) {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }

            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        public void Use() {
            _gl.UseProgram(_handle);
        }

        public void SetUniform(string name, int value) {
            var location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) {
                throw new Exception($"{name} uniform not found on shader.");
            }

            _gl.Uniform1(location, value);
        }

        public void SetUniform(string name, Vector3 value) {
            var location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) {
                throw new Exception($"{name} uniform not found on shader.");
            }

            _gl.Uniform3(location, value.X, value.Y, value.Z);
        }

        public unsafe void SetUniform(string name, Matrix4x4 value) {
            var location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) {
                throw new Exception($"{name} uniform not found on shader.");
            }

            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }

        public void SetUniform(string name, float value) {
            var location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) {
                throw new Exception($"{name} uniform not found on shader.");
            }

            _gl.Uniform1(location, value);
        }

        public void Dispose() {
            _gl.DeleteProgram(_handle);
        }

        private uint LoadShader(ShaderType type, string path) {
            var src = File.ReadAllText(path);
            var handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);
            var infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog)) {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }
            return handle;
        }
    }
}