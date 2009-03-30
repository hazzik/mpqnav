using System;
using System.Collections.Generic;
using System.Text;

namespace MPQNav.ADT
{
    /// <summary>
    /// MH20 Chunk - Fluid information for the MCNK
    /// </summary>
    public class MH2O
    {

        public enum FluidType
        {
            lake,
            lava,
            oceans
        }

        public bool used; //1 if this chunk has liquids, 0 if not. Maybe there are more values. If 0, the other fields are 0 too.

        public FluidType type; //Seems to be the type of liquid (0 for normal "lake" water, 1 for lava and 2 for ocean water)

        public float heightLevel1;	//	The global liquid-height of this chunk.

        public float heightLevel2;	//	Which is always in there twice. Blizzard knows why. (Actually these values are not always identical, I think they define the highest and lowest points in the heightmap)

        public byte xOffset;			//The X offset of the liquid square (0-7)

        public byte yOffset;		//The Y offset of the liquid square (0-7)

        public byte width;		//The width of the liquid square (1-8)

        public byte height;			//The height of the liquid square (1-8)

        public float[] heights; //this will contain all heights of fluid


        public byte[] RenderBitMap;

        public float[,] GetMapHeightsMatrix()
        {
            if ((used != true) || (heights == null))
                throw new Exception("This MH2O chunk is not used");
            float[,] _heights = new float[height + 1, width + 1];
            for (int r = 0; r <= height; r++)
            {
                for (int c = 0; c <= width; c++)
                {

                    _heights[r, c] = heights[c + r * c];
                }
            }
            return _heights;
        }

        public bool[,] GetRenderBitMapMatrix()
        {
            bool[,] _enabled = new bool[height, 8];
            for (int r = 0; r < height; r++)
            {
                for (int c = 7, _c = 0; c >= 0; c--, _c++)
                {
                    _enabled[r, c] = (((RenderBitMap[r] >> c) & 1) == 1);

                }
            }
            return _enabled;
        }


    }
}
