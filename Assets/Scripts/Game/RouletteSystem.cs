using UnityEngine;

public enum RouletteResult
{
    Wait,
    Back,
    ForwardTwo
}

public class RouletteSystem : MonoBehaviour
{
    public RouletteResult Roll()
    {
        int r = Random.Range(0, 3);

        switch (r)
        {
            case 0: return RouletteResult.Wait;
            case 1: return RouletteResult.Back;
            case 2: return RouletteResult.ForwardTwo;
        }

        return RouletteResult.Wait;
    }
}