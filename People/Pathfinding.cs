using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

    public static bool a_star(Person person, Hashtable start, Hashtable end, List<Hashtable> road_map)
    {

        bool end_in_road_map = false;
        foreach (Hashtable road in road_map)
        {
            if ((int)road["x"] == (int)start["x"] && (int)road["y"] == (int)start["y"])
            {
                start = road;
                break;
            }
            if ((int)road["x"] == (int)end["x"] && (int)road["y"] == (int)end["y"])
            {
                end_in_road_map = true;
            }
        }

        if (!end_in_road_map)
        {
            road_map.Add(
                end
            );
        }

        List<Hashtable> priority_open_set = new List<Hashtable>();


        start["g"] = 0;
        start["f"] = heuristic(start, end);

        priority_open_set.Add(
            start  //Talk about PeopleUtils, init used hashtables (no unique for coords)
        );

        Hashtable current = priority_open_set[0];

        while (priority_open_set.Count > 0)
        {

            current = priority_open_set[0];
            if ((int)current["x"] == (int)end["x"] && (int)current["y"] == (int)end["y"])
            {
                person.temp_path = new List<int[]>();
                person.temp_path.Insert(0, new int[]{ (int)current["x"], (int)current["y"] });
                save_path(person, current);
                return true;;
            }

            priority_open_set.RemoveAt(0);

            List<Hashtable> neighbours = get_neighbours(road_map, current);
            foreach (Hashtable neighbour in neighbours)
            {
                int tentative_g = (int)current["g"] + 1;
                if (tentative_g < (int)neighbour["g"])
                {
                    neighbour["parent"] = current;
                    neighbour["g"] = tentative_g;
                    neighbour["f"] = tentative_g + heuristic(neighbour, end);
                    priority_open_set = priotity_replace(priority_open_set, neighbour);
                }

            }

        }

        return false;

    }

    static List<Hashtable> get_neighbours(List<Hashtable> road_map, Hashtable current)
    {

        List<Hashtable> neighbours = new List<Hashtable>();

        Hashtable[] winning = new Hashtable[4];

        Hashtable temp = new Hashtable();
        temp.Add("x", -1);

        for (int i=0; i<4; i++)
        {
            winning[i] = (Hashtable)temp.Clone();  //Might need cast as Clone return object
        }

        foreach (Hashtable cell in road_map)
        {

            if ((bool)current["left"] && (int)current["x"] > (int)cell["x"] && (int)current["y"] == (int)cell["y"])
            {

                if ((int)winning[0]["x"] == -1 || (int)winning[0]["x"] < (int)cell["x"])
                {
                    winning[0] = cell;
                }

            }
            if ((bool)current["right"] && (int)current["x"] < (int)cell["x"] && (int)current["y"] == (int)cell["y"])
            {

                if ((int)winning[1]["x"] == -1 || (int)winning[1]["x"] > (int)cell["x"])
                {
                    winning[1] = cell;
                }

            }
            if ((bool)current["up"] && (int)current["x"] == (int)cell["x"] && (int)current["y"] > (int)cell["y"])
            {
                
                if ((int)winning[2]["x"] == -1 || (int)winning[2]["y"] < (int)cell["y"])
                {
                    winning[2] = cell;
                }

            }
            if ((bool)current["down"] && (int)current["x"] == (int)cell["x"] && (int)current["y"] < (int)cell["y"])
            {

                if ((int)winning[3]["x"] == -1 || (int)winning[3]["y"] > (int)cell["y"])
                {
                    winning[3] = cell;
                }

            }

        }

        bool[] directions = new bool[]{
            (bool)current["left"],
            (bool)current["right"],
            (bool)current["up"],
            (bool)current["down"]
        };

        // Debug.Log("current: x: " + current["x"] + ", y: " + current["y"]);

        for (int i=0; i<4; i++)
        {
            if (directions[i])
            {
                neighbours.Add(winning[i]);
            }
        }

        return neighbours;

    }

    private static List<Hashtable> priotity_replace(List<Hashtable> priority_open_set, Hashtable neighbour)
    {

        // Remove Current Instance of neighboour inside of open_set
        for (int i=0; i<priority_open_set.Count; i++)
        {

            if (   (int)priority_open_set[i]["x"] == (int)neighbour["x"]
                && (int)priority_open_set[i]["y"] == (int)neighbour["y"])
            {

                priority_open_set.RemoveAt(i);
                break;

            }

        }

        if (priority_open_set.Count == 0)
        {
            priority_open_set.Add(
                neighbour
            );
            return priority_open_set;
        }

        //Place neighbour in priority list according to their f score
        bool found = false;
        for (int i=0; i<priority_open_set.Count; i++)
        {

            if ((float)priority_open_set[i]["f"] > (float)neighbour["f"])
            {

                priority_open_set.Insert(i, neighbour);
                found = true;
                break;

            }

        }
        if (!found)
            priority_open_set.Add(neighbour);

        return priority_open_set;

    }

    public static void save_path(Person person, Hashtable cell)
    {

        if (cell["parent"] == null)
        {
            foreach (int[] p in person.temp_path)
            {
                person.append_coord_to_path(p);
            }
            return;
        }

        person.temp_path.Insert(0, new int[]{ (int)((Hashtable)cell["parent"])["x"], (int)((Hashtable)cell["parent"])["y"] });
        save_path(person, (Hashtable)cell["parent"]);

    }

    private static float heuristic(Hashtable start, Hashtable end)
    {
        return (
              ((int)start["x"] - (int)end["x"]) * ((int)start["x"] - (int)end["x"])
            + ((int)start["y"] - (int)end["y"]) * ((int)start["y"] - (int)end["y"])
        );
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

    // Make set of coords useful for navigation using A* algorithm
    // O(n^2)
    public static List<Hashtable> make_road_map(List<CityCell> road_cells)  // IT IS USEFUL, ODD INDEXS; Lol not true
    {

        List<Hashtable> map = new List<Hashtable>();

        foreach (CityCell road in road_cells)
        {

            Hashtable road_location = Pathfinding.road_location(road);

            // Loop thorugh all road twice vs
            road_location = Pathfinding.add_neighbour_directions(
                road_cells,
                road_location
            );

            bool[] neighbour_directions = new bool[4] {
                (bool)road_location["left"],
                (bool)road_location["right"],
                (bool)road_location["up"],
                (bool)road_location["down"]
            };
            
            int neighbours = 0;
            for (int i=0; i<4; i++)
            {
                if (neighbour_directions[i]) { neighbours++; }
            }

            bool opposite_roads = (neighbour_directions[0] && neighbour_directions[1])
                               || (neighbour_directions[2] && neighbour_directions[3]);
            if (!(neighbours == 2 && opposite_roads))
            {

                map.Add(
                    road_location
                );

            }

        }

        return map;

    }

    public static List<Hashtable> copy_road_map(List<Hashtable> road_map)
    {

        List<Hashtable> new_road_map = new List<Hashtable>();
        foreach (Hashtable road in road_map)
        {
            new_road_map.Add(
                (Hashtable)road.Clone()
            );
        }
        return new_road_map;
    }

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


    
}

