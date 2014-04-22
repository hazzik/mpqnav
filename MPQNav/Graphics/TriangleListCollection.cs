using System;
using System.Collections;
using System.Collections.Generic;

namespace MPQNav.Graphics {
	internal class TriangleListCollection :TriangleList, ICollection<TriangleList> {
		private readonly IList<int> _indices = new List<int>();
		private readonly IList<TriangleList> _lists = new List<TriangleList>();
		private readonly IList<VertexPositionNormalColorTexture> _vertices = new List<VertexPositionNormalColorTexture>();
		private bool _durty;

	    public void Add(TriangleList triangeList) {
			_lists.Add(triangeList);
			_durty = true;
		}

		public void Clear() {
			_lists.Clear();
			_durty = true;
		}

		public bool Contains(TriangleList item) {
			return _lists.Contains(item);
		}

		public void CopyTo(TriangleList[] array, int arrayIndex) {
			_lists.CopyTo(array, arrayIndex);
		}

		public bool Remove(TriangleList item) {
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

		IEnumerator<TriangleList> IEnumerable<TriangleList>.GetEnumerator() {
			return GetEnumerator();
		}

	    #region ITriangleList Members

		public override IList<VertexPositionNormalColorTexture> Vertices {
			get {
				lock(this) {
					if(_durty) {
						JoinAll();
					}
					return _vertices;
				}
			}
		}

        public override IList<int> Indices {
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

		public IEnumerator<TriangleList> GetEnumerator() {
			return _lists.GetEnumerator();
		}

		private void JoinAll() {
			_vertices.Clear();
			_indices.Clear();
			var offset = 0;
			foreach(var list in _lists) {
				foreach (var vertex in list.Vertices)
					_vertices.Add(vertex);

				foreach (var index in list.Indices)
					_indices.Add(index + offset);

				offset = _vertices.Count;
			}
			_durty = false;
		}
	}
}