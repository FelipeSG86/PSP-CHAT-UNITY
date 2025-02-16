using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

public class BasicWebSocketClient : MonoBehaviour
{
    // Instancia del cliente WebSocket
    private WebSocket ws;
    //TODO Referencia al ChatManager
    public ChatManager chatManager;
    //TODO Cola de mensajes para procesarlos en el hilo principal
    private Queue<string> messageQueue = new Queue<string>();  

    // Se ejecuta al iniciar la escena
    void Start()
    {
        // Crear una instancia del WebSocket apuntando a la URI del servidor  
        ws = new WebSocket("ws://127.0.0.1:7777/");

        //TODO Intenta encontrar automáticamente el ChatManager si no está asignado
        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }

        // Evento OnOpen: se invoca cuando se establece la conexión con el servidor
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket conectado correctamente.");
        };

        // Evento OnMessage: se invoca cuando se recibe un mensaje del servidor
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Mensaje recibido: " + e.Data);
            lock (messageQueue)
            {
                messageQueue.Enqueue(e.Data);
            }
        };

        // Evento OnError: se invoca cuando ocurre un error en la conexión
        ws.OnError += (sender, e) =>
        {
            Debug.LogError("Error en el WebSocket: " + e.Message);
        };

        // Evento OnClose: se invoca cuando se cierra la conexión con el servidor
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket cerrado. Código: " + e.Code + ", Razón: " + e.Reason);
        };

        // Conectar de forma asíncrona al servidor WebSocket
        ws.ConnectAsync();
    }

    void Update()
    {
        //TODO Procesar mensajes en la cola de mensajes en el hilo principal

        //TODO Comprobamos si hay mensajes en la cola para procesar
        while (messageQueue.Count > 0)
        {
            string message;

            //TODO Se bloquea la cola de mensajes para asegurar que ningún otro hilo acceda a ella mientras estamos procesando
            lock (messageQueue)
            {
                //TODO Extraemos el primer mensaje de la cola
                message = messageQueue.Dequeue();  
            }

            //TODO Verificamos si chatManager está asignado (es decir, no es null)
            if (chatManager != null)
            {
                //TODO Si chatManager no es null, procesamos el mensaje llamando al método ReceiveMessage
                chatManager.ReceiveMessage(message);
            }
            else
            {
                //TODO Si chatManager es null, mostramos una advertencia en la consola indicando que no se pudo mostrar el mensaje
                Debug.LogWarning("ChatManager no está asignado, mensaje no mostrado.");
            }
        }
    }

    // Método para enviar un mensaje al servidor
    public void SendMessageToServer(string message)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(message);
        }
        else
        {
            Debug.LogError("No se puede enviar el mensaje. La conexión no está abierta.");
        }
    }

    // Se ejecuta cuando el objeto se destruye (por ejemplo, al cambiar de escena o cerrar la aplicación)
    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}
