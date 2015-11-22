using UnityEngine;
using System.Collections;

public class code_snippet_script : MonoBehaviour {

	// Use this for initialization
	public Psycho_master psycho_script;

	public bool no_enemies;
	public bool danger;
	public bool battle;

	public bool hit_for_surprise_effect;
	public bool reset_samples;

	void Start () {
	


	}
	
	// Update is called once per frame
	void Update () {

		if (reset_samples) {
			psycho_script.reset = true;
			reset_samples = false;
		}

		if (no_enemies) {
			psycho_script.ambiant_long_bool = true;

			psycho_script.tension_long_bool = false;
			psycho_script.tension_short_bool = false;
			psycho_script.tension_percs_bool = false;

			psycho_script.terror_long_bool = false;
			psycho_script.terror_bass_bool = false;
			psycho_script.terror_percs_bool = false;
			psycho_script.terror_intense_bool = false;
			psycho_script.terror_background_bool = false;
		}

		if (danger) {
			psycho_script.ambiant_long_bool = false;
			
			psycho_script.tension_long_bool = true;
			psycho_script.tension_short_bool = true;
			psycho_script.tension_percs_bool = true;
			
			psycho_script.terror_long_bool = false;
			psycho_script.terror_bass_bool = false;
			psycho_script.terror_percs_bool = false;
			psycho_script.terror_intense_bool = false;
			psycho_script.terror_background_bool = false;
		}

		if (battle) {
			psycho_script.ambiant_long_bool = false;
			
			psycho_script.tension_long_bool = false;
			psycho_script.tension_short_bool = false;
			psycho_script.tension_percs_bool = false;
			
			psycho_script.terror_long_bool = true;
			psycho_script.terror_bass_bool = true;
			psycho_script.terror_percs_bool = true;
			psycho_script.terror_intense_bool = true;
			psycho_script.terror_background_bool = true;
		}


		if (hit_for_surprise_effect) {
			psycho_script.hits = true;
			hit_for_surprise_effect = false;
		}

	}
}
