using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleHandler : MonoBehaviour
{

    public WorldManager world_manager;
    public CityHandler city_handler;

    private int width;
    private int height;

    private int seed;

    public GameObject parent;

    public Sprite infected_sprite;
    public Sprite uninfected_sprite;

    private List<Person> people;

    private List<CityCell> work_cells;
    private List<CityCell> school_cells;
    private List<CityCell> social_cells;
    private List<CityCell> house_cells;

    private List<CityCell> road_cells;

    // Start is called before the first frame update
    void Start()
    {

        seed = world_manager.seed;
        width = world_manager.width;
        height = world_manager.height;

        people = new List<Person>();

        work_cells = city_handler.get_workplaces();
        school_cells = city_handler.get_schools();
        social_cells = city_handler.get_socialplaces();
        house_cells = city_handler.get_houses();
        road_cells = city_handler.get_roads();

        int no_of_activities = work_cells.Count + school_cells.Count;

        System.Random rnd = new System.Random(seed);

        for (int i=0; i<world_manager.population_size; i++)
        {

            CityCell activity;
            int activity_index = rnd.Next(no_of_activities);
            
            if (activity_index < work_cells.Count)
            {
                activity = work_cells[activity_index];
            }
            else
            {
                activity = work_cells[activity_index - work_cells.Count];
            }

            int home_index = rnd.Next(house_cells.Count);
            CityCell home = house_cells[home_index];

            people.Add(
                new Person(
                    activity,
                    home,
                    parent
                )
            );
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Person person in people)
        {

            person.game_object.transform.position = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(  // Struggled with coords
                                                new Vector3(
                                                    Screen.width * (person.house.closest_road.x + 0.5f) / width,
                                                    Screen.height - Screen.height * (person.house.closest_road.y + 0.5f) / height,
                                                    1.0f)
                                                );

            person.game_object.transform.localScale = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(    // * 0.25f works
                                                new Vector3(
                                                    ((float)Screen.width / (float)width) * 0.25f + Screen.width/2,
                                                    ((float)Screen.height / (float)height) * 0.27f + Screen.height/2,
                                                    1.0f)
                                                );


            if (person.infected) 
            {
                person.renderer.sprite = infected_sprite;
            }
            else
            {
                person.renderer.sprite = uninfected_sprite;
            }

        }
    }


    // Make set of coords useful for navigation using A* algorithm
    // O(n^2)
    List<Hashtable> make_road_map()  // IT IS USEFUL, ODD INDEXS
    {

        List<Hashtable> map = new List<Hashtable>();

        foreach (CityCell a_road in road_cells)
        {

            Hashtable road_location = PeopleUtils.road_location(a_road);

            int neighbours = 0;

            // Loop thorugh all road twice vs
            // Find coords of each neighbour and check if road
            foreach (CityCell b_road in road_cells)
            {

                bool left = a_road.x == b_road.x-1 && a_road.y == b_road.y;
                bool right = a_road.x == b_road.x+1 && a_road.y == b_road.y;
                bool up = a_road.x == b_road.x && a_road.y == b_road.y+1;
                bool down = a_road.x == b_road.x && a_road.y == b_road.y-1;

                if (left)
                {
                    road_location["left"] = true;
                    neighbours++;
                }
                else if (right)
                {
                    road_location["right"] = true;
                    neighbours++;
                }
                else if (up)
                {
                    road_location["up"] = true;
                    neighbours++;
                }
                else if (down)
                {
                    road_location["down"] = true;
                    neighbours++;
                }

            }

            if (neighbours != 2)
            {

                map.Add(
                    road_location
                );

            }

        }

        return map;

    }

}
