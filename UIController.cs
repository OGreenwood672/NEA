using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject home_screen;
    public GameObject settings_screen;

    public GameObject info_box;
    public GameObject control_box;

    public CityHandler city_handler;
    public PeopleHandler people_handler;
    public WorldManager world_manager;

    public void on_start_simulation()
    {
        home_screen.SetActive(false);
        info_box.SetActive(true);
        control_box.SetActive(true);
        city_handler.enabled = true;
    }

    public void on_settings_click()
    {
        home_screen.SetActive(false);
        settings_screen.SetActive(true);
    }

    public void on_to_home_click()
    {
        home_screen.SetActive(true);
        settings_screen.SetActive(false);
    }

    public void quit()
    {
        city_handler.enabled = false;
        people_handler.enabled = false;
        home_screen.SetActive(true);
        info_box.SetActive(false);
        control_box.SetActive(false);
        world_manager.pause = false;
    }

    public Text pause_text;
    public void on_pause()
    {
        world_manager.pause = !world_manager.pause;
        if (world_manager.pause)
            pause_text.text = "PLAY";
        else
            pause_text.text = "PAUSE";
    }

    public Text pop_text;
    public Slider pop_slider;
    public void edit_population()
    {
        world_manager.population_size = Mathf.FloorToInt(pop_slider.value);
        pop_text.text = "(" + pop_slider.value + ")";
    }

    public Text width_text;
    public Slider width_slider;
    public void edit_width()
    {
        world_manager.width = Mathf.FloorToInt(width_slider.value);
        width_text.text = "(" + width_slider.value + ")";
    }

    public Text height_text;
    public Slider height_slider;
    public void edit_height()
    {
        world_manager.height = Mathf.FloorToInt(height_slider.value);
        height_text.text = "(" + height_slider.value + ")";
    }

    public Text seed_text;
    public Slider seed_slider;
    public void edit_seed()
    {
        world_manager.seed = Mathf.FloorToInt(seed_slider.value);
        seed_text.text = "(" + seed_slider.value + ")";
    }

    public Text district_comp_text;
    public Slider district_comp_slider;
    public void edit_district_competition()
    {
        world_manager.district_competition = district_comp_slider.value;
        district_comp_text.text = "(" + district_comp_slider.value + ")";
    }
    

    public Text district_count_text;
    public Slider district_count_slider;
    public void edit_num_of_districts()
    {
        world_manager.num_of_districts = Mathf.FloorToInt(district_count_slider.value);
        district_count_text.text = "(" + district_count_slider.value + ")";
    }

    public Text vertical_road_text;
    public Slider vertical_road_slider;
    public void edit_vertical_road_threshhold()
    {
        world_manager.vertical_road_threshhold = vertical_road_slider.value;
        vertical_road_text.text = "(" + vertical_road_slider.value + ")";
    }

    public Text horizontal_road_text;
    public Slider horizontal_road_slider;
    public void edit_horizontal_road_threshhold()
    {
        world_manager.horizontal_road_threshhold = horizontal_road_slider.value;
        horizontal_road_text.text = "(" + horizontal_road_slider.value + ")";
    }

    public Text blocksize_text;
    public Slider blocksize_slider;
    public void edit_blocksize()
    {
        world_manager.blocksize = Mathf.FloorToInt(blocksize_slider.value);
        blocksize_text.text = "(" + blocksize_slider.value + ")";
    }

    public Text work_ratio_text;
    public Slider work_ratio_slider;
    public void edit_num_of_works()
    {
        world_manager.num_of_works = Mathf.FloorToInt(work_ratio_slider.value);
        work_ratio_text.text = "(" + work_ratio_slider.value + ")";
    }

    public Text school_ratio_text;
    public Slider school_ratio_slider;
    public void edit_num_of_schools()
    {
        world_manager.num_of_schools = Mathf.FloorToInt(school_ratio_slider.value);
        school_ratio_text.text = "(" + school_ratio_slider.value + ")";
    }

    public Text social_ratio_text;
    public Slider social_ratio_slider;
    public void edit_num_of_socials()
    {
        world_manager.num_of_socials = Mathf.FloorToInt(social_ratio_slider.value);
        social_ratio_text.text = "(" + social_ratio_slider.value + ")";
    }

    public Text house_ratio_text;
    public Slider house_ratio_slider;
    public void edit_num_of_houses()
    {
        world_manager.num_of_houses = Mathf.FloorToInt(house_ratio_slider.value);
        house_ratio_text.text = "(" + house_ratio_slider.value + ")";
    }

    public Text speed_text;
    public Slider speed_slider;
    public void edit_speed()
    {
        world_manager.speed = speed_slider.value;
        speed_text.text = "(" + speed_slider.value + ")";
    }

    public Text ticks_in_day_text;
    public Slider ticks_in_day_slider;
    public void edit_ticks_in_day()
    {
        world_manager.ticks_in_day = Mathf.FloorToInt(ticks_in_day_slider.value);
        ticks_in_day_text.text = "(" + ticks_in_day_slider.value + ")";
    }

    public Text infection_chance_text;
    public Slider infection_chance_slider;
    public void edit_chance_of_infection()
    {
        world_manager.chance_of_infection = infection_chance_slider.value;
        infection_chance_text.text = "(" + infection_chance_slider.value + ")";
    }

    public Text rate_of_infection_text;
    public Slider rate_of_infection_slider;
    public void edit_rate_of_infection()
    {
        world_manager.rate_of_infection_in_ticks = Mathf.FloorToInt(rate_of_infection_slider.value);
        rate_of_infection_text.text = "(" + rate_of_infection_slider.value + ")";
    }

    public Text virus_range_text;
    public Slider virus_range_slider;
    public void edit_virus_range_radius()
    {
        world_manager.virus_range_radius = Mathf.FloorToInt(virus_range_slider.value);
        virus_range_text.text = "(" + virus_range_slider.value + ")";
    }

    public Text initial_infected_text;
    public Slider initial_infected_slider;
    public void edit_initial_infected_probabilty()
    {
        world_manager.initial_infected_probabilty = initial_infected_slider.value;
        initial_infected_text.text = "(" + initial_infected_slider.value + ")";
    }

    public Text immunity_text;
    public Slider immunity_slider;
    public void edit_immunity()
    {
        world_manager.immunity_range = Mathf.FloorToInt(immunity_slider.value);
        immunity_text.text = "(" + immunity_slider.value + ")";
    }

    public Text death_chance_text;
    public Slider death_chance_slider;
    public void edit_death_chance()
    {
        world_manager.death_chance = death_chance_slider.value;
        death_chance_text.text = "(" + death_chance_slider.value + ")";
    }


    public Image social_distancing_checkbox;
    private bool social_distancing = false;
    public void update_social_distancing()
    {
        social_distancing = !social_distancing;
        if (social_distancing)
        {
            social_distancing_checkbox.color = new Color32(0, 255, 0, 255);
            world_manager.virus_range_radius *= (1-world_manager.social_distancing_effect);
        }
        else 
        {
            social_distancing_checkbox.color = new Color32(255, 0, 0, 255);
            world_manager.virus_range_radius *= (1/(1-world_manager.social_distancing_effect));
        }
    }

    public Image masks_checkbox;
    private bool masks = false;
    public void update_masks()
    {
        masks = !masks;
        if (masks)
        {
            masks_checkbox.color = new Color32(0, 255, 0, 255);
            world_manager.chance_of_infection *= (1-world_manager.mask_effect);
        }
        else 
        {
            masks_checkbox.color = new Color32(255, 0, 0, 255);
            world_manager.chance_of_infection *= (1/(1-world_manager.mask_effect));
        }
    }

    public Image lockdown_checkbox;
    public void update_lockdown()
    {
        world_manager.lockdown = !world_manager.lockdown;
        if (world_manager.lockdown)
        {
            lockdown_checkbox.color = new Color32(0, 255, 0, 255);
        }
        else 
        {
            lockdown_checkbox.color = new Color32(255, 0, 0, 255);
        }
    }

    void Start()
    {

        pop_slider.value = world_manager.population_size;
        width_slider.value = world_manager.width;
        height_slider.value = world_manager.height;
        seed_slider.value = world_manager.seed;
        district_comp_slider.value = world_manager.district_competition;
        district_count_slider.value = world_manager.num_of_districts;
        vertical_road_slider.value = world_manager.vertical_road_threshhold;
        horizontal_road_slider.value = world_manager.horizontal_road_threshhold;
        blocksize_slider.value = world_manager.blocksize;
        work_ratio_slider.value = world_manager.num_of_works;
        school_ratio_slider.value = world_manager.num_of_schools;
        social_ratio_slider.value = world_manager.num_of_socials;
        house_ratio_slider.value = world_manager.num_of_houses;
        speed_slider.value = world_manager.speed;
        death_chance_slider.value = world_manager.death_chance;
        immunity_slider.value = world_manager.immunity_range;
        ticks_in_day_slider.value = world_manager.ticks_in_day;
        infection_chance_slider.value = world_manager.chance_of_infection;
        rate_of_infection_slider.value = world_manager.rate_of_infection_in_ticks;
        virus_range_slider.value = world_manager.virus_range_radius;
        initial_infected_slider.value = world_manager.initial_infected_probabilty;


        pop_text.text = "(" + pop_slider.value + ")";
        width_text.text = "(" + width_slider.value + ")";
        height_text.text = "(" + height_slider.value + ")";
        seed_text.text = "(" + seed_slider.value + ")";
        district_comp_text.text = "(" + district_comp_slider.value + ")";
        district_count_text.text = "(" + district_count_slider.value + ")";
        vertical_road_text.text = "(" + vertical_road_slider.value + ")";
        horizontal_road_text.text = "(" + horizontal_road_slider.value + ")";
        blocksize_text.text = "(" + blocksize_slider.value + ")";
        work_ratio_text.text = "(" + work_ratio_slider.value + ")";
        school_ratio_text.text = "(" + school_ratio_slider.value + ")";
        social_ratio_text.text = "(" + social_ratio_slider.value + ")";
        house_ratio_text.text = "(" + house_ratio_slider.value + ")";
        speed_text.text = "(" + speed_slider.value + ")";
        death_chance_text.text = "(" + death_chance_slider.value + ")";
        immunity_text.text = "(" + immunity_slider.value + ")";
        ticks_in_day_text.text = "(" + ticks_in_day_slider.value + ")";
        infection_chance_text.text = "(" + infection_chance_slider.value + ")";
        rate_of_infection_text.text = "(" + rate_of_infection_slider.value + ")";
        virus_range_text.text = "(" + virus_range_slider.value + ")";
        initial_infected_text.text = "(" + initial_infected_slider.value + ")";
        
    }

}
