using System.Collections.Generic;
using Farmer;

//
public class World : Singleton<World>
{
    private List<ConstructionMono> constructionInWorld = new List<ConstructionMono>();

    public void AddConstructionToWorld(ConstructionMono constructionMono)
    {
        if (!constructionInWorld.Contains(constructionMono))
        {
            constructionInWorld.Add(constructionMono);
        }
    }

    public void RemoveConstructionToWorld(ConstructionMono constructionMono)
    {
        if (constructionInWorld.Contains(constructionMono))
        {
            constructionInWorld.Remove(constructionMono);
        }
    }
}