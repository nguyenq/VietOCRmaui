using IImage = Microsoft.Maui.Graphics.IImage;

namespace VietOCR.Drawables
{
    internal class LoadImageDrawable : IDrawable
    {
        public IImage Image { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (Image != null)
            {
                canvas.DrawImage(Image, 10, 10, Image.Width, Image.Height);
            }
        }
    }
}
