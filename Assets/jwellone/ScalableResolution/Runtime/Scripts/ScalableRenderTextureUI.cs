using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace jwellone
{
	public sealed class ScalableRenderTextureUI : ScalableResolution
	{
		RawImage _rawImage = null!;
		int _depth;
		FilterMode _filterMode;
		RenderTextureFormat _format;

		RenderTexture? _renderTexture = null;

		public int depth
		{
			get => _depth;
			set
			{
				if (_depth != value)
				{
					_depth = value;
					SetDirty();
				}
			}
		}

		public RenderTextureFormat format
		{
			get => _format;
			set
			{
				if (_format != value)
				{
					_format = value;
					SetDirty();
				}
			}
		}

		public FilterMode filterMode
		{
			get => _filterMode;
			set
			{
				if (_filterMode != value)
				{
					_filterMode = value;
					SetDirty();
				}
			}
		}

		public ScalableRenderTextureUI(in Camera camera, in RawImage rawImage, int depth = 24, RenderTextureFormat format = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Bilinear)
			: base(camera)
		{
			_rawImage = rawImage;
			_depth = depth;
			_filterMode = filterMode;
			_format = format;
		}

		protected override void OnApply()
		{
			ReleaseRT();

			var s = size;
			_renderTexture = new RenderTexture((int)s.x, (int)s.y, _depth, _format);
			_renderTexture.name = "RT_ScalableRenderTextureCamera";
			_renderTexture.useMipMap = false;
			_renderTexture.filterMode = _filterMode;

			if (_camera != null)
			{
				_camera.targetTexture = _renderTexture;
			}
			_rawImage.texture = _renderTexture;
		}

		protected override void OnDispose()
		{
			ReleaseRT();
		}

		void ReleaseRT()
		{
			if (_rawImage != null)
			{
				_rawImage.texture = null;
			}

			if (_camera != null)
			{
				_camera.targetTexture = null;
			}

			if (_renderTexture == null)
			{
				return;
			}

			_renderTexture.Release();
			Texture.Destroy(_renderTexture);
			_renderTexture = null;
		}
	}
}