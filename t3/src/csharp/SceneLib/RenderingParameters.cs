using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
    //Represents a filtering mode for texture mapping
    public enum SceneTextureMode
    {
        Nearest,
        Bilinear,
        Mipmap,
        Trilinear
    }

    //Represents a rendering mode
    public enum SceneRenderMode
    {
        WireFrame,
        WireFrameAntialias,
        Fill,
        FillAntialias
    }

    public enum ShadingMode
    {
        Gouraud,
        Phong
    }

    /// <summary>
    /// Stores relevant rendering parameters
    /// </summary>
    public class RenderingParameters
    {
        public SceneTextureMode MinTextureMode { get; set; }
        public SceneTextureMode MagTextureMode { get; set; }
        public SceneRenderMode RenderMode { get; set; }
        public ShadingMode ShadeMode { get; set; }

        public bool EnablePersepctiveCorrected { get; set; }
        public bool EnableShading { get; set; }
        public bool EnableReflections { get; set; }
        public bool EnableRefractions { get; set; }
        public bool EnableShadows { get; set; }
        public bool EnableAttenuation { get; set; }
        public bool EnableAntialias { get; set; }
        public bool EnableDepthOfField { get; set; }
        public bool EnableTestDepth { get; set; }
        public bool EnableFustrumCulling { get; set; }
        public bool EnableFustrumClipping { get; set; }
        public bool EnableMultipleLights { get; set; }
        public bool EnableSoftShadows { get; set; }
        public bool EnableMotionBlur { get; set; }
        public bool EnableTextureMapping { get; set; }
        public bool EnablePhotonMapping { get; set; }
        public bool EnableRenderZBuffer { get; set; }
        public bool EnableShadowMapping { get; set; }
        public int PixelSize { get; set; }

        public RenderingParameters()
        {
            MinTextureMode = SceneTextureMode.Bilinear;
            MagTextureMode = SceneTextureMode.Bilinear;
            RenderMode = SceneRenderMode.Fill;
            ShadeMode = ShadingMode.Phong;
            EnablePersepctiveCorrected = true;
            EnableShading = true;
            EnableReflections = true;
            EnableRefractions = false;
            EnableShadows = false;
            EnableAttenuation = true;
            EnableAntialias = false;
            EnableDepthOfField = false;
            EnableTestDepth = true;
            EnableFustrumCulling = true;
            EnableFustrumClipping = false;
            EnableMultipleLights = true;
            EnableSoftShadows = false;
            EnableMotionBlur = false;
            EnableTextureMapping = true;
            EnablePhotonMapping = false;
            EnableRenderZBuffer = false;
            EnableShadowMapping = false;
            PixelSize = 1;
        }
    }
}
