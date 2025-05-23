using UnityEngine;

public class MovimientoPlataformaVertical : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento de la plataforma")]
    public float velocidad = 0.5f;
    
    [Tooltip("Distancia máxima que subirá la plataforma desde su posición inicial")]
    public float distanciaArriba = 0.0005f;
    
    [Tooltip("Distancia máxima que bajará la plataforma desde su posición inicial")]
    public float distanciaAbajo = 0.0005f;
    
    [Header("Configuración de Comportamiento")]
    [Tooltip("¿Debe esperar la plataforma al llegar a los límites?")]
    public bool esperarEnLimites = false;
    
    [Tooltip("Tiempo de espera en segundos cuando llega a un límite")]
    public float tiempoEspera = 1.0f;
    
    // Variables privadas
    private Vector3 posicionInicial;
    private float tiempoEsperando = 0f;
    private bool moviendoseHaciaArriba = true;
    private bool esperando = false;

    private void Start()
    {
        // Guardar la posición inicial de la plataforma
        posicionInicial = transform.position;
    }

    private void Update()
    {
        // Si está en modo espera, controlar el temporizador
        if (esperando)
        {
            tiempoEsperando += Time.deltaTime;
            if (tiempoEsperando >= tiempoEspera)
            {
                esperando = false;
                tiempoEsperando = 0f;
                // Cambiar dirección después de esperar
                moviendoseHaciaArriba = !moviendoseHaciaArriba;
            }
            return;
        }

        // Calcular posición actual relativa a la posición inicial
        float distanciaActual = transform.position.y - posicionInicial.y;
        
        // Determinar si necesita cambiar de dirección
        if (moviendoseHaciaArriba && distanciaActual >= distanciaArriba)
        {
            if (esperarEnLimites)
            {
                esperando = true;
            }
            else
            {
                moviendoseHaciaArriba = false;
            }
        }
        else if (!moviendoseHaciaArriba && distanciaActual <= -distanciaAbajo)
        {
            if (esperarEnLimites)
            {
                esperando = true;
            }
            else
            {
                moviendoseHaciaArriba = true;
            }
        }
        
        // Calcular dirección y aplicar movimiento
        float direccion = moviendoseHaciaArriba ? 1f : -1f;
        Vector3 movimiento = new Vector3(0, direccion * velocidad * Time.deltaTime, 0);
        transform.Translate(movimiento);
    }

    // Esta función permite que los objetos se muevan junto con la plataforma
    private void OnCollisionStay(Collision collision)
    {
        // Si es el jugador u otro objeto que deba moverse con la plataforma
        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Cuando el objeto deja la plataforma, eliminar la relación padre-hijo
        collision.transform.SetParent(null);
    }

    // Opcionalmente, dibujar gizmos en el editor para visualizar el recorrido
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
        {
            posicionInicial = transform.position;
        }
        
        Gizmos.color = Color.green;
        // Dibujar línea del recorrido
        Gizmos.DrawLine(
            posicionInicial + new Vector3(0, -distanciaAbajo, 0),
            posicionInicial + new Vector3(0, distanciaArriba, 0)
        );
        
        // Dibujar puntos en los límites
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(posicionInicial + new Vector3(0, distanciaArriba, 0), 0.2f);
        Gizmos.DrawSphere(posicionInicial + new Vector3(0, -distanciaAbajo, 0), 0.2f);
    }
}