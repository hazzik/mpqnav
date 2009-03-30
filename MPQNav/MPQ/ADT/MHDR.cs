using System;
using System.Collections.Generic;
using System.Text;

namespace MPQNav.ADT
{
    /// <summary>
    /// Contains offsets (relative to 0x14) for some other chunks that appear in the file. Since the file follows a well-defined structure, this is redundant information.
    /// </summary>
    class MHDR
    {
        public UInt32 Base;
        /*000h*/
        public UInt32  pad;
        /*004h*/
        public UInt32 offsInfo;
        /*008h*/
        public UInt32 offsTex;
        /*00Ch*/
        public UInt32 offsModels;
        /*010h*/
        public UInt32 offsModelsIds;
        /*014h*/
        public UInt32 offsMapObejcts;
        /*018h*/
        public UInt32 offsMapObejctsIds;
        /*01Ch*/
        public UInt32 offsDoodsDef;
        /*020h*/
        public UInt32 offsObjectsDef;
        /*024h*/
        public UInt32 offsFlightBoundary; // tbc, wotlk	
        /*028h*/
        public UInt32 offsMH2O;		// new in WotLK
        /*02Ch*/
        public UInt32 pad3;
        /*030h*/
        public UInt32 pad4;
        /*034h*/
        public UInt32 pad5;
        /*038h*/
        public UInt32 pad6;
        /*03Ch*/
        public UInt32 pad7;
        /*040h*/


    }
}
