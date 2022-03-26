using UnityEngine;
using UnityEngine.Rendering;

#nullable enable

namespace jwellone
{
	public sealed class ScalableCommandBufferCamera : ScalableResolution
	{
		int _depth = 24;
		RenderTextureFormat _format = RenderTextureFormat.ARGB32;
		FilterMode _filterMode = FilterMode.Bilinear;

		Camera? _commandBufferCamera;
		RenderTexture? _renderTexture;
		CommandBuffer? _commandBuffer;

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

		public ScalableCommandBufferCamera(in Camera camera, int depth = 24, RenderTextureFormat format = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Bilinear)
			: base(camera)
		{
			_depth = depth;
			_format = format;
			_filterMode = filterMode;

			var go = new GameObject($"CommandBufferCamera({camera.name})");
			go.hideFlags = HideFlags.HideAndDontSave;
			_commandBufferCamera = go.AddComponent<Camera>();
			_commandBufferCamera.cullingMask = 0;
			_commandBufferCamera.renderingPath = RenderingPath.VertexLit;
			_commandBufferCamera.allowHDR = false;
			_commandBufferCamera.allowMSAA = false;
			_commandBufferCamera.useOcclusionCulling = false;
			_commandBufferCamera.clearFlags = CameraClearFlags.Nothing;
		}

		protected override void OnApply()
		{
			ReleaseRT();
			_renderTexture = new RenderTexture((int)size.x, (int)size.y, _depth, _format);
			_renderTexture.name = "RT_CommandBufferCamera";
			_renderTexture.useMipMap = false;
			_renderTexture.filterMode = _filterMode;
			_renderTexture.antiAliasing = 8;

			if (_camera != null)
			{
				_camera.targetTexture = _renderTexture;
			}

			RemoveCommandBuffer();
			_commandBuffer = new CommandBuffer();
			_commandBuffer.SetRenderTarget(-1);
			_commandBuffer.Blit((RenderTargetIdentifier)_renderTexture, BuiltinRenderTextureType.CameraTarget);
			_commandBufferCamera?.AddCommandBuffer(CameraEvent.AfterEverything, _commandBuffer);
		}

		protected override void OnDispose()
		{
			ReleaseRT();
			RemoveCommandBuffer();

			if (_commandBufferCamera != null)
			{
				GameObject.Destroy(_commandBufferCamera.gameObject);
				_commandBufferCamera = null;
			}
		}

		private void ReleaseRT()
		{
			if (_camera != null)
			{
				_camera.targetTexture = null;
			}

			if (_renderTexture != null)
			{
				_renderTexture.Release();
				Texture.Destroy(_renderTexture);
				_renderTexture = null;
			}
		}

		private void RemoveCommandBuffer()
		{
			if (_commandBuffer == null)
			{
				return;
			}

			if (_commandBufferCamera != null)
			{
				_commandBufferCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, _commandBuffer);
			}

			_commandBuffer = null;
		}
	}
}
