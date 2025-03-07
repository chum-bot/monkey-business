using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static MBNamespace.MBVars;

//i don't think i've ever made a namespace before but
//i have to say it's actually the best thing
//if i don't know where to put a function i can just toss it in the namespace
namespace MBNamespace
{
    public class MBVars : MonoBehaviour
    {
        //maybe some barrels would have you sort by color or by type
        //or by what hat they're wearing, i think someone mentioned giving them funny hats
        //so that's what this is
        public enum SORTING
        {
            color,
            type,
            accessory
        };

        //not affiliated with monkeytype.com...
        public enum MONKEYTYPE
        {
            normal,
            rocket,
            fat,
            bomb
        };

        public static int comboCount = 0;
        public static Array types = Enum.GetValues(typeof(MONKEYTYPE));
        public static List<Texture> monkeyTextures = MBFunctions.GetMonkeyTextures();
        public static List<string> barrelColors = MBFunctions.GetBarrelColors();
    }
    public class MBFunctions : MonoBehaviour
    {
        public static bool IsSorted(Monkey monkey, Barrel barrel)
        {
            if (monkey.sortingMetric == barrel.sortingMetric)
            {
                switch (monkey.sortingMetric)
                {
                    case SORTING.color:
                        return monkey.color == barrel.color;
                    //noticing i never explicitly check if it's true here
                    //i just return the result, and the other thing that uses it will handle that

                    //we can easily add more metrics and do more stuff here if we wanted to/had the time for it
                    //it'd be easier in the future

                    default:
                        return false;

                }
            }
            return false;
        }
        //this is for the random monkey colors mainly
        //it'll pick from the colors of all the barrels in the scene
        //and this gets those colors
        public static List<string> GetBarrelColors()
        {
            UnityEngine.Object[] barrels = FindObjectsOfType(typeof(Barrel));
            List<string> colors = new List<string>();
            foreach (Barrel barrel in barrels)
            {
                string name = barrel.GetComponent<Renderer>().material.name;
                colors.Add(name.Substring(0, name.IndexOf(" ")));
                barrel.color = name.Substring(0, name.IndexOf(" "));
            }
            return colors;
        }

        public static List<Texture> GetMonkeyTextures()
        {
            List<Texture> monkeyTxtrs = new List<Texture>();
            UnityEngine.Object[] textures = Resources.FindObjectsOfTypeAll(typeof(Texture));
            foreach (Texture txtr in textures)
            {
                if (txtr.name.Contains("Monkey"))
                {
                    monkeyTxtrs.Add(txtr);
                }
            }
            return monkeyTxtrs;
        }

        public static Texture RandomizedMonkeyTexture()
        {
            List<Texture> validTextures = new List<Texture>();
            foreach (Texture txtr in monkeyTextures)
            {
                foreach (string color in barrelColors)
                {
                    if (txtr.name.Contains(color))
                    {
                        validTextures.Add(txtr);
                    }
                }
            }
            int randMonkeyMat = UnityEngine.Random.Range(0, validTextures.Count);
            return validTextures[randMonkeyMat];
        }

        //the respawning
        //really just warping the monkey back to the start and giving it new properties
        public static void CreateMonkey(Monkey monkey)
        {
            monkey.type = (MONKEYTYPE)UnityEngine.Random.Range(0, types.Length);

            monkey.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", RandomizedMonkeyTexture());
            string monkeyTexColor = monkey.GetComponentInChildren<Renderer>().material.mainTexture.name;
            monkey.color = monkeyTexColor.Substring(0, monkeyTexColor.IndexOf("M"));
            monkey.transform.position = monkey.initPos;
            monkey.rb.velocity = Vector3.zero;
            monkey.transform.rotation = new Quaternion(0, 0, 0, 1);
            monkey.fly = false;

            //the stuff for the different attributes of each type
            //i can always separate this into smth else if it ends up being too big
            switch (monkey.type)
            {
                case MONKEYTYPE.normal:
                    monkey.transform.localScale = new Vector3(1, 1, 1);
                    monkey.typeforce = new Vector3(0, 0);
                    monkey.rb.mass = 1.0f;
                    break;
                case MONKEYTYPE.fat:
                    monkey.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                    monkey.rb.mass = 1.3f;
                    break;
                case MONKEYTYPE.rocket:
                    monkey.transform.localScale = new Vector3(.75f, .75f, .75f);
                    monkey.rb.mass = 0.9f;
                    break;
                case MONKEYTYPE.bomb: //nothing special for now
                    monkey.transform.localScale = new Vector3(1, 1, 1);
                    monkey.typeforce = new Vector3(0, 0);
                    monkey.rb.mass = 1.0f;
                    break;
            }
            Debug.Log($"{monkey.type} monkey incoming!");
        }

        //the antigrab
        //stops you from grabbing the monkey if it's past the death zone
        //i actually just gave the monkey a fly parameter that serves the same purpose but
        //i ran into an issue where i could only pick it up once
        //didn't want to bother fixing it so i just added this back
        public static bool AntiGrab(Monkey monkey)
        {
            Array deathZones = FindObjectsOfType<DeathZone>();
            float closestStartZDist = 999999;
            Vector3 closestStart = Vector3.zero;
            foreach (DeathZone deathZone in deathZones)
            {
                float startXDist = Math.Abs(deathZone.start.z);
                if (startXDist < closestStartZDist)
                {
                    closestStartZDist = startXDist;
                    closestStart = deathZone.start;
                }
            }
            return closestStart.z > monkey.transform.position.z;
        }
    }
}