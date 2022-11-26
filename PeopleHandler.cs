using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleHandler : MonoBehaviour
{

    public WorldManager world_manager;
    public CityHandler city_handler;

    private System.Random rnd;

    private float speed;

    private int game_ticks;

    private int ticks_in_day;

    private int start_work_time;
    private int start_work_time_range;
    private int start_school_time;
    private int start_school_time_range;

    private int end_work_time;
    private int end_work_time_range;
    private int end_school_time;
    private int end_school_time_range;

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

    private List<Hashtable> road_map;

    // Start is called before the first frame update
    void Start()
    {

        seed = world_manager.seed;
        width = world_manager.width;
        height = world_manager.height;

        speed = world_manager.speed;

        ticks_in_day = world_manager.ticks_in_day;

        start_work_time = world_manager.start_work_time;
        start_work_time_range = world_manager.start_work_time_range;
        start_school_time = world_manager.start_school_time;
        start_school_time_range = world_manager.start_school_time_range;

        end_work_time = world_manager.end_work_time;
        end_work_time_range = world_manager.end_work_time_range;
        end_school_time = world_manager.end_school_time;
        end_school_time_range = world_manager.end_school_time_range;


        people = new List<Person>();

        work_cells = city_handler.get_workplaces();
        school_cells = city_handler.get_schools();
        social_cells = city_handler.get_socialplaces();
        house_cells = city_handler.get_houses();
        road_cells = city_handler.get_roads();

        road_map = make_road_map();

        int no_of_activities = work_cells.Count + school_cells.Count;

        rnd = new System.Random(seed);

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
                activity = school_cells[activity_index - work_cells.Count];
            }

            int home_index = rnd.Next(house_cells.Count);
            CityCell home = house_cells[home_index];

            Person person = new Person(
                                activity,
                                home,
                                parent
                            );

            person.renderer.sprite = uninfected_sprite;

            people.Add(
                person
            );
        }
        
    }

    // 50 fps
    void FixedUpdate()
    {

        game_ticks++;
        if (game_ticks > ticks_in_day)
        {
            game_ticks -= ticks_in_day;
        }

        foreach (Person person in people)
        {

            // Home -> work
            if (game_ticks == start_work_time && person.activity.district == "work")
            {
                person.activity_time = rnd.Next(start_work_time_range);
            }
            if (game_ticks == start_school_time && person.activity.district == "school")
            {
                person.activity_time = rnd.Next(start_school_time_range);
            }
            if ((game_ticks == start_work_time + person.activity_time && person.activity.district == "work")
             || (game_ticks == start_school_time + person.activity_time && person.activity.district == "school"))
            {

                Hashtable house = PeopleUtils.add_neighbour_directions(
                    road_cells,
                    PeopleUtils.road_location(
                        person.house.closest_road
                    )
                );
                Hashtable activity = PeopleUtils.road_location(
                    person.activity.closest_road
                );

                person.a_star(
                    house,
                    activity,
                    copy_road_map(road_map)
                );

                person.x = person.house.closest_road.x;
                person.y = person.house.closest_road.y;

            }

            //Probably messed up the activity and home switch around

            // Work -> Home
            if (game_ticks == end_work_time && person.activity.district == "work")
            {
                person.activity_time = rnd.Next(end_work_time_range);
            }
            if (game_ticks == end_school_time && person.activity.district == "school")
            {
                person.activity_time = rnd.Next(end_school_time_range);
            }
            if (game_ticks == end_work_time + person.activity_time && person.activity.district == "work"
             || game_ticks == end_school_time + person.activity_time && person.activity.district == "school")
            {

                Hashtable activity = PeopleUtils.add_neighbour_directions(
                    road_cells,
                    PeopleUtils.road_location(
                        person.activity.closest_road
                    )
                );
                Hashtable house = PeopleUtils.road_location(
                    person.house.closest_road
                );

                person.a_star(
                    activity,
                    house,
                    copy_road_map(road_map)
                );

                person.x = person.activity.closest_road.x;
                person.y = person.activity.closest_road.y;
            }




            if (person.has_path())
            {
                int[] target = person.get_current_target_coord();
                if (person.x < target[0])
                {
                    person.x = Mathf.Min(person.x + speed, target[0]);
                }
                else if (person.x > target[0])
                {
                    person.x = Mathf.Max(person.x - speed, target[0]);
                }
                else if (person.y < target[1])
                {
                    person.y = Mathf.Min(person.y + speed, target[1]);
                }
                else if (person.y > target[1])
                {
                    person.y = Mathf.Max(person.y - speed, target[1]);
                }
                else
                {
                    person.next_coord();
                }

            }


            // Could add offset in here
            person.game_object.transform.position = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(  // Struggled with coords
                                                new Vector3(
                                                    (float)Screen.width * (person.x + 0.5f) / width,
                                                    (float)Screen.height - (float)Screen.height * (person.y + 0.5f) / height,
                                                    0.95f)
                                                );

            person.game_object.transform.localScale = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(
                                              new Vector3(
                                                  ((float)Screen.width / (float)width) * 0.25f + Screen.width/2,
                                                  ((float)Screen.height / (float)height) * 0.27f + Screen.height/2,
                                                  1.0f)
                                              );
            
        }
    }


    // Make set of coords useful for navigation using A* algorithm
    // O(n^2)
    List<Hashtable> make_road_map()  // IT IS USEFUL, ODD INDEXS; Lol not true
    {

        List<Hashtable> map = new List<Hashtable>();

        foreach (CityCell road in road_cells)
        {

            Hashtable road_location = PeopleUtils.road_location(road);

            // Loop thorugh all road twice vs
            road_location = PeopleUtils.add_neighbour_directions(
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

            if (neighbours != 2)
            {

                map.Add(
                    road_location
                );

            }

        }

        return map;

    }

    List<Hashtable> copy_road_map(List<Hashtable> road_map)
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

}
