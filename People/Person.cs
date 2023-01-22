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
    public List<int[]> temp_path;

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

    public void append_cell_to_path(CityCell cell)
    {
        this.path.Add(
            new int[] { cell.x, cell.y }
        );
    }

    public void append_coord_to_path(int[] cell)
    {
        this.path.Add(
            new int[] { cell[0], cell[1] }
        );
    }

    public bool check_if_dead(System.Random rnd, float death_chance)
    {
        
        if (this.infected_time == 0)
        {
            int probability = rnd.Next(101);
            if ((float)( probability / 100f ) > death_chance)
            {
                this.is_dead = true;
                return true;
            }
            else
            {
                this.infected = false;
                this.infected_time = -1;
            }
        }
        else if (this.infected_time > 0)
        {
            this.infected_time -= 1;
        }
        return false;

    }

}

