using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Communication.Language.Scramblers;

public class WordMaskScrambler : ILanguageScrambler
{
    protected WordMaskScrambler()
    {
        Mask = "..."; // TODO - dynamically load
    }

    public static WordMaskScrambler Instance { get; } = new();

    public virtual string Mask { get; protected set; }

    public string Name => "Word Mask";

    public string Description => "All words that are not understood are masked by \"...\".";

    public bool Instanced => false;

    public string Scramble(string input, double obscuredratio)
    {
        return Scramble(new ExplodedString(input), obscuredratio);
    }

    public string Scramble(ExplodedString exploded, double obscuredratio)
    {
        // Work out how many words will be obscured. Do a little bit of a randomisation so that short sentences may still be obscured.
        double rawobscuredwords = exploded.Words.Count * obscuredratio;
        int obscuredwords =
            (int)
            (Constants.Random.NextDouble() <= rawobscuredwords % 1.0
                ? Math.Floor(rawobscuredwords)
                : Math.Ceiling(rawobscuredwords));

        // Get a queue of truth values in the word ratio we've requested
        Queue<bool> mask = RandomUtilities.RandomTruthMask(obscuredwords, exploded.Words.Count - obscuredwords);

        // Replace obscured words with obscure character
        for (int i = 0; i < exploded.Words.Count; i++)
        {
            if (mask.Dequeue())
            {
                exploded.Words[i] = Mask;
            }
        }

        return exploded.ToString();
    }

    public string Scramble(ExplodedString exploded, double shortbiasedobscuredratio, double longbiasedobscuredratio)
    {
        // Work out how many words will be obscured. Do a little bit of a randomisation so that short sentences may still be obscured.
        double rawshortobscuredwords = exploded.Words.Count * shortbiasedobscuredratio;
        int shortobscuredwords =
            (int)
            (Constants.Random.NextDouble() <= rawshortobscuredwords % 1.0
                ? Math.Floor(rawshortobscuredwords)
                : Math.Ceiling(rawshortobscuredwords));

        // Get a queue of truth values in the word ratio we've requested
        Queue<bool> shortmask = RandomUtilities.RandomBiasedTruthMask(shortobscuredwords,
            exploded.Words.Count - shortobscuredwords,
            exploded.Words.SelectNotNull(x => 1.0 / Math.Pow(x.Length, 1.85)).ToArray());

        // Work out how many words will be obscured. Do a little bit of a randomisation so that short sentences may still be obscured.
        double rawlongobscuredwords = exploded.Words.Count * longbiasedobscuredratio;
        int longobscuredwords =
            (int)
            (Constants.Random.NextDouble() <= rawlongobscuredwords % 1.0
                ? Math.Floor(rawlongobscuredwords)
                : Math.Ceiling(rawlongobscuredwords));

        // Get a queue of truth values in the word ratio we've requested
        Queue<bool> longmask = RandomUtilities.RandomBiasedTruthMask(longobscuredwords,
            exploded.Words.Count - longobscuredwords,
            exploded.Words.SelectNotNull(x => Math.Pow(x.Length, 1.85)).ToArray());

        Queue<bool> mask = new();
        for (int i = 0; i < exploded.Words.Count; i++)
        {
            mask.Enqueue(shortmask.Dequeue() | longmask.Dequeue());
        }


        // Replace obscured words with obscure character
        for (int i = 0; i < exploded.Words.Count; i++)
        {
            if (mask.Dequeue())
            {
                exploded.Words[i] = Mask;
            }
        }

        return exploded.ToString();
    }
}