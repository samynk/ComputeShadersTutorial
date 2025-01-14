using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    using OpenTK.Mathematics;


    public class Camera
    {
        private Vector3 _cameraPosition;
        private Vector3 _worldUpAxis;
        private bool _rightHanded;

        // Rotation angles
        private float _phi;       // Horizontal rotation angle
        private float _azimuth;   // Vertical rotation angle

        // Reference matrix that stores the camera's (right, up, forward) vectors in its columns
        private Matrix4 _cameraReference;

        // Final camera matrix after applying azimuth/phi rotations + translation
        private Matrix4 _cameraMatrix;

        /// <summary>
        /// Constructs a camera with given position, world-up axis, forward axis,
        /// and whether the coordinate system is right-handed.
        /// </summary>
        public Camera(Vector3 position, Vector3 upAxis, Vector3 forwardAxis, bool rightHanded)
        {
            _cameraPosition = position;
            _worldUpAxis = upAxis;
            _rightHanded = rightHanded;

            // Compute the camera right axis
            Vector3 cameraRight;
            if (_rightHanded)
            {
                // Right-handed cross: forward × up
                cameraRight = Vector3.Cross(forwardAxis, _worldUpAxis);
            }
            else
            {
                // Left-handed cross: up × forward
                cameraRight = Vector3.Cross(_worldUpAxis, forwardAxis);
            }

            // Initialize the reference matrix to identity
            _cameraReference = Matrix4.Identity;

            // Store the axes as columns: [cameraRight, upAxis, forwardAxis, translation=0]
            _cameraReference.Column0 = new Vector4(cameraRight, 0.0f);
            _cameraReference.Column1 = new Vector4(_worldUpAxis, 0.0f);
            _cameraReference.Column2 = new Vector4(forwardAxis, 0.0f);
        }

        public void SetPhi(float phi)
        {
            _phi = phi;
        }

        /// <summary>
        /// Updates the final camera matrix by applying the rotations around
        /// the camera's right axis (for azimuth) and the camera's up axis (for phi).
        /// </summary>
        public void Update()
        {
            // 1. Rotation around the camera's right axis (_cameraReference.Column0)
            Vector3 cameraRight = _cameraReference.Column0.Xyz;
            Matrix4 azimuthMatrix = Matrix4.CreateFromAxisAngle(cameraRight, _azimuth);

            // Apply the azimuth rotation
            Matrix4 pitchedCamera = azimuthMatrix * _cameraReference;

            // 2. Rotation around the up axis (in the reference)
            Vector3 cameraUp = _cameraReference.Column1.Xyz;
            Matrix4 upRotation = Matrix4.CreateFromAxisAngle(cameraUp, _phi);

            // Combine them
            _cameraMatrix = upRotation * pitchedCamera;

            // 3. Put the camera position into the 4th column (translation)
            _cameraMatrix.Column3 = new Vector4(_cameraPosition, 1.0f);
        }

        /// <summary>
        /// Gets the final 4×4 camera matrix (orientation + translation).
        /// </summary>
        public Matrix4 GetCameraMatrix()
        {
            return _cameraMatrix;
        }
    }
}
