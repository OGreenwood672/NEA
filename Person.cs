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

    public int activity_time;

    public bool infected;

    private List<int[]> path;

    public Person(
        CityCell activity,
        CityCell house,
        GameObject parent
    )
    {

        this.activity = activity;
        this.house = house;

        this.activity_time = 0;
        
        infected = false;

        this.path = new List<int[]>();

        game_object = new GameObject();
        game_object.transform.parent = parent.transform;

        renderer = game_object.AddComponent<SpriteRenderer>();

    }

    public int[] get_current_target_coord()
    {

        return this.path[0];

    }

    public bool next_coord()
    {

        this.path.RemoveAt(0);
        return this.path.Count > 0;

    }

    public bool has_path()
    {

        return this.path.Count > 0;
        
    }

    public void a_star(Hashtable start, Hashtable end, List<Hashtable> road_map)
    {

        //Hashtable end = PeopleUtils.road_location(endpoint);

        Debug.Log("start: x: " + start["x"] + ", y: " + start["y"]);
        Debug.Log("end x: " + end["x"] + ", y: " + end["y"]);

        road_map.Add(
            end
        );

        List<Hashtable> priority_open_set = new List<Hashtable>();

        start["g"] = 0;
        start["f"] = heuristic(start, end);

        priority_open_set.Add(
            start  //Talk about PeopleUtils, init used hashtables (no unique for coords)
        );

        while (priority_open_set.Count > 0)
        {

            Hashtable current = priority_open_set[0];
            if ((int)current["x"] == (int)end["x"] && (int)current["y"] == (int)end["y"])
            {
                this.path.Insert(0, new int[]{ (int)current["x"], (int)current["y"] });
                save_path(current);
                return;
            }

            priority_open_set.RemoveAt(0);
            List<Hashtable> neighbours = get_neighbours(road_map, current);
            // Debug.Log(neighbours.Count);
            foreach (Hashtable neighbour in neighbours)
            {
                int tentative_g = (int)current["g"] + 1;
                if (tentative_g < (int)neighbour["g"])
                {
                    neighbour["parent"] = current;
                    neighbour["g"] = tentative_g;
                    neighbour["f"] = tentative_g + heuristic(neighbour, end);
                    priority_open_set = priotity_replace(priority_open_set, neighbour);
                    // Debug.Log(priority_open_set.Count);
                }

            }

        }
        Debug.Log("Not found");

    }

    private List<Hashtable> get_neighbours(List<Hashtable> road_map, Hashtable current)
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

        Debug.Log("current: x: " + current["x"] + ", y: " + current["y"]);

        for (int i=0; i<4; i++)
        {
            if (directions[i])
            {
                neighbours.Add(winning[i]);
                Debug.Log(i + ": x: " + winning[i]["x"] + ", y: " + winning[i]["y"]);
            }
        }

        return neighbours;

    }

    private List<Hashtable> priotity_replace(List<Hashtable> priority_open_set, Hashtable neighbour)
    {

        // Remove Current Instance of neighboour inside of open_set
        for (int i=0; i<priority_open_set.Count; i++)
        {

            if (   priority_open_set[i]["x"] == neighbour["x"]
                && priority_open_set[i]["y"] == neighbour["y"])
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
        for (int i=0; i<priority_open_set.Count; i++)
        {

            if ((float)priority_open_set[i]["f"] > (float)neighbour["f"])
            {

                priority_open_set.Insert(i, neighbour);
                break;

            }

        }

        return priority_open_set;

    }

    private void save_path(Hashtable cell)
    {

        if (cell["parent"] == null)
        {
            return;
        }

        this.path.Insert(0, new int[]{ (int)((Hashtable)cell["parent"])["x"], (int)((Hashtable)cell["parent"])["y"] });
        save_path((Hashtable)cell["parent"]);

    }

    private float heuristic(Hashtable start, Hashtable end)
    {
        return (
              ((int)start["x"] - (int)end["x"]) * ((int)start["x"] - (int)end["x"])
            + ((int)start["y"] - (int)end["y"]) * ((int)start["y"] - (int)end["y"])
        );
    }


    
}

//CASTING ERRORS, DUE TO HASHTABLES ONLY HOLDING GENERIC OBJECT CLASS