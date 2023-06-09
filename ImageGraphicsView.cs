using IImage = Microsoft.Maui.Graphics.IImage;

namespace VietOCR.Drawables
{
    internal class ImageGraphicsView : GraphicsView
    {
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(IImage), typeof(LoadImageDrawable), propertyChanged: ImagePropertyChanged);

        public IImage Image
        {
            get => (IImage)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public ImageGraphicsView()
        {
            BackgroundColor = Colors.Transparent;
            //Loaded += View_Loaded;
        }

        //private void View_Loaded(object sender, EventArgs e)
        //{
        //    System.Reflection.Assembly assembly = typeof(Microsoft.Maui.Graphics.Win2D.W2DCanvas).Assembly;
        //    var type = assembly.GetType("Microsoft.Maui.Graphics.Win2D.W2DGraphicsService");
        //    var prop = type.GetProperty("GlobalCreator");

        //    var graphicsView = (GraphicsView)sender;
        //    var view = (Microsoft.Maui.Platform.PlatformTouchGraphicsView)graphicsView.Handler.PlatformView;
        //    var view2 = (Microsoft.Maui.Graphics.Win2D.W2DGraphicsView)view.Content;
        //    prop.SetValue(null, view2.Content);
        //}

        private static void ImagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not ImageGraphicsView { Drawable: LoadImageDrawable drawable } view)
            {
                return;
            }

            drawable.Image = (IImage) newValue;
            view.Invalidate();
        }
    }
}
