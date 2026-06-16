using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selector reutilizable por cualquier agente. 
/// Permite registrar opciones con pesos y seleccionar una opción aleatoria según los pesos acumulados.
/// </summary>
public class RouletteWheelSelector<T>
{
    private readonly Dictionary<T, float> _weights = new Dictionary<T, float>();
    private float _totalWeight = 0f;

    /// <summary>
    /// Registra una opción con su peso inicial. Si ya existe, lo sustituye.
    /// </summary>
    public void Register(T option, float weight)
    {
        if (_weights.ContainsKey(option))
        {
            _totalWeight -= _weights[option];
            _weights[option] = Mathf.Max(0f, weight);
            _totalWeight += _weights[option];
        }
        else
        {
            _weights[option] = Mathf.Max(0f, weight);
            _totalWeight += _weights[option];
        }
    }

    /// <summary>
    /// Actualiza el peso de una opción ya registrada. Si no existe, la registra.
    /// </summary>
    public void UpdateWeight(T option, float weight)
    {
        Register(option, weight);
    }

    /// <summary>
    /// Elimina todas las opciones.
    /// </summary>
    public void Clear()
    {
        _weights.Clear();
        _totalWeight = 0f;
    }

    /// <summary>
    /// Selecciona una opción aleatoria según los pesos actuales.
    /// Si la suma de pesos es 0 devuelve el primer elemento registrado (si existe) o default(T).
    /// </summary>
    public T Select()
    {
        if (_weights.Count == 0)
            return default(T);

        if (_totalWeight <= 0f)
        {
            // Todos los pesos son 0 -> fallback determinista al primer elemento
            foreach (var kv in _weights)
                return kv.Key;
            return default(T);
        }

        float roll = UnityEngine.Random.value * _totalWeight;
        float cumulative = 0f;

        foreach (var kv in _weights)
        {
            cumulative += kv.Value;
            if (roll <= cumulative)
                return kv.Key;
        }

        // Seguridad: retornar último
        T last = default(T);
        foreach (var kv in _weights)
            last = kv.Key;
        return last;
    }
}
