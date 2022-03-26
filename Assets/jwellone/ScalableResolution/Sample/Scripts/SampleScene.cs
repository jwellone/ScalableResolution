using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace jwellone
{
	public class SampleScene : MonoBehaviour
	{
		public enum Kind
		{
			Nothing,
			ScalableScreen,
			ScalableBuffer,
			ScalableCommandBuffer,
			ScalableRenderTextureUI
		}

		class Nothing : IScalableResolution
		{
			public float scale { get; set; }
			public Vector2 size => new Vector2(Screen.width, Screen.height);
			public void SetDirty() { }
			public void Apply() { }
			public void Dispose() { }
		}

		[SerializeField] Camera _camera = null!;
		[SerializeField] RawImage _rawImage = null!;
		[SerializeField] Text _text = null!;
		[SerializeField] Slider _slider = null!;
		[SerializeField] Dropdown _dropdown = null!;

		IScalableResolution? _scalable;

		Kind _kind
		{
			get
			{
				var type = _scalable?.GetType();
				if (type == typeof(ScalableScreen))
				{
					return Kind.ScalableScreen;
				}
				else if (type == typeof(ScalableBufferCamera))
				{
					return Kind.ScalableBuffer;
				}
				else if (type == typeof(ScalableCommandBufferCamera))
				{
					return Kind.ScalableCommandBuffer;
				}
				else if (type == typeof(ScalableRenderTextureUI))
				{
					return Kind.ScalableRenderTextureUI;
				}

				return Kind.Nothing;
			}
		}

		void Awake()
		{
			Build();
			Apply();

			_dropdown.options.Clear();
			foreach (var kind in Enum.GetNames(typeof(Kind)))
			{
				_dropdown.options.Add(new Dropdown.OptionData(kind.ToString()));
			}
		}

		void OnDestroy()
		{
			_scalable?.Dispose();
			_scalable = null;
		}

		void Update()
		{
			var sb = new StringBuilder();
			sb.Append("kind:").AppendLine(_kind.ToString());
			sb.Append("basicSize:").AppendLine(ScalableResolution.basicResolution.size.ToString());
			sb.Append("scale:").AppendLine(_scalable?.scale.ToString());
			sb.Append("size:").AppendLine(_scalable?.size.ToString());
			_text.text = sb.ToString();

			var camTransform = _camera.transform;
			camTransform.position = Quaternion.Euler(0, Time.deltaTime * 25f, 0) * camTransform.position;
			camTransform.LookAt(Vector3.zero);
		}

		void Apply()
		{
			_scalable?.Apply();
		}

		void Build()
		{
			var selectKind = (Kind)_dropdown.value;
			if (_kind == selectKind)
			{
				if (_scalable != null)
				{
					_scalable.scale = _slider.value;
				}
				return;
			}

			_scalable?.Dispose();
			_rawImage.enabled = false;

			switch (selectKind)
			{
				case Kind.ScalableScreen:
					_scalable = new ScalableScreen();
					break;

				case Kind.ScalableBuffer:
					_scalable = new ScalableBufferCamera(_camera);
					break;

				case Kind.ScalableCommandBuffer:
					_scalable = new ScalableCommandBufferCamera(_camera);
					break;

				case Kind.ScalableRenderTextureUI:
					_rawImage.enabled = true;
					_scalable = new ScalableRenderTextureUI(_camera, _rawImage);
					break;

				default:
					_scalable = new Nothing();
					break;
			}

			_scalable.scale = _slider.value;
		}

		public void OnClickApply()
		{
			Build();
			Apply();
		}
	}
}
