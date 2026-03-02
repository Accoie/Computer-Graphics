using OpenTK.Graphics.OpenGL;

namespace Task3.Rendering
{
    public class FontRenderer
    {
        private const int CharacterSize = 8;
        private const int CharGap = 2;
        private const int LineGap = 4;
        
        public void DrawText(float x, float y, string text, float scale = 1.0f)
        {
            float charWidth = CharacterSize * scale;
            float charHeight = CharacterSize * scale;
            float startX = x;
            
            foreach (char c in text.ToUpper())
            {
                if (c == '\n')
                {
                    y -= charHeight + LineGap * scale;
                    x = startX;
                    
                    continue;
                }
                
                DrawCharacter(x, y, c, scale);
                x += charWidth + CharGap * scale;
            }
        }

        public void DrawTextCenteredOnArea(float areaX, float areaWidth, float y, string text, float scale = 1.0f)
        {
            float textWidth = MeasureTextWidth(text, scale);
            float x = areaX + (areaWidth - textWidth) / 2.0f;
            DrawText(x, y, text, scale);
        }

        private float MeasureTextWidth(string text, float scale = 1.0f)
        {
            float charWidth = CharacterSize * scale + CharGap * scale;
            
            return text.Length * charWidth;
        }
        
        private void DrawCharacter(float x, float y, char c, float scale)
        {
            byte[] bitmap = BitmapFont.GetCharacter(c);
            
            for (int row = 0; row < CharacterSize; row++)
            {
                for (int col = 0; col < CharacterSize; col++)
                {
                    if (IsPixelSet(bitmap, row, col))
                    {
                        DrawPixel(x, y, row, col, scale);
                    }
                }
            }
        }
        
        private static bool IsPixelSet(byte[] bitmap, int row, int col)
        {
            return (bitmap[row] & (1 << (CharacterSize - 1 - col))) != 0;
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
