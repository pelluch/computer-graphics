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
        Normal,
        Antialias
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
        public bool EnableMultipleLights { get; set; }
        public bool EnableSoftShadows { get; set; }
        public bool EnableMotionBlur { get; set; }
        public bool EnableTextureMapping { get; set; }

        public int PixelSize { get; set; }

        public RenderingParameters()
        {
            MinTextureMode = SceneTextureMode.Nearest;
            MagTextureMode = SceneTextureMode.Nearest;
            RenderMode = SceneRenderMode.Normal;
            ShadeMode = ShadingMode.Phong;
            EnablePersepctiveCorrected = true;
            EnableShading = true;
            EnableReflections = true;
            EnableRefractions = true;
            EnableShadows = true;
            EnableAttenuation = true;
            EnableAntialias = true;
            EnableDepthOfField = false;
            EnableTestDepth = true;
            EnableFustrumCulling = true;
            EnableMultipleLights = true;
            EnableSoftShadows = false;
            EnableMotionBlur = true;
            EnableTextureMapping = true;

            PixelSize = 1;
        }
    }
}
