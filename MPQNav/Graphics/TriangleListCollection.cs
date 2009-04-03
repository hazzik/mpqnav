using System;
using System.Collections;
using System.Collections.Generic;

namespace MPQNav.Graphics {
	internal class TriangleListCollection : ICollection<ITriangleList>, ITriangleList {
		private readonly IList<int> _indices = new List<int>();
		private readonly IList<ITriangleList> _lists = new List<ITriangleList>();
		private readonly IList<VertexPositionNormalColored> _vertices = new List<VertexPositionNormalColored>();
		private bool _durty;
		private int _offset;

		#region ICollection<ITriangleList> Members

		public void Add(ITriangleList triangeList) {
			_lists.Add(triangeList);
			_durty = true;
		}

		public void Clear() {
			_lists.Clear();
			_durty = true;
		}

		public bool Contains(ITriangleList item) {
			return _lists.Contains(item);
		}

		public void CopyTo(ITriangleList[] array, int arrayIndex) {
			_lists.CopyTo(array, arrayIndex);
		}

		public bool Remove(ITriangleList item) {
			if(_lists.Remove(item)) {
				_durty = true;
				return true;
			}
			return false;
		}

		public int Count {
			get { return _lists.Count; }
		}

		public bool IsReadOnly {
			get { return _lists.IsReadOnly; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		IEnumerator<ITriangleList> IEnumerable<ITriangleList>.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion

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

		public IEnumerator<ITriangleList> GetEnumerator() {
			return _lists.GetEnumerator();
		}

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
	}
}