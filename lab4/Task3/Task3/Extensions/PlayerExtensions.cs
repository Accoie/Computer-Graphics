using Task3.Models;

namespace Task3.Extensions;

public static class PlayerExtensions
{
    public static bool IsColliding(this Player player, float x, float z)
    {
        for (float dx = -player.Radius; dx <= player.Radius; dx += player.Radius)
        {
            for (float dz = -player.Radius; dz <= player.Radius; dz += player.Radius)
            {
                if (CheckPoint(x + dx, z + dz))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    private static bool CheckPoint(float x, float z)
    {
        int ix = (int)Math.Floor(x);
        int iz = (int)Math.Floor(z);

        bool outOfLabyrinth = ix < 0 || ix >= GameConfig.World.LabyrinthMap.GetLength(1) || iz < 0 ||
                 iz >= GameConfig.World.LabyrinthMap.GetLength(0);
        
        if (outOfLabyrinth)
        {
            return true;
        }
        
        return GameConfig.World.LabyrinthMap[iz, ix] == 1;
    }
}