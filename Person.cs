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

    public a_star(CityCell endpoint, List<int[]> road_map)
    {

        List<int[]> open_set = new List<int[]>();
        open_set.Add(new int[]{ x, y });

        Hashtable came_from = new Hashtable();

    }


    
}
