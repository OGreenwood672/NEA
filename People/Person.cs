using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person
{

    public CityCell activity;
    public CityCell house;

    public GameObject game_object;
    public SpriteRenderer renderer;

    public float x;
    public float y;

    public float x_off;
    public float y_off;

    public int activity_time;

    public float immunity;
    
    public int infected_time;

    public bool infected;
    public bool is_dead;

    public bool in_lockdown;

    private List<int[]> path;
    private List<int[]> temp_path;

    public Person(
        CityCell activity,
        CityCell house,
        GameObject parent,
        float x_off,
        float y_off,
        float immunity
    )
    {

        this.activity = activity;
        this.house = house;

        this.activity_time = 0;
        this.infected_time = -1;

        this.immunity = immunity;

        this.is_dead = false;
        this.infected = false;

        this.path = new List<int[]>();
        this.temp_path = new List<int[]>();

        x = house.x;
        y = house.y;

        this.x_off = x_off;
        this.y_off = y_off;

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

    public void clear_path()
    {

        this.path = new List<int[]>();
        
    }

    public void append_to_path(CityCell cell)
    {
        this.path.Add(
            new int[] { cell.x, cell.y }
        );
    }

    public bool a_star(Hashtable start, Hashtable end, List<Hashtable> road_map)
    {

        //Hashtable end = PeopleUtils.road_location(endpoint);

        // Debug.Log("start: x: " + start["x"] + ", y: " + start["y"]);
        // Debug.Log("end x: " + end["x"] + ", y: " + end["y"]);

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
                this.temp_path = new List<int[]>();
                this.temp_path.Insert(0, new int[]{ (int)current["x"], (int)current["y"] });
                save_path(current);
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
        // Debug.Log("start: x: " + start["x"] + ", y: " + start["y"]);
        // Debug.Log("end x: " + end["x"] + ", y: " + end["y"]);
        // Debug.Log("Not found");

        return false;

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

    private List<Hashtable> priotity_replace(List<Hashtable> priority_open_set, Hashtable neighbour)
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

    private void save_path(Hashtable cell)
    {

        if (cell["parent"] == null)
        {
            foreach (int[] p in this.temp_path)
            {
                this.path.Add(p);
            }
            return;
        }

        this.temp_path.Insert(0, new int[]{ (int)((Hashtable)cell["parent"])["x"], (int)((Hashtable)cell["parent"])["y"] });
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
