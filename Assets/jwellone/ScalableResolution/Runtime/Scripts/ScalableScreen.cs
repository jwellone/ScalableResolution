using UnityEngine;

#nullable enable

namespace jwellone
{
	public sealed class ScalableScreen : IScalableResolution
	{
		bool _isDirty = true;
		float _scale = 1f;

		public Vector2 basicSize => ScalableResolution.basicResolution.size;

		public float scale
		{
			get => _scale;
			set
			{
				_isDirty |= (value != _scale);
				_scale = value;
			}
		}

		public Vector2 size => scale * basicSize;

		public void SetDirty()
		{
			_isDirty = true;
		}

		public void Apply()
		{
			if (!_isDirty)
			{
				return;
			}

			_isDirty = false;

			var s = size;
			Screen.SetResolution((int)s.x, (int)s.y, false);
		}

		public void Dispose()
		{
			Screen.SetResolution((int)basicSize.x, (int)basicSize.y, false);
		}
	}
}
