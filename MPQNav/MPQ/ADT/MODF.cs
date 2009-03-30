using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MPQNav.ADT
{
    /// <summary>
    /// MODF Class - WMO Placement Information
    /// </summary>
    public class MODF
    {
        /// <summary>
        /// Filename of the WMO
        /// </summary>
        public String fileName;
        /// <summary>
        /// Unique ID of the WMO in this ADT
        /// </summary>
        public UInt32 uniqid;
        /// <summary>
        /// Position of the WMO
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Rotation of the Z axis
        /// </summary>
        public float orientation_a;
        /// <summary>
        /// Rotation of the Y axis
        /// </summary>
        public float orientation_b;
        /// <summary>
        ///  Rotation of the X axis
        /// </summary>
        public float orientation_c;
    }
}
