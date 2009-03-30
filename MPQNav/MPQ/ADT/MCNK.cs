using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MPQNav.ADT
{
       /// <summary>
        /// Class for the MCNK chunk (vertex information for the ADT)
        /// </summary>
        public class MCNK
        {
            /// <summary>
            /// X position of the MCNK
            /// </summary>
            public float x;
            /// <summary>
            /// Y position of the MCNK
            /// </summary>
            public float y;
            /// <summary>
            /// Z position of the MCNK
            /// </summary>
            public float z;
            /// <summary>
            /// Holes of the MCNK
            /// </summary>
            public UInt16 holes;
            /// <summary>
            /// MCTV Chunk (Height values for the MCNK)
            /// </summary>
            public MCVT _MCVT = new MCVT();
            /// <summary>
            /// MCNR Chunk (Normals for the MCNK)
            /// </summary>
            public MCNR _MCNR = new MCNR();
            /// <summary>
            /// MH20 Chunk (Water information for the MCNK)
            /// </summary>
            public MH2O _MH20 = new MH2O();
            /// <summary>
            /// MCLQ Chunk (Water information for the MCNK) - Deprecated
            /// </summary>
            //public MCLQ _MCLQ = new MCLQ();

            public bool[,] GetHolesMap()
            {
                bool[,] lret = new bool[4, 4];
                for (int i = 0; i < 16; i++)
                {
                    lret[i / 4, i % 4] = (((holes >> (i)) & 1) == 1);
                }
                return lret;

            }

            /// <summary>
            /// Default contstructor
            /// </summary>
            public MCNK()
            {

            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="x">X position of the MCNK</param>
            /// <param name="y">Y position of the MCNK</param>
            /// <param name="z">Z position of the MCNK</param>
            /// <param name="holes">Holes of this MCNK</param>
            public MCNK(float x, float y, float z,UInt16 holes)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.holes = holes;
            }
            /// <summary>
            /// MCVT Chunk - Height information for the MCNK
            /// </summary>
            public class MCVT
            {
                /// <summary>
                /// 145 Floats for the 9 x 9 and 8 x 8 grid of height data.
                /// </summary>
                public float[] heights = new float[145];
                /// <summary>
                /// 145 Floats for the 9 x 9 and 8 x 8 grid of height data.
                /// </summary>
                public float[] GetLowResMapArray()
                {
                    float[] _heights = new float[81];

                    for (int r = 0; r < 17; r++)
                    {
                        if (r % 2 == 0)
                        {
                            for (int c = 0; c < 9; c++)
                            {

                                int count = ((r / 2) * 9) + ((r / 2) * 8) + c;
                                _heights[c + ((r/2)*8)] = heights[count];
                            }
                        }
                    }
                    return _heights;
                }
                /// <summary>
                /// 145 Floats for the 9 x 9 and 8 x 8 grid of height data.
                /// </summary>
                public float[,] GetLowResMapMatrix()
                {
                    // *  1    2    3    4    5    6    7    8    9       Row 0
                    // *    10   11   12   13   14   15   16   17         Row 1
                    // *  18   19   20   21   22   23   24   25   26      Row 2
                    // *    27   28   29   30   31   32   33   34         Row 3
                    // *  35   36   37   38   39   40   41   42   43      Row 4
                    // *    44   45   46   47   48   49   50   51         Row 5
                    // *  52   53   54   55   56   57   58   59   60      Row 6
                    // *    61   62   63   64   65   66   67   68         Row 7
                    // *  69   70   71   72   73   74   75   76   77      Row 8
                    // *    78   79   80   81   82   83   84   85         Row 9
                    // *  86   87   88   89   90   91   92   93   94      Row 10
                    // *    95   96   97   98   99   100  101  102        Row 11
                    // * 103  104  105  106  107  108  109  110  111      Row 12
                    // *   112  113  114  115  116  117  118  119         Row 13
                    // * 120  121  122  123  124  125  126  127  128      Row 14
                    // *   129  130  131  132  133  134  135  136         Row 15
                    // * 137  138  139  140  141  142  143  144  145      Row 16
                    // We only want even rows
                    float[,] _heights = new float[9,9];
                    for (int r = 0; r < 17; r++)
                    {
                        if (r % 2 == 0)
                        {
                            for (int c = 0; c < 9; c++)
                            {

                                int count = ((r / 2) * 9) + ((r / 2) * 8) + c;
                                _heights[r/2,c] = heights[count];
                            }
                        }
                    }
                    return _heights;
                }
            }
            


            /// <summary>
            /// MCNR Chunk - Normal information for the MCNK
            /// </summary>
            public class MCNR
            {
                // x, y, x format. One byte per integer. Each int is a signed int and between -127 and 127
                // with -127 being -1 and 127 being 1. This is for our normal vectors.
                /// <summary>
                /// Normal information, 3 integers per normal, one normal per vertex, 145 vertices
                /// </summary>
                public Vector3[] normals = new Vector3[145];
                /// <summary>
                /// 145 Floats for the 9 x 9 and 8 x 8 grid of Normal data.
                /// </summary>
                public Vector3[,] GetLowResNormalMatrix()
                {
                    Vector3[,] _normals = new Vector3[9,9];
                    for (int r = 0; r < 17; r++)
                    {
                        if (r % 2 == 0)
                        {
                            for (int c = 0; c < 9; c++)
                            {

                                int count = ((r / 2) * 9) + ((r / 2) * 8) + c;
                                _normals[r/2,c] = normals[count];
                            }
                        }
                    }
                    return _normals;
                }
            }
            
        }
}
