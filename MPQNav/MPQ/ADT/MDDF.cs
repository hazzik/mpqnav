using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MPQNav.ADT
{
    /// <summary>
    /// MDDF Chunk Class - Placement information for M2 Models
    /// </summary>
    public class MDDF
    {
        /// <summary>
        /// Filename of the M2
        /// </summary>
        public String filePath;
        /// <summary>
        /// Unique ID of the M2 in this ADT
        /// </summary>
        public UInt32 uniqid;
        /// <summary>
        /// Position of the M2
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Rotation around the Z axis
        /// </summary>
        public float orientation_a;
        /// <summary>
        /// Rotation around the Y axis
        /// </summary>
        public float orientation_b;
        /// <summary>
        /// Rotation around the X axis
        /// </summary>
        public float orientation_c;
        /// <summary>
        ///  Scale factor of the M2
        /// </summary>
        public float scale = 1;
    }
}
