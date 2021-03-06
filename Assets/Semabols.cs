﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rnd = UnityEngine.Random;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class Semabols : MonoBehaviour {
    public Sprite[] symbols;
    public GameObject[] leds;
    public SpriteRenderer[] outerSymbolRenderers;
    public SpriteRenderer[] innerSymbolRenderers;
    public KMSelectable[] innerSymbolSelectables;
    public class OuterSymbol
    {
        public Sprite symbol;
        public SpriteRenderer renderer;
        public GameObject[] leds = new GameObject[4];
        public bool[] ledsActive = new bool[4];
    }
    public class InnerSymbol
    {
        public Semaphore semaphore = new Semaphore();
        public SpriteRenderer renderer;
        public Sprite symbol;
    }
    public struct Semaphore
    {
        public int[] positions;
        public int[] cubeIndices;
        public List<int> forbiddenSemaphoreIndices;

        public Semaphore(int[] a, int[] b, int c)
        {
            positions = a;
            cubeIndices = b;
            forbiddenSemaphoreIndices = new List<int>();
            forbiddenSemaphoreIndices.Add(c);
        }
        public Semaphore(int[] a, int[] b)
        {
            positions = a;
            cubeIndices = b;
            forbiddenSemaphoreIndices = new List<int>();
        }
    }
    static Semaphore[] semaphores = new Semaphore[] {
    new Semaphore(new int[] {5, 6}, new int[] {0,0,0}, 2 ),
    new Semaphore(new int[] {3, 6}, new int[] {0,0,1}, 15 ),
    new Semaphore(new int[] {0, 6}, new int[] {0,0,2}, 10 ),
    new Semaphore(new int[] {1, 6}, new int[] {0,1,0}, 19),
    new Semaphore(new int[] {2, 6}, new int[] {0,1,1}, 4 ),
    new Semaphore(new int[] {4, 6}, new int[] {0,1,2}, 9),
    new Semaphore(new int[] {6, 7}, new int[] {0,2,0}),
    new Semaphore(new int[] {3, 5}, new int[] {0,2,1}, 14),
    new Semaphore(new int[] {0, 5}, new int[] {0,2,2}, 25),
    new Semaphore(new int[] {1, 4}, new int[] {1,0,0}, 22),
    new Semaphore(new int[] {1, 5}, new int[] {1,0,1}, 20),
    new Semaphore(new int[] {2, 5}, new int[] {1,0,2}),
    new Semaphore(new int[] {4, 5}, new int[] {1,1,0}, 7),
    new Semaphore(new int[] {5, 7}, new int[] {1,1,2}, 3),
    new Semaphore(new int[] {0, 3}, new int[] {1,2,0}, 24),
    new Semaphore(new int[] {1, 3}, new int[] {1,2,1}, 18),
    new Semaphore(new int[] {2, 3}, new int[] {1,2,2}, 1),
    new Semaphore(new int[] {3, 4}, new int[] {2,0,0}, 0),
    new Semaphore(new int[] {3, 7}, new int[] {2,0,1}, 16),
    new Semaphore(new int[] {0, 1}, new int[] {2,0,2}, 17),
    new Semaphore(new int[] {0, 2}, new int[] {2,1,0}, 13),
    new Semaphore(new int[] {1, 7}, new int[] {2,1,1}, 15),
    new Semaphore(new int[] {2, 4}, new int[] {2,1,2}, 21),
    new Semaphore(new int[] {2, 7}, new int[] {2,2,0}, 6),
    new Semaphore(new int[] {0, 4}, new int[] {2,2,1}, 12),
    new Semaphore(new int[] {4, 7}, new int[] {2,2,2}, 5),
    };
    Semaphore[] moduleSemaphores = new Semaphore[6];
    OuterSymbol[] outerSymbols = new OuterSymbol[] { new OuterSymbol(), new OuterSymbol(), new OuterSymbol(), new OuterSymbol(), new OuterSymbol(), new OuterSymbol(), new OuterSymbol(), new OuterSymbol() };
    public InnerSymbol[] innerSymbols = new InnerSymbol[] { new InnerSymbol(), new InnerSymbol(), new InnerSymbol(), new InnerSymbol(), new InnerSymbol(), new InnerSymbol() };
    Sprite[][][] cubeLayers = new Sprite[9][][];
    Sprite[][][] cube = new Sprite[3][][];
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    int stage;
    bool solved;
    public KMBombModule module;
    public KMAudio sound;
    int moduleId;
    static int moduleIdCounter = 1;

    private void Awake()
    {
        moduleId = moduleIdCounter++;
    }
    private void Start()
    {
        BuildCube();
        CreateCrossSection();
        PickSemaphore();
        PrepareStart();
    }
    void BuildCube()
    {
        int index = 0;
        for (int i = 0; i < 9; i++)
        {
            cubeLayers[i] = new Sprite[3][];
            for (int j = 0; j < 3; j++)
            {
                cubeLayers[i][j] = new Sprite[3];
                for (int k = 0; k < 3; k++)
                {
                    cubeLayers[i][j][k] = symbols[index];
                    index++;
                }
            }
        }
        IEnumerable layers = Enumerable.Range(0, 9).ToList().Shuffle().Take(3);
        int l = 0;
        foreach (int layer in layers)
        {
            cube[l] = cubeLayers[layer];
            Debug.LogFormat("[Semabols #{0}] Layer {1} of the cube is layer {2}.", moduleId, l + 1, layer + 1);
            l++;
        }
    }
    void CreateCrossSection() {
        string axes = "XY"; //Really, me?
        int axis = rnd.Range(0, 2);
        int slice = rnd.Range(0, 3);
        int index = 0;
        Debug.LogFormat("[Semabols #{0}] The cross-section is along the {1} axis and is at coordinate {2}.", moduleId, axes[axis], slice + 1);
        for (int i = 0; i < 3; i++)
        {
            switch (axis)
            {
                case 0:
                for (int j = 0; j < 3; j++)
                    {
                        if (!(i == 1 && j == 1))
                        {
                            outerSymbols[index].symbol = cube[i][slice][j];
                            index++;
                        }
                    }
                    break;
                case 1:
                    for (int j = 0; j < 3; j++)
                    {
                        if (!(i == 1 && j == 1))
                        {
                            outerSymbols[index].symbol = cube[i][j][slice];
                            index++;
                        }
                    }
                    break;
            }
        }
    }
    void PickSemaphore()
    {
        List<int> chosenSemaphoreIndices = new List<int>();
        List<int> forbiddenSemaphoreIndices = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            int candidateSemaphoreIndex = rnd.Range(0, 26);
            while (chosenSemaphoreIndices.Contains(candidateSemaphoreIndex) || (forbiddenSemaphoreIndices.Contains(candidateSemaphoreIndex) && TestForbiddenIndices(candidateSemaphoreIndex) && i > 3)) candidateSemaphoreIndex = rnd.Range(0, 26);
            chosenSemaphoreIndices.Add(candidateSemaphoreIndex);
            if (semaphores[candidateSemaphoreIndex].forbiddenSemaphoreIndices.Count() != 0) forbiddenSemaphoreIndices.Add(semaphores[candidateSemaphoreIndex].forbiddenSemaphoreIndices[0]);
            moduleSemaphores[i] = semaphores[candidateSemaphoreIndex];
            innerSymbols[i].semaphore = semaphores[candidateSemaphoreIndex];
            innerSymbols[i].symbol = cube[semaphores[candidateSemaphoreIndex].cubeIndices[0]][semaphores[candidateSemaphoreIndex].cubeIndices[1]][semaphores[candidateSemaphoreIndex].cubeIndices[2]];
            if (i < 4)
            {
                Debug.LogFormat("[Semabols #{0}] LED pair {1} corresponds to the letter {2}.", moduleId, i + 1, alphabet[candidateSemaphoreIndex]);
                foreach (int j in semaphores[candidateSemaphoreIndex].positions)
                {
                    outerSymbols[j].ledsActive[i] = true;
                }
            }
        }
    }
    bool TestForbiddenIndices(int testIndex) {
        foreach (Semaphore semaphore in moduleSemaphores)
        {
            if (semaphores[testIndex].forbiddenSemaphoreIndices.Contains(Array.IndexOf(semaphores, semaphore)))
            {
                return true;
            }
        }
        return false;
    }
    void PrepareStart()
    {
        outerSymbols = outerSymbols.Shuffle();
        innerSymbols = innerSymbols.Shuffle();
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            outerSymbols[i].renderer = outerSymbolRenderers[i];
            for (int j = 0; j < 4; j++)
            {
                outerSymbols[i].leds[j] = leds[index];
                index++;                
            }
        }
        for (int i = 0; i < 6; i++)
        {
            int j = i;
            innerSymbols[i].renderer = innerSymbolRenderers[i];
            innerSymbolSelectables[j].OnInteract += delegate () { PressSymbol(innerSymbolSelectables[j]); return false; };
        }
        module.OnActivate += Activate;
    }
    void PressSymbol(KMSelectable symbol)
    {
        if (!solved)
        {
            symbol.AddInteractionPunch();
            Debug.LogFormat("[Semabols #{0}] You pressed the symbol corresponding to {1}.", moduleId, alphabet[Array.IndexOf(semaphores, innerSymbols[Array.IndexOf(innerSymbolSelectables, symbol)].semaphore)]);
            if (innerSymbols[Array.IndexOf(innerSymbolSelectables, symbol)].semaphore.Equals(moduleSemaphores[stage]))
            {
                stage++;
                Debug.LogFormat("[Semabols #{0}] That was correct.", moduleId);
                if (stage == 4)
                {
                    module.HandlePass();
                    sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    Debug.LogFormat("[Semabols #{0}] Module solved.", moduleId);
                    solved = true;
                }
            }
            else
            {
                stage = 0;
                module.HandleStrike();
                Debug.LogFormat("[Semabols #{0}] That was incorrect. Strike!", moduleId);
            }
        }
    }
    void Activate() {
        for (int i = 0; i < 8; i++)
        {
            if (i < 6) innerSymbols[i].renderer.sprite = innerSymbols[i].symbol;
            outerSymbols[i].renderer.sprite = outerSymbols[i].symbol;
            for (int j = 0; j < 4; j++) outerSymbols[i].leds[j].SetActive(outerSymbols[i].ledsActive[j]);
        }
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <#> (#)... [Presses the inner symbol in the specified position (optionally include multiple positions)] | Valid positions are 1-6 in reading order";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the position(s) of the inner symbol(s) you wish to press!";
            }
            else
            {
                for (int i = 1; i < parameters.Length; i++)
                {
                    int temp = 0;
                    if (!int.TryParse(parameters[i], out temp))
                    {
                        yield return "sendtochaterror The specified position '" + parameters[i] + "' is invalid!";
                        yield break;
                    }
                    if (temp < 1 || temp > 6)
                    {
                        yield return "sendtochaterror The specified position '" + parameters[i] + "' is out of range 1-6!";
                        yield break;
                    }
                }
                for (int i = 1; i < parameters.Length; i++)
                {
                    innerSymbolSelectables[int.Parse(parameters[i]) - 1].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        int start = stage;
        for (int i = start; i < 4; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (innerSymbols[j].semaphore.Equals(moduleSemaphores[i]))
                {
                    innerSymbolSelectables[j].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                    break;
                }
            }
        }
    }
}