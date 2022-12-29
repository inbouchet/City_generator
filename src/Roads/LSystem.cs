using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystem
{
    private string axiomes;
    private List<Dictionary<char,string>> regles;
    private string resultat = string.Empty;
    public int iteration = 1;
    public LSystem(List<Dictionary<char,string>> regles, string axiomes, int iteration)
    {
        this.axiomes = axiomes;
        this.regles = regles;
        this.iteration = iteration;
    }

    public string Generate()
    {
        resultat = axiomes;
        for(int i = 0; i<iteration ; i++)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<char,string> regle = regles[Random.Range(0,regles.Count)];

            foreach(char c in resultat)
            {
                if(regle.ContainsKey(c))
                {
                    sb.Append(regle[c]);
                }else{
                    sb.Append(c.ToString());
                }
            }
            resultat = sb.ToString();
        }
        return resultat;
    }
}
