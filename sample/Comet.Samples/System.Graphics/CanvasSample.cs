using System;
using System.Graphics;

namespace Comet.Samples
{
	public class CanvasSample : View
	{
		[Body]
		View body() => new Canvas
		{
			Draw = (canvas,rect) => {
				canvas.StrokeColor = Colors.LightGrey;
				canvas.DrawRectangle(50.5f, 50.5f, 50, 50);
				var path = new PathF(50.5f, 50.5f);
				path.LineTo(100.5f, 100.5f);
				canvas.StrokeColor = Colors.Black;
				canvas.DrawPath(path);
			}
		};
	}
}
