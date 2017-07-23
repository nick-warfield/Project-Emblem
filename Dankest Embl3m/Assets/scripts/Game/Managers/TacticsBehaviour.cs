using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//I plan to use this script to derive all of my key functions from. Then I'll have other scripts inherit from here to access them.
public class TacticsBehaviour : MonoBehaviour
{
    //Checks if a pair of coordinates are on the map
    public bool BoundsCheck(Vector2 Coordinates, Terrain[,] Map)
    {
        if (Coordinates.x >= 0 && Coordinates.x < Map.GetLength(0) && Coordinates.y >= 0 && Coordinates.y < Map.GetLength(1))
        { return true; }
        else
        { return false; }
    }
    public bool BoundsCheck(int x, int y, Terrain[,] Map)
    {
        if (x >= 0 && x < Map.GetLength(0) && y >= 0 && y < Map.GetLength(1))
        { return true; }
        else
        { return false; }
    }

    //find the shortest path to every tile in range
    protected Terrain[] DijkstraAlgorithm(RPGClass Unit, Terrain[,] Map)
    {
        //Grab Starting Coordinates
        int x = Unit.x, y = Unit.y;

        List<Terrain> visited = new List<Terrain> { Map[x, y] };    //this list contains the tiles that form the shortest path
        List<Terrain> unvisited = new List<Terrain> { };        //this list contains the tiles being considered to add to the closed list

        Terrain mostRecent = Map[x, y];
        Terrain temp;

        mostRecent.EstimatedMovementCost = 0;

        do
        {
            //Add adjacent tiles to the list
            for (int i = 0; i < 4; i++)
            {
                //Grab a neighbor to the mostRecent tile
                temp = GrabNeighbors(i, mostRecent, Map);

                //if an existing tile was found, do some checks and update the lists accordingly
                if (temp != null)
                { UpdatePathfindingListsDijstra(temp, mostRecent, visited, unvisited, Unit); }
            }


            //sort list (lazily) so that the first result has the smallest H value
            for (int i = 1; i < unvisited.Count; i++)
            {
                if (unvisited[i].EstimatedMovementCost < unvisited[0].EstimatedMovementCost)
                {
                    temp = unvisited[0];
                    unvisited[0] = unvisited[i];
                    unvisited[i] = temp;
                }
            }

            visited.Add(unvisited[0]);              //add the unvisted tile with the smallest F value to the visited list
            unvisited.Remove(unvisited[0]);         //remove the added tile from the unvisited list
            mostRecent = visited.Last();   //update the mostrecent tile

        } while (unvisited.Count > 0);   //stop looping once the unvisited list becomes empty

        //reset the h values for future pathfindings... parent should not be reset so that it can be used in future.
        for (int i = 0; i < visited.Count; i++) { visited[i].EstimatedMovementCost = Mathf.Infinity; }

        return visited.ToArray();
    }
    protected Terrain[] DijkstraAlgorithm(Terrain StartTile, Terrain[,] Map)
    {
        //Grab Starting Coordinates
        int x = StartTile.x, y = StartTile.y;

        List<Terrain> visited = new List<Terrain> { Map[x, y] };    //this list contains the tiles that form the shortest path
        List<Terrain> unvisited = new List<Terrain> { };        //this list contains the tiles being considered to add to the closed list

        Terrain mostRecent = Map[x, y];
        Terrain temp;

        mostRecent.EstimatedMovementCost = 0;

        do
        {
            //Add adjacent tiles to the list
            for (int i = 0; i < 4; i++)
            {
                //Grab a neighbor to the mostRecent tile
                temp = GrabNeighbors(i, mostRecent, Map);

                //if an existing tile was found, do some checks and update the lists accordingly
                if (temp != null)
                { UpdatePathfindingListsDijstra(temp, mostRecent, visited, unvisited); }
            }


            //sort list (lazily) so that the first result has the smallest H value
            for (int i = 1; i < unvisited.Count; i++)
            {
                if (unvisited[i].EstimatedMovementCost < unvisited[0].EstimatedMovementCost)
                {
                    temp = unvisited[0];
                    unvisited[0] = unvisited[i];
                    unvisited[i] = temp;
                }
            }

            visited.Add(unvisited[0]);              //add the unvisted tile with the smallest F value to the visited list
            unvisited.Remove(unvisited[0]);         //remove the added tile from the unvisited list
            mostRecent = visited.Last();   //update the mostrecent tile

        } while (unvisited.Count > 0);   //stop looping once the unvisited list becomes empty

        //reset the h values for future pathfindings... parent should not be reset so that it can be used in future.
        for (int i = 0; i < visited.Count; i++) { visited[i].EstimatedMovementCost = Mathf.Infinity; }

        return visited.ToArray();
    }
    protected Terrain[] DijkstraAlgorithm(Terrain StartTile, int Range, Terrain[,] Map)
    {
        //Grab Starting Coordinates
        int x = StartTile.x, y = StartTile.y;

        List<Terrain> visited = new List<Terrain> { Map[x, y] };    //this list contains the tiles that form the shortest path
        List<Terrain> unvisited = new List<Terrain> { };        //this list contains the tiles being considered to add to the closed list

        Terrain mostRecent = Map[x, y];
        Terrain temp;

        mostRecent.EstimatedMovementCost = 0;

        do
        {
            //Add adjacent tiles to the list
            for (int i = 0; i < 4; i++)
            {
                //Grab a neighbor to the mostRecent tile
                temp = GrabNeighbors(i, mostRecent, Map);

                //if an existing tile was found, do some checks and update the lists accordingly
                if (temp != null)
                { UpdatePathfindingListsDijstra(temp, mostRecent, visited, unvisited, Range); }
            }


            //sort list (lazily) so that the first result has the smallest H value
            for (int i = 1; i < unvisited.Count; i++)
            {
                if (unvisited[i].EstimatedMovementCost < unvisited[0].EstimatedMovementCost)
                {
                    temp = unvisited[0];
                    unvisited[0] = unvisited[i];
                    unvisited[i] = temp;
                }
            }

            visited.Add(unvisited[0]);              //add the unvisted tile with the smallest F value to the visited list
            unvisited.Remove(unvisited[0]);         //remove the added tile from the unvisited list
            mostRecent = visited.Last();   //update the mostrecent tile

        } while (unvisited.Count > 0);   //stop looping once the unvisited list becomes empty

        //reset the h values for future pathfindings... parent should not be reset so that it can be used in future.
        for (int i = 0; i < visited.Count; i++) { visited[i].EstimatedMovementCost = Mathf.Infinity; }

        return visited.ToArray();
    }
    protected Terrain[] DijkstraAlgorithm(Terrain StartTile, int MinimumRange, int MaximumRange, Terrain[,] Map)
    {
        //Grab Starting Coordinates
        int x = StartTile.x, y = StartTile.y;

        List<Terrain> visited = new List<Terrain> { };    //this list contains the tiles that form the shortest path
        List<Terrain> unvisited = new List<Terrain> { };        //this list contains the tiles being considered to add to the closed list

        Terrain mostRecent = Map[x, y];
        Terrain temp;

        mostRecent.EstimatedMovementCost = 0;

        do
        {
            //Add adjacent tiles to the list
            for (int i = 0; i < 4; i++)
            {
                //Grab a neighbor to the mostRecent tile
                temp = GrabNeighbors(i, mostRecent, Map);

                //if an existing tile was found and it was far enough away from the start, do some checks and update the lists accordingly
                if (temp != null)
                { UpdatePathfindingListsDijstra(temp, mostRecent, visited, unvisited, MinimumRange, MaximumRange); }
            }


            //sort list (lazily) so that the first result has the smallest H value
            for (int i = 1; i < unvisited.Count; i++)
            {
                if (unvisited[i].EstimatedMovementCost < unvisited[0].EstimatedMovementCost)
                {
                    temp = unvisited[0];
                    unvisited[0] = unvisited[i];
                    unvisited[i] = temp;
                }
            }

            visited.Add(unvisited[0]);              //add the unvisted tile with the smallest F value to the visited list
            unvisited.Remove(unvisited[0]);         //remove the added tile from the unvisited list
            mostRecent = visited.Last();   //update the mostrecent tile

        } while (unvisited.Count > 0);   //stop looping once the unvisited list becomes empty

        //reset the h values for future pathfindings... parent should not be reset so that it can be used in future.
        for (int i = 0; i < visited.Count; i++) { visited[i].EstimatedMovementCost = Mathf.Infinity; }

        return visited.ToArray();
    }

