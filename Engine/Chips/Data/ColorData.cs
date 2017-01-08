﻿//  
// Copyright (c) Jesse Freeman. All rights reserved.  
// 
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// 

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PixelVisionSDK.Engine.Chips.Data
{

    /// <summary>
    ///     The ColorData class is a wraper for color data in the engine.
    ///     It provides a simple interface for storing RBG color data as
    ///     well as converting that data in Hex or vise versa.
    /// </summary>
    public class ColorData : AbstractData
    {

        protected float _b;
        protected float _g;

        protected float _r;

        /// <summary>
        ///     Use this constructor for setting the ColorData instance up
        ///     with RBG values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public ColorData(float r = 0f, float g = 0f, float b = 0f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        /// <summary>
        ///     Use this constructor for setting the ColorData instance up
        ///     with a HEX color value.
        /// </summary>
        /// <param name="hexColor"></param>
        public ColorData(string hexColor)
        {
            float tmpR, tmpG, tmpB;

            HexToColor(hexColor, out tmpR, out tmpG, out tmpB);

            r = tmpR;
            g = tmpG;
            b = tmpB;
        }

        /// <summary>
        ///     The red value of a color. This ranges from 0 to 255.
        /// </summary>
        public float r
        {
            get { return _r; }
            set { _r = value; }
        }

        /// <summary>
        ///     The green value of a color. This ranges from 0 to 255.
        /// </summary>
        public float g
        {
            get { return _g; }
            set { _g = value; }
        }

        /// <summary>
        ///     The blue value of a color. This ranges from 0 to 255.
        /// </summary>
        public float b
        {
            get { return _b; }
            set { _b = value; }
        }

        /// <summary>
        ///     Returns the HEX value of the color data.
        /// </summary>
        /// <returns name="String"></returns>
        public string ToHex()
        {
            return ColorToHex(r, g, b);
        }

        /// <summary>
        ///     Static method for converting RGB colors into HEX values
        /// </summary>
        /// <param name="r">Red value</param>
        /// <param name="g">Green value</param>
        /// <param name="b">Blue value</param>
        /// <returns></returns>
        public static string ColorToHex(float r, float g, float b)
        {
            var hex = "#" + ((int) (r * 255)).ToString("X2") + ((int) (g * 255)).ToString("X2") +
                      ((int) (b * 255)).ToString("X2");
            return hex;
        }

        /// <summary>
        ///     Static method for converting a HEX color into an RGB value.
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void HexToColor(string hex, out float r, out float g, out float b)
        {
            if (hex == null)
            {
                hex = "FF00FF";
            }

            if (hex[0] == '#')
                hex = hex.Substring(1);

            r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / (float) byte.MaxValue;
            g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / (float) byte.MaxValue;
            b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / (float) byte.MaxValue;
        }

        /// <summary>
        ///     Static method to calculate the brightness of a ColorData instance
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Brightness(float r, float g, float b)
        {
            return (float) Math.Sqrt(
                r * r * .241 +
                g * g * .691 +
                b * b * .068);
        }

        /// <summary>
        ///     Static method to valide that a HEX color can be parsed. HEX colors need
        ///     to be in the correct format #FFFFFF or #FFF
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool ValidateColor(string color)
        {
            var regex = new Regex(@"^#(?:[0-9a-fA-F]{3}){1,2}$");
            var match = regex.Match(color);
            return match.Success;
        }

    }

}