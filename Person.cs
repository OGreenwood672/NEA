using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person
{

    public CityCell activity;
    public CityCell house;

    public GameObject game_object;
    public SpriteRenderer renderer;

    public int x;
    public int y;

    public bool infected;

    public Person(
        CityCell activity,
        CityCell house,
        GameObject parent
    )
    {

        this.activity = activity;
        this.house = house;
        
        infected = false;

        game_object = new GameObject();
        game_object.transform.parent = parent.transform;

        renderer = game_object.AddComponent<SpriteRenderer>();

    }

    public a_star(CityCell start, CityCell endpoint, List<Hashtable> road_map)
    {

        Hashtable end = PeopleUtils.RoadLocation(endpoint);

        road_map.Add(
            end
        );

        List<Hashtable> priority_open_set = new List<HashSet>();
        priority_open_set.Add(
            PeopleUtils.RoadLocation(start)  //Talk about PeopleUtils, init used hashtables (no unique for coords)
        );

        priority_open_set[0]["f"] = heuristic(priority_open_set[0], end);

        while (priority_open_set.Count > 0)
        {

            Hashtable current = priority_open_set[0];
            if (current["x"] == end["x"] && current["y"] == end["y"])
            {
                //Finish
            }

            priority_open_set.RemoveAt(0);
            List<Hashtable> neighbours = get_neighbours(road_map, current);
            foreach (Hashtable neighbour in neighbours)
            {

                int tentative_g = current["g"] + 1;
                if (tentative_g < neighbour["g"])
                {
                    neighbour["parent"] = current;
                    neighbour["g"] = tentative_g;
                    neighbour["f"] = tentative_g + heuristic(neighbour, end);
                    priority_open_set = priotity_replace(priority_open_set, neighbour);
                }

            }

        }

    }

    private List<Hashtable> get_neighbours(List<Hashtable> road_map, Hashtable current)
    {

        List<Hashtable> neighbours = new List<Hashtable>();

        Hashtable[] winning = new Hashtable[4];

        Hashtable temp = new Hashtable();
        temp.Add("x", -1);

        for (int i=0; i<4; i++)
        {
            winning[i] = temp.Clone();
        }

        foreach (Hashtable cell in road_map)
        {

            if (current["left"] && current["x"] > cell["x"] && current["y"] == cell["y"])
            {

                if (winning["x"] == -1 || winning["x"] < cell["x"])
                {
                    winning[0] = cell;
                }

            }
            if (current["right"] && current["x"] < cell["x"] && current["y"] == cell["y"])
            {

                if (winning["x"] == -1 || winning["x"] > cell["x"])
                {
                    winning[1] = cell;
                }

            }
            if (current["up"] && current["x"] == cell["x"] && current["y"] < cell["y"])
            {
                
                if (winning["x"] == -1 || winning["y"] > cell["y"])
                {
                    winning[2] = cell;
                }

            }
            if (current["down"] && current["x"] == cell["x"] && current["y"] > cell["y"])
            {

                if (winning["x"] == -1 || winning["y"] < cell["y"])
                {
                    winning[3] = cell;
                }

            }

        }

        bool[] directions = new bool[]{
            current["left"],
            current["right"],
            current["up"],
            current["down"]
        };

        for (int i=0; i<4; i++)
        {
            if (directions[i])
            {
                neighbours.Add(winning[i]);
            }
        }

        return neighbours;

    }

    private List<Hashtable> priotity_replace(List<Hashtable> priority_open_set, Hashtable neighbour)
    {

        // Remove Current Instance of neighboour inside of open_set
        for (int i=0; i<priority_open_set.Count; i++)
        {

            if (   priorty_open_set[i]["x"] == neighbour["x"]
                && priorty_open_set[i]["y"] == neighbour["y"])
            {

                priorty_open_set.RemoveAt(i);
                break;

            }

        }

        //Place neighbour in priority list according to their f score
        for (int i=0; i<priority_open_set.Count; i++)
        {

            if (priorty_open_set[i]["f"] > neighbour["f"])
            {

                priorty_open_set.Insert(i, neighbour);
                break;

            }

        }

        return priorty_open_set;

    }

    private float heuristic(Hashtable start, Hashtable end)
    {
        return (
              (start["x"] - end["x"]) * (start["x"] - end["x"])
            + (start["y"] - end["y"]) * (start["y"] - end["y"])
        );
    }


    
}