    //Readjusts the path to be the shortest one via following tile parents
    protected Terrain[] ShortenPath (Terrain EndLocation, Terrain StartLocation)
    {
        //Create a new Path, add the tile we want to end at to it
        List<Terrain> NewPath = new List<Terrain> { EndLocation };

        //Then we add the parent of the last tile added until we have added the starting tile
        do
        { NewPath.Add(NewPath.Last().Parent); }
        while
        (NewPath.Last() != StartLocation);

        //Then reverse the list so that the starting tile is the first index
        NewPath.Reverse();

        //Then convert and return
        return NewPath.ToArray();
    }
    protected Terrain[] ShortenPath (Terrain EndLocation, Terrain[] CurrentPath)
    {
        //Create a new path
        List<Terrain> NewPath = new List<Terrain> {  };

        //loop through the existing path until the endtile is found
        int i = 0;
        do
        { NewPath.Add(CurrentPath[i]); i++; }
        while
        (NewPath.Last() != EndLocation);

        //convert and return
        return NewPath.ToArray();
    }

    //Custom contain function so that I don't need Linq in other scripts
    protected bool PathContains (Terrain Tile, Terrain[] Path)
    {
        return Path.Contains(Tile);
    }
    protected bool AdjacentTiles (Terrain Tile1, Terrain Tile2)
    {
        if (((Tile1.x - 1 == Tile2.x || Tile1.x + 1 == Tile2.x) && Tile1.y == Tile2.y) ^ 
            ((Tile1.y - 1 == Tile2.y || Tile1.y + 1 == Tile2.y) && Tile1.x == Tile2.x) )
        { return true; }
        else
        { return false; }
    }

