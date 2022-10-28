using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour  // Needed as added to World Manager Object
{
    public int seed;

    public int population_size;

    public int width;
    public int height;


    //City Generation Variables
    public int num_of_districts;
    public float district_competition;
    public float vertical_road_threshhold;
    public float horizontal_road_threshhold;
    public int blocksize;


    public int num_of_works;
    public int num_of_schools;
    public int num_of_socials;
    public int num_of_houses;

}
