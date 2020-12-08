using System;
using System.Drawing;
using System.Graphics.CoreGraphics;
using Comet.iOS.Handlers;

namespace Comet.iOS
{
	public class CanvasHandler : AbstractControlHandler<Canvas, NativeGraphicsView>
	{
		public static readonly PropertyMapper<Canvas> Mapper = new PropertyMapper<Canvas>(ViewHandler.Mapper);

		public CanvasHandler() : base(Mapper)
		{

		}

		public override SizeF GetIntrinsicSize(SizeF availableSize) => availableSize;

		protected override NativeGraphicsView CreateView() => new NativeGraphicsView();
		public override void SetView(View view)
		{
			base.SetView(view);
			this.TypedNativeView.Drawable = this.VirtualView;
		}
		protected override void DisposeView(NativeGraphicsView nativeView) => throw new NotImplementedException();
	}
}
