﻿//   
// Copyright (c) Jesse Freeman, Pixel Vision 8. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) except for a few
// portions of the code. See LICENSE file in the project root for full 
// license information. Third-party libraries used by Pixel Vision 8 are 
// under their own licenses. Please refer to those libraries for details 
// on the license they use.
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christina-Antoinette Neofotistou @CastPixel
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany
//

namespace PixelVision8.Engine
{
    public class TileData : AbstractData
    {
        private int _colorOffset;
        private int _flag = -1;
        private bool _flipH;
        private bool _flipV;
        private int _spriteID;

        public int index;


        public TileData(int index, int spriteID = -1, int colorOffset = 0, int flag = -1, bool flipH = false,
            bool flipV = false)
        {
            this.index = index;
            this.spriteID = spriteID;
            this.colorOffset = colorOffset;
            this.flag = flag;

            this.flipH = flipH;
            this.flipV = flipV;
            Invalidate();
        }

        public int spriteID
        {
            get => _spriteID;
            set
            {
                _spriteID = value;
                Invalidate();
            }
        }

        public int colorOffset
        {
            get => _colorOffset;
            set
            {
                _colorOffset = value;
                Invalidate();
            }
        }

        public int flag
        {
            get => _flag;
            set
            {
                _flag = value;
                Invalidate();
            }
        }

        public bool flipH
        {
            get => _flipH;
            set
            {
                _flipH = value;
                Invalidate();
            }
        }


        public bool flipV
        {
            get => _flipV;
            set
            {
                _flipV = value;
                Invalidate();
            }
        }

        public void Clear()
        {
            spriteID = -1;
            colorOffset = 0;
            flag = -1;

            flipH = false;
            flipV = false;

            Invalidate();
        }
    }
}


//public uint index;
//
//        public int spriteID => ((int) (gid & SpriteIDMask))-1;
//        public int colorOffset => (int)((gid & ColorOffsetMask) >> 19);
//        public int flag => (int)((gid & FlagIDMask) >> 15)-1;
//        public bool flipH => (FlipHMask & gid) != 0;
//        public bool flipV => (FlipVMask & gid) != 0;
//
//        public bool isEmpty
//        {
//            set
//            {
//                
//            }
//            get
//            {
//                return (EmptyMask & gid) != 0;
//            }
//        }
//        public TileData(uint index, int spriteID, int colorOffset = 0, int flag = -1, bool flipH = false, bool flipV = false)
//        {
//            this.index = index;
//            
//            gid = CreateTileGID(spriteID, flipH, flipV, flag, colorOffset);
//            
//            Console.WriteLine("TileData "+ spriteID+" "+this.spriteID);
//
////            this.index = index;
////            this.spriteID = spriteID;
////            this.colorOffset = colorOffset;
////            this.flag = flag;
////
////            this.flipH = flipH;
////            this.flipV = flipV;
//        }
//
//        public TileData(uint index, uint gid)
//        {
//            this.gid = gid;
//            this.index = index;
//
//        }
//
//        public uint gid;
//
//        public enum MyEnum:uint
//        {
//            Test = 11
//        }
//        
//        public const uint SpriteIDMask = (1 << (int)MyEnum.Test) - 1;
//        public const uint EmptyMask = (1 << 12);
//        public const uint FlipHMask = (1 << 13);
//        public const uint FlipVMask = (1 << 14);
//        public const uint FlagIDMask = (15 << 15);
//        public const uint ColorOffsetMask = (255 << 19);
//        
//        public void ReadTileGID(uint gid)
//        {
//            // Get values
//
////        var gid = 0;
//        
//            // 11 bits for sprite ID (2048)
//        
//            //uint spriteIDMask = (1 << 11) - 1;
//            var spriteID = gid & SpriteIDMask;
//        
//        
//            // 1 bit flip H
//        
//            //uint flipHMask = (1 << 12);
//        
//            var fliph = (FlipHMask & gid) != 0;
//
//            // 1 bit flip V
//        
//            //uint flipVMask = (1 << 13);
//        
//            var flipV = (FlipVMask & gid) != 0;
//
//        
//            // 4 bits for flag
//            //uint flagMask = (15 << 14); // Shifting the mask up to the 14th position
//            var flag = (gid & FlagIDMask) >> 14;
//
//        
//            // 8 bits for color offset
//            //uint colorMask = (255 << 18); // Shifting the mask up to the 14th position
//            var color = (gid & ColorOffsetMask) >> 18;
//        
//            Console.WriteLine("Read GID "+ spriteID +  " flipH " + fliph + " flipV " + flipV +" mask "+flag +" color " + color);
//        }
//    
//        public uint CreateTileGID(int spriteID, bool flipH, bool flipV, int flag, int color)
//        {
//            var gid = 0U;
//        
//            // sprite ID
//            gid = (gid & ~SpriteIDMask) | (uint)spriteID; // Make sure to clean up for negative number
//
//            // Flip H
//            gid &= ~FlipHMask;
//        
//            if (flipH)
//                gid |= FlipHMask;
//
//        
//            // Flip V
//            gid &= ~FlipVMask;
//        
//            if (flipV)
//                gid |= FlipVMask;
//        
//        
//            // sprite ID
//            gid = (gid & ~FlagIDMask) | ((uint)flag << 14);
//        
//            // sprite ID
//            gid = (gid & ~ColorOffsetMask) | ((uint)color << 18);
//        
//
//            return gid;
//        }