using System;
using System.Graphics;

namespace Comet
{
	public class Canvas : View, IDrawable
	{
		public Action<ICanvas, RectangleF> Draw;
		public virtual void OnDraw(ICanvas canvas, RectangleF dirtyRect) => Draw?.Invoke(canvas, dirtyRect);
		void IDrawable.Draw(ICanvas canvas, RectangleF dirtyRect) => OnDraw(canvas, dirtyRect);
	}
}
