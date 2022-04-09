using System;
using System.Collections.Generic;
using System.Drawing;

namespace SwIconGenerator
{
    public static class IconGeneratorUtil
    {
		public static Bitmap CombineBitmap(List<string> files, int iconSize)
		{
			var images = new List<Bitmap>();
			Bitmap result = null;
			try
			{
				int width = iconSize * files.Count;
				int iconBarSize = iconSize;
				result = new Bitmap(width, iconBarSize);
				using (Graphics g = Graphics.FromImage(result))
				{
					g.Clear(Color.Transparent);
					int offset = 0;
                    foreach (var file in files)
                    {
                        var iconBitmap = new Bitmap(file);
                        images.Add(iconBitmap);
                        float num = (float)iconSize / (float)Math.Max(iconBitmap.Width, iconBitmap.Height);
                        g.DrawImage(iconBitmap, new Rectangle(offset, 0, (int)(num * (float)iconBitmap.Width), (int)(num * (float)iconBitmap.Height)));
                        offset += iconSize;
                    }
				}
			}
			catch (Exception)
			{
			    result?.Dispose();
				throw;
			}
			finally
			{
                foreach (var image in images)
                {
					image.Dispose();
                }
			}
			return result;
		}
	}
}
