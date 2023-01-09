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
    private int start_social_time;
    private int start_social_time_range;


    private int end_work_time;
    private int end_work_time_range;
    private int end_school_time;
    private int end_school_time_range;
    private int end_social_time;
    private int end_social_time_range;

    private int width;
    private int height;

    private int seed;

    public GameObject parent;

    public Sprite infected_sprite;
    public Sprite uninfected_sprite;

    private List<Person> people = new List<Person>();

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
        start_social_time = world_manager.start_social_time;
        start_social_time_range = world_manager.start_social_time_range;

        end_work_time = world_manager.end_work_time;
        end_work_time_range = world_manager.end_work_time_range;
        end_school_time = world_manager.end_school_time;
        end_school_time_range = world_manager.end_school_time_range;
        end_social_time = world_manager.end_social_time;
        end_social_time_range = world_manager.end_social_time_range;

        work_cells = city_handler.get_workplaces();
        school_cells = city_handler.get_schools();
        social_cells = city_handler.get_socialplaces();
        house_cells = city_handler.get_houses();
        road_cells = city_handler.get_roads();

        road_map = make_road_map();

        int no_of_activities = work_cells.Count
                             + school_cells.Count
                             + social_cells.Count;

        rnd = new System.Random(seed);

        for (int i=0; i<world_manager.population_size; i++)
        {

            CityCell activity;
            int activity_index = rnd.Next(no_of_activities);
            
            if (activity_index < work_cells.Count)
            {
                activity = work_cells[activity_index];
            }
            else if (activity_index < work_cells.Count + school_cells.Count)
            {
                activity = school_cells[activity_index - work_cells.Count];
            }
            else
            {
                activity = social_cells[
                    activity_index
                  - work_cells.Count
                  - school_cells.Count
                ];
            }

            int home_index = rnd.Next(house_cells.Count);
            CityCell home = house_cells[home_index];

            float offset_range = 0.4f;

            Person person = new Person(
                                activity,
                                home,
                                parent,
                                (float)rnd.Next((int)(offset_range * 100)) / 100f - (offset_range/2.0f),
                                (float)rnd.Next((int)(offset_range * 100)) / 100f - (offset_range/2.0f)
                            );

            person.renderer.sprite = uninfected_sprite;
            person.renderer.enabled = false;

            // if (rnd.Next(101) / 100f < world_manager.initial_infected_probabilty)
            // {
            //     person.infected = true;
            //     person.renderer.sprite = infected_sprite;
            // }

            people.Add(
                person
            );
        }

        int no_of_infected = Mathf.CeilToInt(
            world_manager.population_size * world_manager.initial_infected_probabilty
        );
        for (int i=0; i<no_of_infected; i++)
        {
            people[i].infected = true;
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

            if (!world_manager.lockdown)
            {
                person.in_lockdown = false;

                add_activity_time(person);

                add_path(person);

            }
            else
            {
                if (!person.in_lockdown)
                {
                    person.in_lockdown = true;
                    person.clear_path();

                    CityCell current_cell = city_handler.get_cell_by_coords(Mathf.FloorToInt(person.x), Mathf.FloorToInt(person.y));
                    if (!current_cell.road && !(current_cell.x == person.house.x && current_cell.y == person.house.y))
                    {
                        current_cell = current_cell.closest_road;
                    }
                    find_path(
                        person,
                        current_cell,
                        person.house
                    );
                }
            }

            move_people(person);

            if (person.infected && game_ticks % world_manager.rate_of_infection_in_ticks == 0)
            {
                VirusHandler.infect_people(world_manager, people, person);
            }
            if (person.infected)
            {
                person.renderer.sprite = infected_sprite;
            }
            else
            {
                person.renderer.sprite = uninfected_sprite;
            }


            if (person.has_path())
            {
                person.renderer.enabled = true;
                person.game_object.transform.position = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(  // Struggled with coords
                                                    new Vector3(
                                                        (float)Screen.width * (person.x + 0.5f + person.x_off) / width,
                                                        (float)Screen.height - (float)Screen.height * (person.y + 0.5f + person.y_off) / height,
                                                        0.95f)
                                                    );

                person.game_object.transform.localScale = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(
                                                new Vector3(
                                                    ((float)Screen.width / (float)width) * 0.1f + Screen.width/2,
                                                    ((float)Screen.height / (float)height) * 0.1f + Screen.height/2,
                                                    1.0f)
                                                );
            }
            else
            {
                person.renderer.enabled = false;
            }
            
        }
    }

    void add_activity_time(Person person)
    {

        if (person.activity.district == "work")
        {
            if (game_ticks == start_work_time)
            {
                person.activity_time = rnd.Next(start_work_time_range);
            }
            else if (game_ticks == end_work_time)
            {
                person.activity_time = rnd.Next(end_work_time_range);
            }
        }

        if (person.activity.district == "school")
        {
            if (game_ticks == start_school_time)
            {
                person.activity_time = rnd.Next(start_school_time_range);
            }
            else if (game_ticks == end_school_time)
            {
                person.activity_time = rnd.Next(end_school_time_range);
            }
        }

        if (person.activity.district == "social")
        {
            if (game_ticks == start_social_time)
            {
                person.activity_time = rnd.Next(start_social_time_range);
            }
            else if (game_ticks == end_social_time)
            {
                person.activity_time = rnd.Next(end_social_time_range);
            }
        }
    }

    void add_path(Person person) 
    {

        bool work_time_check = game_ticks == person.activity_time + start_work_time && person.activity.district == "work";
        bool school_time_check = game_ticks == person.activity_time + start_school_time && person.activity.district == "school";
        bool social_time_check = game_ticks == person.activity_time + start_social_time && person.activity.district == "social";

        bool end_work_time_check = game_ticks == person.activity_time + end_work_time && person.activity.district == "work";
        bool end_school_time_check = game_ticks == person.activity_time + end_school_time && person.activity.district == "school";
        bool end_social_time_check = game_ticks == person.activity_time + end_social_time && person.activity.district == "social";


        if (work_time_check || school_time_check || social_time_check)
        {
            find_path(
                person,
                person.house.closest_road,
                person.activity
            );
            person.x = person.house.x;
            person.y = person.house.y;
        }

        CityCell current_cell = city_handler.get_cell_by_coords(Mathf.FloorToInt(person.x), Mathf.FloorToInt(person.y));
        if (
            (end_work_time_check || end_school_time_check || end_social_time_check) 
            && !(current_cell.x == person.house.x && current_cell.y == person.house.y))
        {
            find_path(
                person,
                person.activity.closest_road,
                person.house
            );
            person.x = person.activity.x;
            person.y = person.activity.y;
        }
    }

    void find_path(Person person, CityCell start_road, CityCell end)
    {

        if (start_road.x == end.x && start_road.y == end.y) { return; }

        Hashtable start_hashtable = PeopleUtils.add_neighbour_directions(
            road_cells,
            PeopleUtils.road_location(
                start_road
            )
        );
        Hashtable end_hashtable = PeopleUtils.road_location(
            end.closest_road
        );

        bool found = person.a_star(
            start_hashtable,
            end_hashtable,
            copy_road_map(road_map)
        );

        if (found)
        {
            person.append_to_path(end);
        }

    }

    void move_people(Person person)
    {

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

            bool opposite_roads = (neighbour_directions[0] && neighbour_directions[1])
                               || (neighbour_directions[2] && neighbour_directions[3]);
            if (!(neighbours == 2 && opposite_roads))
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

    public int get_game_ticks()
    {
        return game_ticks;
    }

    public int get_infected_count()
    {
        int count = 0;
        foreach (Person person in people)
        {
            if (person.infected)
                count++;
        }
        return count;
    }

    public int get_population_count()
    {
        return world_manager.population_size;
    }

}