    //Resets Parent and Estimated Movement Cost on each Tile stored in the map
    protected void ResetTerrainPathfindingVariables(Terrain[] Path)
    {
        for (int i = 0; i < Path.Length; i++)
        {
            Path[i].EstimatedMovementCost = Mathf.Infinity;
            Path[i].Parent = null;
        }
    }
    protected void ResetTerrainPathfindingVariables (Terrain[,] Map)
    {
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                Map[i, j].EstimatedMovementCost = Mathf.Infinity;
                Map[i, j].Parent = null;
            }
        }
    }



    //HELPER FUNCTIONS\\

    //Checks if a neighbor is in bounds and returns it. For use with for (int i = 0; i < 4; i++). Values below 0 and above 3 return null values always.
    private Terrain GrabNeighbors(int i, Terrain Tile, Terrain[,] Map)
    {
        int x = Tile.x, y = Tile.y;
        switch (i)
        {
            case 0: if (BoundsCheck(x + 1, y, Map)) { return Map[x + 1, y]; } else { return null; }     //right tile
            case 1: if (BoundsCheck(x - 1, y, Map)) { return Map[x - 1, y]; } else { return null; }     //left tile
            case 2: if (BoundsCheck(x, y + 1, Map)) { return Map[x, y + 1]; } else { return null; }     //above tile
            case 3: if (BoundsCheck(x, y - 1, Map)) { return Map[x, y - 1]; } else { return null; }     //below tile
            default: return null;       //in case temp does not get assigned for some reason
        }
    }
    private Terrain GrabNeighbors(int i, int x, int y, Terrain[,] Map)
    {
        //print("i: " + i + ", x: " + x + ", y: " + y);
        switch (i)
        {
            case 0: if (BoundsCheck(x + 1, y, Map)) { return Map[x + 1, y]; } else { return null; }     //right tile
            case 1: if (BoundsCheck(x - 1, y, Map)) { return Map[x - 1, y]; } else { return null; }     //left tile
            case 2: if (BoundsCheck(x, y + 1, Map)) { return Map[x, y + 1]; } else { return null; }     //above tile
            case 3: if (BoundsCheck(x, y - 1, Map)) { return Map[x, y - 1]; } else { return null; }     //below tile
            default: return null;       //in case temp does not get assigned for some reason
        }
    }

    //updates the visited and unvisited lists, as well as set key pathfinding variables attatched to added tile as applicable
    private void UpdatePathfindingListsDijstra(Terrain PossibleTile, Terrain PossibleParent, List<Terrain> VisitedTiles, List<Terrain> UnvisitedTiles)
    {
        //ignore the tile if i have already visited it
        if (!VisitedTiles.Contains(PossibleTile))
        {
            //if the checked tile is already on the unvisited list, check to see if this new path is shorter
            if (UnvisitedTiles.Contains(PossibleTile))
            {
                if (PossibleParent.EstimatedMovementCost < PossibleTile.EstimatedMovementCost)
                {
                    PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost;
                    PossibleTile.Parent = PossibleParent;
                }
            }
            //otherwise, add it to the unvisited list
            else
            {
                UnvisitedTiles.Add(PossibleTile);                //add to unvisited list
                PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost;     //update estimated cost for future comparison
                PossibleTile.Parent = PossibleParent;           //update parent so a path can be formed as needed
            }
        }
    }
    private void UpdatePathfindingListsDijstra(Terrain PossibleTile, Terrain PossibleParent, List<Terrain> VisitedTiles, List<Terrain> UnvisitedTiles, int Range)
    {
        //ignore the tile if i have already visited it, or there is not enough movement to pay for it
        if (!VisitedTiles.Contains(PossibleTile) && PossibleParent.EstimatedMovementCost + 1 <= Range)
        {
            //if the checked tile is already on the unvisited list, check to see if this new path is shorter
            if (UnvisitedTiles.Contains(PossibleTile))
            {
                if (PossibleParent.EstimatedMovementCost < PossibleTile.EstimatedMovementCost)
                {
                    PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost;
                    PossibleTile.Parent = PossibleParent;
                }
            }
            //otherwise, add it to the unvisited list
            else
            {
                UnvisitedTiles.Add(PossibleTile);                //add to unvisited list
                PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost + 1;     //update estimated cost for future comparison
                PossibleTile.Parent = PossibleParent;           //update parent so a path can be formed as needed
            }
        }
    }
    private void UpdatePathfindingListsDijstra(Terrain PossibleTile, Terrain PossibleParent, List<Terrain> VisitedTiles, List<Terrain> UnvisitedTiles, int MinRange, int MaxRange)
    {
        //ignore the tile if i have already visited it, or there is not enough movement to pay for it
        if (!VisitedTiles.Contains(PossibleTile) && PossibleParent.EstimatedMovementCost + 1 <= MaxRange && PossibleParent.EstimatedMovementCost + 1 >= MinRange)
        {
            //if the checked tile is already on the unvisited list, check to see if this new path is shorter
            if (UnvisitedTiles.Contains(PossibleTile))
            {
                if (PossibleParent.EstimatedMovementCost < PossibleTile.EstimatedMovementCost)
                {
                    PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost;
                    PossibleTile.Parent = PossibleParent;
                }
            }
            //otherwise, add it to the unvisited list
            else
            {
                UnvisitedTiles.Add(PossibleTile);                //add to unvisited list
                PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost + 1;     //update estimated cost for future comparison
                PossibleTile.Parent = PossibleParent;           //update parent so a path can be formed as needed
            }
        }
    }
    private void UpdatePathfindingListsDijstra(Terrain PossibleTile, Terrain PossibleParent, List<Terrain> VisitedTiles, List<Terrain> UnvisitedTiles, RPGClass Unit)
    {
        float AdjusteTileCost = Unit.AdjustTileMovementCost(PossibleTile);

        //ignore the tile if i have already visited it, or there is not enough movement to pay for it
        if (!VisitedTiles.Contains(PossibleTile) && PossibleParent.EstimatedMovementCost + AdjusteTileCost <= Unit.Stats[(int)RPGClass.Stat.Move].dynamicValue)
        {
            //if the checked tile is already on the unvisited list, check to see if this new path is shorter
            if (UnvisitedTiles.Contains(PossibleTile))
            {
                if (PossibleParent.EstimatedMovementCost + AdjusteTileCost < PossibleTile.EstimatedMovementCost)
                {
                    PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost + AdjusteTileCost;
                    PossibleTile.Parent = PossibleParent;
                }
            }
            //otherwise, add it to the unvisited list
            else
            {
                UnvisitedTiles.Add(PossibleTile);                //add to unvisited list
                PossibleTile.EstimatedMovementCost = PossibleParent.EstimatedMovementCost + AdjusteTileCost;     //update estimated cost for future comparison
                PossibleTile.Parent = PossibleParent;           //update parent so a path can be formed as needed
            }
        }
    }

}

