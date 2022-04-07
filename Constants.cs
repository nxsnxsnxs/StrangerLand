using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    //common
    public const float deadbody_disappear_time = 3f;
    public const float max_chase_distance = 15f;
    public const int normal_inventory_maxstack = 20;
    //projectile fly speed
    public const float slow_projectile_fly_speed = 7.5f;
    public const float normal_projectile_fly_speed = 9.5f;
    public const float fast_projectile_fly_speed = 12f;
    //projectile fly maxdistance
    public const float normal_projectile_fly_maxdistance = 14;
    //attack duration
    public const float hand_attack_duration = 1.8f;
    public const float tool_attack_duration = 1.4f;
    public const float sword_attack_duration = 0.7f;
    //attack cd
    public const float normal_min_attack_gap = 1;
    public const float fast_cool_down_time = 2.5f;
    public const float normal_cool_down_time = 3.5f;
    public const float slow_cool_down_time = 5f;
    //attack distance
    public const float try_attack_distance = 12;
    public const float hand_attack_distance = 2.0f;
    public const float hand_hit_distance = 2.1f;
    public const float tool_attack_distance = 2.1f;
    public const float tool_hit_distance = 2.3f;
    public const float sword_attack_distance = 2.3f;
    public const float sword_hit_distance = 2.5f;
    //damage
    public const int hand_damage = 12;
    public const int axe_damage = 20;
    public const int pickaxe_damage = 20;
    public const int rookie_sword_damage = 29;
    public const int justice_sword_damage = 69;    
    public const float normal_check_radius = 10f;
    //construct
    public const float construct_distance = 1;
    //craft
    public const float try_craft_distance = 12;
    public const float craft_distance = 1;
    //construct time
    public const float normal_construct_time = 3f;
    //inspect
    public const string default_rimLightMat_path = "RimLightWhite";
    public const string rookie_sword_inspect_str = "具有不俗的伤害的武器，适合与初级怪物对抗";
    //inventory
    public const int default_inventory_size = 11;
    //player
    public const float player_move_speed = 5;
    public const int maleknight_max_health = 100;
    //maxwork
    public const int rock_maxwork = 8;

    //golem
    public const int golem_health = 60;
    public const float golem_move_speed = 2.5f;
    public const float golem_dash_speed = 3.4f;
    public const float golem_attack_cd = 3.5f;
    public const float golem_attack_damage = 20;
    public const float golem_attack_duration = 1.5f;
    public const float golem_attack_distance = 2.1f;
    public const float golem_hit_distance = 2.5f;
    public const float golem_spin_cd = 10;
    public const float golem_spin_attack_distance = 2.3f;
    public const float golem_spin_hit_distance = 2.8f;
    public const float golem_spin_attack_duration = 1.2f;
    public const float golem_spin_attack_damage = 40f;
    public const float golem_backhome_dist = 14f;
    //scorpionking
    public const int scorpionking_health = 800;
    public const float scorpionking_move_speed = 4.5f;
    public const float scorpionking_target_range = 10f;
    public const float scorpionking_attack_cd = 4.5f;
    public const float scorpionking_attack_damage = 40;
    public const float scorpionking_attack_duration = 0f;
    public const float scorpionking_attack_distance = 3f;
    public const float scorpionking_hit_distance = 3.5f;
    public const float scorpionking_tail_attack_cd = 12f;
    public const float scorpionking_tail_attack_damage = 55;
    public const float scorpionking_tail_attack_duration = 0f;
    public const float scorpionking_tail_attack_distance = 4f;
    public const float scorpionking_tail_hit_distance = 4.8f;
    public const float scorpionking_summon_radius = 8;
    public const float scorpionking_summon_cd = 8;
    public const int scorpionking_summon_maxcount = 4;
    //scorpion
    public const int scorpion_health = 75;
    public const float scorpion_move_speed = 3f;
    public const float scorpion_run_speed = 4f;
    public const float scorpion_attack_cd = 4.5f;
    public const float scorpion_attack_damage = 25;
    public const float scorpion_attack_duration = 0f;
    public const float scorpion_attack_distance = 2f;
    public const float scorpion_hit_distance = 2.3f;
    public const float scorpion_telson_attack_cd = 10f;
    public const float scorpion_telson_attack_damage = 15;
    public const float scorpion_telson_attack_duration = 0f;
    public const float scorpion_telson_attack_distance = 6.5f;
    public const float scorpion_max_noattack_time = 13f;

}
