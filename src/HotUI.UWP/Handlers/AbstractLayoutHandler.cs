using System;
using System.Collections.Generic;
using System.Drawing;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HotUI.Layout;
using Size = Windows.Foundation.Size;

namespace HotUI.UWP.Handlers
{
    public abstract class AbstractLayoutHandler : Canvas, IUIElement, ILayoutHandler<UIElement>
    {
        private readonly ILayoutManager<UIElement> _layoutManager;
        private AbstractLayout _view;

        protected AbstractLayoutHandler(ILayoutManager<UIElement> layoutManager)
        {
            _layoutManager = layoutManager;
        }

        public SizeF GetSize(UIElement view)
        {
            if (view.RenderSize.Width <= 0 && view.RenderSize.Height <= 0) return view.DesiredSize.ToSizeF();

            return view.RenderSize.ToSizeF();
        }

        public void SetFrame(UIElement view, float x, float y, float width, float height)
        {
            if (width > 0 && height > 0)
            {
                if (view is Windows.UI.Xaml.Controls.ListView listview)
                {
                    listview.InvalidateMeasure();
                    view.Arrange(new Rect(x, y, width, height));
                    listview.Width = width;
                    listview.Height = height;
                }
                else
                {
                    view.Arrange(new Rect(x, y, width, height));
                }
            }
        }

        public void SetSize(UIElement view, float width, float height)
        {
            if (view is FrameworkElement element)
            {
                element.Width = width;
                element.Height = height;
            }
        }

        public IEnumerable<UIElement> GetSubviews()
        {
            foreach (var element in Children) yield return element;
        }

        public UIElement View => this;

        public void SetView(View view)
        {
            _view = view as AbstractLayout;
            if (_view != null)
            {
                _view.ChildrenChanged += HandleChildrenChanged;
                _view.ChildrenAdded += HandleChildrenAdded;
                _view.ChildrenRemoved += ViewOnChildrenRemoved;

                foreach (var subView in _view)
                {
                    var nativeView = subView.ToView();
                    Children.Add(nativeView);
                }

                LayoutSubviews();
            }
        }

        public void Remove(View view)
        {
            Children.Clear();

            if (view != null)
            {
                _view.ChildrenChanged -= HandleChildrenChanged;
                _view.ChildrenAdded -= HandleChildrenAdded;
                _view.ChildrenRemoved -= ViewOnChildrenRemoved;
                _view = null;
            }
        }

        public virtual void UpdateValue(string property, object value)
        {
        }

        private void HandleChildrenAdded(object sender, LayoutEventArgs e)
        {
            for (var i = 0; i < e.Count; i++)
            {
                var index = e.Start + i;
                var view = _view[index];
                var nativeView = view.ToView();
                Children.Insert(index, nativeView);
            }

            LayoutSubviews();
        }

        private void ViewOnChildrenRemoved(object sender, LayoutEventArgs e)
        {
            for (var i = 0; i < e.Count; i++)
            {
                var index = e.Start + i;
                Children.RemoveAt(index);
            }

            LayoutSubviews();
        }

        private void HandleChildrenChanged(object sender, LayoutEventArgs e)
        {
            for (var i = 0; i < e.Count; i++)
            {
                var index = e.Start + i;
                Children.RemoveAt(index);

                var view = _view[index];
                var newNativeView = view.ToView();
                Children.Insert(index, newNativeView);
            }

            LayoutSubviews();
        }

        private void LayoutSubviews()
        {
            _layoutManager.Layout(this, this, _view);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size();

            foreach (var child in Children)
            {
                if (child is Windows.UI.Xaml.Controls.ListView listView)
                {
                    var sizeToUse = GetMeasuredSize(listView, availableSize);
                    child.Measure(sizeToUse);
                    size.Height = Math.Max(child.DesiredSize.Height, size.Height);
                    size.Width = Math.Max(child.DesiredSize.Width, size.Width);
                }
                else
                {
                    child.Measure(availableSize);
                    size.Height = Math.Max(child.DesiredSize.Height, size.Height);
                    size.Width = Math.Max(child.DesiredSize.Width, size.Width);
                }
            }

            return size;
        }

        protected virtual Size GetMeasuredSize(UIElement child, Size availableSize)
        {
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Width = finalSize.Width;
            Height = finalSize.Height;
            if (finalSize.Width > 0 && finalSize.Height > 0) LayoutSubviews();

            return finalSize;
        }
    }
}