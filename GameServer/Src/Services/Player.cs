using Dapper;
using GameServer.Config;

namespace GameServer.Services;

public class Player
{
    public static Models.Player Insert(Models.Player player)
    {
       player.Id= DB.Instance.Ctx.Execute("INSERT INTO Player (X,Y) VALUES (@X,@Y);",player);
       return player;
    }
    
    public static Models.Player[] GetPlayersInRange(float centerX, float centerY, float range)
    {
        try
        {
            return DB.Instance.Ctx.Query<Models.Player>(
                @"SELECT * FROM Player WHERE ((X - @X) * (X - @X) + (Y - @Y) * (Y - @Y)) <= @RadiusSquared",
    new {
                X = centerX,
                Y = centerY,
                RadiusSquared = range * range
            }).ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
           return null;
        }
    }

    public static void UpdatePlayer(Models.Player player)
    {
        try
        {
            DB.Instance.Ctx.Query<Models.Player>("UPDATE Player SET X=@X, Y=@Y WHERE Id=@Id",player); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
            
    }
}