using System;
using System.Collections.Generic;

namespace MPQNav.Graphics {
	internal class TriangleListCollection : ITriangleList {
		private readonly IList<int> _indices = new List<int>();
		private readonly IList<ITriangleList> _lists = new List<ITriangleList>();
		private readonly IList<VertexPositionNormalColored> _vertices = new List<VertexPositionNormalColored>();
		private bool _durty;
		private int _offset;

		#region ITriangleList Members

		public IList<VertexPositionNormalColored> Vertices {
			get {
				lock(this) {
					if(_durty) {
						JoinAll();
					}
					return _vertices;
				}
			}
		}

		public IList<int> Indices {
			get {
				lock(this) {
					if(_durty) {
						JoinAll();
					}
					return _indices;
				}
			}
		}

		#endregion

		private void JoinAll() {
			_vertices.Clear();
			_indices.Clear();
			foreach(ITriangleList list in _lists) {
				for(int v = 0; v < list.Vertices.Count; v++) {
					_vertices.Add(list.Vertices[v]);
				}
				for(int i = 0; i < list.Indices.Count; i++) {
					_indices.Add(list.Indices[i] + _offset);
				}
				_offset = _vertices.Count;
			}
			_durty = false;
		}

		public void Add(ITriangleList triangeList) {
			_lists.Add(triangeList);
			_durty = true;
		}
	}
}