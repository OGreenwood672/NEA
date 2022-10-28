using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleHandler : MonoBehaviour
{

    public WorldManager world_manager;
    public CityHandler city_handler;

    private int seed;

    private List<Person> people;

    private List<CityCell> work_cells;
    private List<CityCell> school_cells;
    private List<CityCell> social_cells;
    private List<CityCell> house_cells;

    // Start is called before the first frame update
    void Start()
    {

        seed = world_manager.seed;

        work_cells = city_handler.get_workplaces();
        school_cells = city_handler.get_schools();
        social_cells = city_handler.get_socialplaces();
        house_cells = city_handler.get_houses();

        

        for (int i=0; i<world_manager.population_size; i++)
        {
            
            // CityCell activity = 
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
