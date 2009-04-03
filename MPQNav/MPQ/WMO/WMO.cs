using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MPQNav.ADT;

namespace MPQNav.MPQ.ADT {
	/// <summary>
	/// Class for the WMO Group File
	/// </summary>
	internal class WMO {
		private readonly IList<ITriangleList> _wmoSubList = new List<ITriangleList>();
		private ITriangleList _triangleList = new TriangleList();

		/// <summary>
		/// List containg all the WMO Sub-Chunks for this WMO Group File
		/// </summary>
		public IList<ITriangleList> WmoSubList {
			get { return _wmoSubList; }
		}

		public ITriangleList TriangleList {
			get { return _triangleList; }
		}

		public void Transform(Vector3 position, Vector3 rotation, float scale) {
			_triangleList = GetTriangleList().Transform(ADTManager.CreateOrigin(position), ADTManager.CreateTransform(rotation, scale));
		}

		private ITriangleList GetTriangleList() {
			var list = new TriangleListCollection();

			for(int i = 0; i < WmoSubList.Count; i++) {
				list.Add(WmoSubList[i]);
			}
			return list;
		}
	}
}