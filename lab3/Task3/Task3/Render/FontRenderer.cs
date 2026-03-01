using OpenTK.Graphics.OpenGL;

namespace Task3.Render
{
    public class FontRenderer
    {
        private const int GlyphSize = 8;
        
        public void DrawText(float x, float y, string text, float scale = 1.0f)
        {
            float charWidth = GlyphSize * scale;
            float charHeight = GlyphSize * scale;
            float startX = x;
            
            foreach (char c in text.ToUpper())
            {
                if (c == '\n')
                {
                    y -= charHeight + 4 * scale;
                    x = startX;
                    
                    continue;
                }
                
                DrawCharacter(x, y, c, scale);
                x += charWidth + 2 * scale;
            }
        }

        public void DrawTextCenteredOnArea(float areaX, float areaWidth, float y, string text, float scale = 1.0f)
        {
            float textWidth = MeasureText(text, scale);
            float x = areaX + (areaWidth - textWidth) / 2.0f;
            DrawText(x, y, text, scale);
        }

        private float MeasureText(string text, float scale = 1.0f)
        {
            float charWidth = GlyphSize * scale + 2 * scale;
            
            return text.Length * charWidth;
        }
        
        private void DrawCharacter(float x, float y, char c, float scale)
        {
            byte[] bitmap = BitmapFont.GetGlyph(c);
            
            for (int row = 0; row < GlyphSize; row++)
            {
                for (int col = 0; col < GlyphSize; col++)
                {
                    if (IsPixelSet(bitmap, row, col))
                    {
                        DrawPixel(x, y, row, col, scale);
                    }
                }
            }
        }
        
        private bool IsPixelSet(byte[] bitmap, int row, int col)
        {
            return (bitmap[row] & (1 << (GlyphSize - 1 - col))) != 0;
        }
        
        private void DrawPixel(float x, float y, int row, int col, float scale)
        {
            float px = x + col * scale;
            float py = y - row * scale;
            
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(px, py);
            GL.Vertex2(px + scale, py);
            GL.Vertex2(px + scale, py + scale);
            GL.Vertex2(px, py + scale);
            GL.End();
        }
    }
}
