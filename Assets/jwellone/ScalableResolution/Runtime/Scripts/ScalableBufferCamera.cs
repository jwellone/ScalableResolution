using UnityEngine;

#nullable enable

namespace jwellone
{
	public sealed class ScalableBufferCamera : ScalableResolution
	{
		public ScalableBufferCamera(in Camera camera)
			: base(camera)
		{
			camera.allowDynamicResolution = true;
		}

		protected override void OnApply()
		{
			ScalableBufferManager.ResizeBuffers(scale, scale);
		}

		protected override void OnDispose()
		{
			if (_camera != null)
			{
				_camera.allowDynamicResolution = false;
			}
			ScalableBufferManager.ResizeBuffers(1f, 1f);
		}
	}
}