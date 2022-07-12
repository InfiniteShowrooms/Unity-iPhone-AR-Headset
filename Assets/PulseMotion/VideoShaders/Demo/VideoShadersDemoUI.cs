using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PulseMotion.VideoShaders
{
    public class VideoShadersDemoUI : MonoBehaviour
    {
        public SimpleWebCam webcam;

        public Material chromaKeyMaterial;
        public GameObject chromaKeyPanel;
        public Dropdown chromaKeyDropdown;
        public Slider chromaThresholdSlider;
        public Slider chromaSlopeSlider;

        // Use this for initialization
        void Start()
        {
            if (webcam == null)
            {
                webcam = FindObjectOfType<SimpleWebCam>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (chromaKeyMaterial == null || !object.ReferenceEquals(webcam.material, chromaKeyMaterial))
            {
                if (chromaKeyPanel != null)
                {
                    chromaKeyPanel.SetActive(false);
                }
            }
            else
            {
                if (chromaKeyPanel != null)
                {
                    chromaKeyPanel.SetActive(true);
                }
                if (chromaKeyDropdown != null)
                {
                    var option = chromaKeyDropdown.options[chromaKeyDropdown.value];
                    switch (option.text)
                    {
                        case "Green":
                            chromaKeyMaterial.SetColor("_ChromaKey", Color.green);
                            break;
                        case "Cyan":
                            chromaKeyMaterial.SetColor("_ChromaKey", Color.cyan);
                            break;
                        case "Blue":
                            chromaKeyMaterial.SetColor("_ChromaKey", Color.blue);
                            break;
                        case "Magenta":
                            chromaKeyMaterial.SetColor("_ChromaKey", Color.magenta);
                            break;
                        case "Red":
                            chromaKeyMaterial.SetColor("_ChromaKey", Color.red);
                            break;
                        case "Yellow":
                            chromaKeyMaterial.SetColor("_ChromaKey", Color.yellow);
                            break;
                    }
                }
                if (chromaThresholdSlider != null)
                {
                    chromaKeyMaterial.SetFloat("_ChromaThreshold", chromaThresholdSlider.value);
                }
                if (chromaSlopeSlider != null)
                {
                    chromaKeyMaterial.SetFloat("_ChromaSlope", chromaSlopeSlider.value);
                }
            }
        }

        public void SetMaterial(Material value)
        {
            if (webcam != null)
            {
                webcam.material = value;
            }
        }

        public void Play()
        {
            if (webcam != null)
            {
                webcam.StopWebCam();
                webcam.ResizeWebCam();
                webcam.PlayWebCam();
            }
        }
    }
}
