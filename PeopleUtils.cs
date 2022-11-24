using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleUtils
{

    public static Hashtable road_location(CityCell road)
    {

        return new Hashtable{
            {"x", road.x},
            {"y", road.y},
            {"g", 99999},
            {"f", 99999},
            {"parent", null},
            {"left", false},
            {"right", false},
            {"up", false},
            {"down", false}
        };

    }

    // Find coords of each neighbour and check if road
    public static Hashtable add_neighbour_directions(List<CityCell> road_cells, Hashtable road_location)
    {
        
        foreach (CityCell road in road_cells)
        {

            bool left = (int)road_location["x"] == road.x+1 && (int)road_location["y"] == road.y;
            bool right = (int)road_location["x"] == road.x-1 && (int)road_location["y"] == road.y;
            bool up = (int)road_location["x"] == road.x && (int)road_location["y"] == road.y+1;
            bool down = (int)road_location["x"] == road.x && (int)road_location["y"] == road.y-1;

            if (left)
            {
                road_location["left"] = true;
            }
            else if (right)
            {
                road_location["right"] = true;
            }
            else if (up)
            {
                road_location["up"] = true;
            }
            else if (down)
            {
                road_location["down"] = true;
            }

        }

        return road_location;
    }

}
