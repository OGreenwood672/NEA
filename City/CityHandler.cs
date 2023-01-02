using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHandler : MonoBehaviour
{

    //city variables
    private int width;
    private int height;

    public GameObject city_parent;


    public Sprite work_sprite;
    public Sprite school_sprite;
    public Sprite social_sprite;
    public Sprite house_sprite;
    public Sprite road_sprite;


    //generate empty city
    private CityCell[,] city;

    private List<CityCell> work_cells;
    private List<CityCell> school_cells;
    private List<CityCell> social_cells;
    private List<CityCell> house_cells;
    private List<CityCell> road_cells;

    public WorldManager world_manager;
    public PeopleHandler people_handler;


    // Start is called before the first frame update
    void Start()
    {

        width = world_manager.width;
        height = world_manager.height;

        work_cells = new List<CityCell>();
        school_cells = new List<CityCell>();
        social_cells = new List<CityCell>();
        house_cells = new List<CityCell>();
        road_cells = new List<CityCell>();

        CityGeneration city_gen = new CityGeneration(
            world_manager
        );

        city = city_gen.generate_city();

        classify_cells_by_district(city);

        render();

        //Enables people script
        people_handler.enabled = true;

    }


    void render()
    {
        
        for (int y=0; y<height; y++)
        {
            for (int x=0; x<width; x++)
            {

                GameObject cell = new GameObject();

                cell.transform.parent = city_parent.transform;

                cell.transform.position = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(  // Struggled with coords
                                                    new Vector3(
                                                        Screen.width * (x + 0.5f) / width,
                                                        Screen.height - Screen.height * (y + 0.5f) / height,
                                                        1.0f)
                                                    );

                cell.transform.localScale = Camera.main.GetComponent<Camera>().ScreenToWorldPoint(    // * 0.25f works
                                                    new Vector3(
                                                        ((float)Screen.width / (float)width) * 0.25f + Screen.width/2,
                                                        ((float)Screen.height / (float)height) * 0.27f + Screen.height/2,
                                                        1.0f)
                                                    );

                SpriteRenderer renderer = cell.AddComponent<SpriteRenderer>();
                
                if (city[y, x].road) {
                    renderer.sprite = road_sprite;
                } else {
                    switch (city[y, x].district)
                    {
                        case "work":
                            renderer.sprite = work_sprite;
                            break;
                        case "school":
                            renderer.sprite = school_sprite;
                            break;
                        case "social":
                            renderer.sprite = social_sprite;
                            break;
                        case "house":
                            renderer.sprite = house_sprite;
                            break;
                        default:
                            break;
                    }
                }

            }
        }
    }

    void classify_cells_by_district(CityCell[,] city)
    {
        
        // Had to initializze the lists

        for (int y=0; y<height; y++)
        {
            for (int x=0; x<width; x++)
            {
                
                if (city[y, x].road)
                {
                    road_cells.Add(city[y, x]);
                } else 
                {

                    switch (city[y, x].district)
                    {
                        case "work":
                            work_cells.Add(city[y, x]);
                            break;
                        
                        case "school":
                            school_cells.Add(city[y, x]);
                            break;
                        
                        case "social":
                            social_cells.Add(city[y, x]);
                            break;

                        case "house":
                            house_cells.Add(city[y, x]);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
    

    public List<CityCell> get_workplaces()
    {
        return work_cells;
    }

    public List<CityCell> get_schools()
    {
        return school_cells;
    }

    public List<CityCell> get_socialplaces()
    {
        return social_cells;
    }
    public List<CityCell> get_houses()
    {
        return house_cells;
    }

    public List<CityCell> get_roads()
    {
        return road_cells;
    }

    public CityCell[,] get_city()
    {
        return city;
    }
}