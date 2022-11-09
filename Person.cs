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
            List<Hashtable> neighbours = get_neighbours(current);
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

    private float heuristic(Hashtable start, Hashtable end)
    {
        return (
              (start["x"] - end["x"]) * (start["x"] - end["x"])
            + (start["y"] - end["y"]) * (start["y"] - end["y"])
        );
    }


    
}
