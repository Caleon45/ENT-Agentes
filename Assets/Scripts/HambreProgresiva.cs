using UnityEngine;

[RequireComponent(typeof(Predator))]
public class HambreProgresiva : MonoBehaviour
{
    [Header("Configuracion de Hambre Progresiva")]
    public float tiempoParaAumento = 4f; // Intervalo de tiempo para aumentar agresividad por hambre
    public float incrementoVelocidad = 0.5f; // Cantidad de velocidad sumada por nivel de hambre
    public float velocidadMaxima = 4f; // Limite maximo de velocidad
    public float intensidadAgresiva = 0.4f; // Fuerza de movimientos erraticos y agresivos hacia la presa

    private Predator predator;
    private float tiempoSinComer = 0f;
    private float velocidadBase;

    private void Start()
    {
        predator = GetComponent<Predator>();
        if (predator != null)
        {
            velocidadBase = predator.speed;
        }
    }

    private void Update()
    {
        if (predator == null || !predator.isAlive) return;

        // Al comer, se sacia el hambre y recupera su velocidad normal
        if (predator.currentState == PredatorState.Eating)
        {
            tiempoSinComer = 0f;
            predator.speed = velocidadBase;
            return;
        }

        // Acumula tiempo transcurrido sin comer
        tiempoSinComer += Time.deltaTime;

        // Aumenta progresivamente la velocidad con el hambre
        if (tiempoSinComer >= tiempoParaAumento)
        {
            tiempoSinComer = 0f;
            if (predator.speed < velocidadMaxima)
            {
                predator.speed = Mathf.Min(predator.speed + incrementoVelocidad, velocidadMaxima);
            }
        }

        // Si esta en caceria, aplica impulsos mas rapidos y erraticos hacia la presa
        if (predator.currentState == PredatorState.SearchingFood)
        {
            Bunny presa = ObtenerPresaCercana();
            if (presa != null)
            {
                Vector3 direccionPresa = (presa.transform.position - transform.position).normalized;
                Vector3 desvioErratico = new Vector3(
                    Random.Range(-intensidadAgresiva, intensidadAgresiva),
                    Random.Range(-intensidadAgresiva, intensidadAgresiva),
                    0f
                );

                // Impulso adicional directo y agresivo hacia la presa
                transform.position += (direccionPresa + desvioErratico) * (predator.speed * 0.25f) * Time.deltaTime;
            }
        }
    }

    Bunny ObtenerPresaCercana()
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, predator.visionRange, LayerMask.GetMask("Bunnies"));
        Bunny presaCercana = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (var col in colisiones)
        {
            Bunny b = col.GetComponent<Bunny>();
            if (b != null && b.isAlive)
            {
                float dist = Vector2.Distance(transform.position, b.transform.position);
                if (dist < distanciaMinima)
                {
                    distanciaMinima = dist;
                    presaCercana = b;
                }
            }
        }

        return presaCercana;
    }
}
