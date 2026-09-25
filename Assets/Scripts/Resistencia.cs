using UnityEngine;

[RequireComponent(typeof(Predator))]
public class Resistencia : MonoBehaviour
{
    [Header("Configuracion de Cansancio")]
    public float tiempoMaximoPersecucion = 10f; // Tiempo maximo de caceria antes de cansarse
    public float tiempoDescanso = 5f; // Tiempo de recuperacion tras cansarse

    private Predator predator;
    private float temporizadorPersecucion = 0f;
    private float temporizadorDescanso = 0f;
    public bool estaCansado = false;

    private void Start()
    {
        predator = GetComponent<Predator>();
    }

    private void Update()
    {
        if (predator == null || !predator.isAlive) return;

        // Si esta en modo descanso por cansancio
        if (estaCansado)
        {
            temporizadorDescanso += Time.deltaTime;

            // Mientras descansa, evita que persiga presas y lo mantiene explorando
            if (predator.currentState == PredatorState.SearchingFood)
            {
                predator.currentState = PredatorState.Exploring;
            }

            // Al terminar el descanso, recupera su energia
            if (temporizadorDescanso >= tiempoDescanso)
            {
                estaCansado = false;
                temporizadorDescanso = 0f;
                temporizadorPersecucion = 0f;
            }
            return;
        }

        // Si esta persiguiendo a una presa
        if (predator.currentState == PredatorState.SearchingFood)
        {
            temporizadorPersecucion += Time.deltaTime;

            // Si supera el tiempo maximo, se cansa y abandona la persecucion
            if (temporizadorPersecucion >= tiempoMaximoPersecucion)
            {
                estaCansado = true;
                temporizadorPersecucion = 0f;
                temporizadorDescanso = 0f;
                predator.currentState = PredatorState.Exploring;
            }
        }
        else
        {
            // Resetea el contador si no esta persiguiendo
            temporizadorPersecucion = 0f;
        }
    }
}
