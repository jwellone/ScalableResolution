using System;
using UnityEngine;

#nullable enable

namespace jwellone
{
	public interface IScalableResolution : IDisposable
	{
		float scale { get; set; }
		Vector2 size { get; }
		void SetDirty();
		void Apply();
	}

	public interface IScalableBasicResolution
	{
		Vector2 size { get; }
	}

	public abstract class ScalableResolution : IScalableResolution
	{
		class DefaultBasicResolution : IScalableBasicResolution
		{
			public Vector2 size
			{
				get;
				private set;
			} = new Vector2(Screen.width, Screen.height);
		}


		bool _disposed;
		bool _isDirty = true;
		float _scale = 1f;
		protected Camera? _camera;

		public float scale
		{
			get => _scale;
			set
			{
				_isDirty |= (_scale != value);
				_scale = value;
			}
		}

		public Vector2 size => scale * basicResolution.size;

		public static IScalableBasicResolution basicResolution
		{
			get;
			set;
		} = new DefaultBasicResolution();

		public ScalableResolution(in Camera camera)
		{
			_camera = camera;
		}

		~ScalableResolution()
		{
			Dispose(false);
		}

		public void SetDirty()
		{
			_isDirty = true;
		}

		public void Apply()
		{
			if (_isDirty)
			{
				_isDirty = false;
				OnApply();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool isDisposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			OnDispose();
		}

		protected abstract void OnApply();
		protected abstract void OnDispose();
	}
}