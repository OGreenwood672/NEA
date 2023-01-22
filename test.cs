void FixedUpdate()
{
    update_game_ticks();

    foreach (Person person in people)
    {

        add_activity_time(person);

        add_path(person);

        move_people(person);

        render_person(person);
        
    }
}