using System;
using System.Linq;
using UnityEngine;

namespace PulseMotion.VideoShaders
{
    public class SimpleWebCam : MonoBehaviour
    {
        public enum DeviceFacing
        {
            Any, FrontFacing, BackFacing
        };

        [HideInInspector]
        public const float farClipPlaneOffset = -1.0f;

        public string deviceName = null;
        public DeviceFacing desiredFacing = DeviceFacing.Any;
        public int desiredFPS = 30;
        public Camera targetCamera = null;
        public Renderer targetRenderer = null;
        public bool autoPlay = false;
        
        private WebCamTexture _texture = null;
        private Material _material = null;
        private float _videoRotationAngle = 0;

        public WebCamTexture texture
        {
            get { return _texture; }
        }

        public Material material
        {
            get { return _material; }
            set
            {
                if (_material != value)
                {
                    _material = value;
                    targetRenderer.material = value;

                    if (_material != null && _texture != null)
                    {
                        _material.mainTexture = _texture;
                    }
                }
            }
        }

        public void SetMaterial(Material value)
        {
            this.material = value;
        }

        public event Action WebCamResized;
        public event Action WebCamStarted;
        public event Action WebCamStopped;

        void Start()
        {
            Application.RequestUserAuthorization(UserAuthorization.WebCam);

            // Try to find the camera automatically
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
            // Try to find the material automatically
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<Renderer>();
            }

            ResizeWebCam();
            if (autoPlay)
            {
                PlayWebCam();
            }
        }

        void Update()
        {
            if (_texture != null && _texture.videoRotationAngle != _videoRotationAngle)
            {
                _videoRotationAngle = _texture.videoRotationAngle;
                ResizeWebCam();
            }
        }

        void OnDestroy()
        {
            StopWebCam();
        }

        // Resize the quad that this webcam is being rendered to
        public void ResizeWebCam()
        {
            // Don't resize the webcam if not ready
            if (targetCamera == null || targetRenderer == null)
            {
                return;
            }

            Transform cameraTransform = targetCamera.transform;
            float farClipPlane = targetCamera.farClipPlane + farClipPlaneOffset;
            targetRenderer.transform.position =
                cameraTransform.position + cameraTransform.forward * farClipPlane;

            var corner1 = targetCamera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, farClipPlane));
            var corner2 = targetCamera.ScreenToWorldPoint(new Vector3(targetCamera.pixelWidth, targetCamera.pixelHeight, farClipPlane));
            var cornerDelta = (corner2 - corner1);

            targetRenderer.transform.localScale = new Vector3(
                Vector3.Project(cornerDelta, cameraTransform.right).magnitude,
                Vector3.Project(cornerDelta, cameraTransform.up).magnitude,
                1.0f
            );

            if (WebCamResized != null)
            {
                WebCamResized();
            }

            // Resize the currently playing texture if needed
            if (_texture != null && _texture.isPlaying)
            {
                _videoRotationAngle = _texture.videoRotationAngle;
                targetRenderer.transform.localRotation = Quaternion.AngleAxis(
                    -_videoRotationAngle, Vector3.forward
                );

                if (_texture.requestedWidth != targetCamera.pixelWidth ||
                    _texture.requestedHeight != targetCamera.pixelHeight)
                {
                    StopWebCam();
                    PlayWebCam();
                }
            }
        }

        public void PlayWebCam()
        {
            // Do nothing if the webcam is already playing
            if (_texture != null && _texture.isPlaying)
            {
                return;
            }

            // Don't start the webcam if not ready
            if (targetCamera == null || targetRenderer == null ||
                !Application.HasUserAuthorization(UserAuthorization.WebCam) ||
                WebCamTexture.devices.Length <= 0)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(this.deviceName))
            {
                this.deviceName = WebCamTexture.devices[0].name;
                switch (desiredFacing)
                {
                    case DeviceFacing.FrontFacing:
                        foreach (var device in WebCamTexture.devices)
                        {
                            if (device.isFrontFacing)
                            {
                                this.deviceName = device.name;
                                break;
                            }
                        }
                        break;
                    case DeviceFacing.BackFacing:
                        foreach (var device in WebCamTexture.devices)
                        {
                            if (!device.isFrontFacing)
                            {
                                this.deviceName = device.name;
                                break;
                            }
                        }
                        break;
                    case DeviceFacing.Any:
                    default:
                        break;
                }
            }

            _texture = new WebCamTexture(
                deviceName, 
                targetCamera.pixelWidth, 
                targetCamera.pixelHeight, 
                desiredFPS
            );
            if (_material != null)
            {
                targetRenderer.material = _material;
            }
            else
            {
                _material = targetRenderer.material;
            }
            _material.mainTexture = _texture;
            _texture.Play();

            if (WebCamStarted != null)
            {
                WebCamStarted();
            }
        }

        public void StopWebCam()
        {
            if (_texture != null)
            {
                _texture.Stop();
                Destroy(_texture);
                _texture = null;

                if (WebCamStopped != null)
                {
                    WebCamStopped();
                }
            }
        }
    }
}