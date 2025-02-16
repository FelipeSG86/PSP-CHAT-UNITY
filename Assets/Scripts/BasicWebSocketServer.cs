using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor;

//Clase que se adjunta a un GameObject en Unity para iniciar el servidor WebSocket.
public class BasicWebSocketServer : MonoBehaviour
{
    //Instancia del servidor WebSocket.
    private WebSocketServer wss;


    //Se ejecuta al iniciar la escena.
    void Start()
    {
        //Crear un servidor WebSocket que escucha en el puerto 7777.
        wss = new WebSocketServer(7777);

        //Añadir un servicio en la ruta "/" que utiliza el comportamiento EchoBehavior.
        wss.AddWebSocketService<EchoBehavior>("/");

        //Iniciar el servidor.
        wss.Start();

        Debug.Log("Servidor WebSocket iniciado en ws://127.0.0.1:7777/");


    }

    //Se ejecuta cuando el objeto se destruye (por ejemplo, al cerrar la aplicación o cambiar de escena).
    void OnDestroy()
    {
        //Si el servidor está activo, se detiene de forma limpia.
        if (wss != null)
        {
            wss.Stop();
            wss = null;
            Debug.Log("Servidor WebSocket detenido.");
        }
    }


}

// Comportamiento básico del servicio WebSocket: simplemente devuelve el mensaje recibido.
public class EchoBehavior : WebSocketBehavior
{
    //TODO Contador estático para asignar identificadores únicos a los usuarios.
    private static int connectionCount = 0;
    private string userName;
    //TODO Diccionario de colores para cada usuario (opcional, puedes asignar un color en función del número de usuario).
    private Dictionary<string, string> userColors;



    //TODO Se invoca cuando un cliente se conecta.
    protected override void OnOpen()
    {


        userColors = new Dictionary<string, string>
        {
            { "Usuario1", "#FF5733" },
            { "Usuario2", "#3357FF" },
            { "Usuario3", "#33FF57" },
            { "Usuario4", "#9B59B6" },
            { "Usuario5", "#F5B041" }
        };
        //TODO Incrementa el contador y asigna un nombre de usuario único.
        connectionCount++;
        userName = "Usuario" + connectionCount;

        //TODO Notifica a todos los clientes que un usuario se ha conectado.
        string userColor = userColors.ContainsKey(userName) ? userColors[userName] : "#FFFFFF";

        Sessions.Broadcast($"<color={userColor}><b>{userName} se ha conectado.</b></color>");
        Debug.Log($"{userName} se ha conectado.");
    }

    //Se invoca cuando se recibe un mensaje desde un cliente.
    protected override void OnMessage(MessageEventArgs e)
    {
        //TODO Notifica a todos los clientes que el usuario se ha desconectado.
        string userColor = userColors.ContainsKey(userName) ? userColors[userName] : "#FFFFFF";
        //TODO Formatea el mensaje incluyendo el identificador del usuario.
        string formattedMessage = $"<color={userColor}><b>{userName}:</b></color>{e.Data}";

        //TODO Envía el mensaje a todos los clientes conectados.
        Sessions.Broadcast(formattedMessage);
        Debug.Log($"Mensaje de {userName}: {e.Data}");
    }

    //TODO Se invoca cuando un cliente se desconecta.
    protected override void OnClose(CloseEventArgs e)
    {
        //TODO Notifica a todos los clientes que el usuario se ha desconectado.
        string userColor = userColors.ContainsKey(userName) ? userColors[userName] : "#FFFFFF";

        Sessions.Broadcast($"<color={userColor}><b>{userName} se ha desconectado.</b></color>");
        Debug.Log($"{userName} se ha desconectado.");
    }
}
// Comportamiento básico del servicio WebSocket: simplemente devuelve el mensaje recibido.
//public class EchoBehavior : WebSocketBehavior
//{
//    //Se invoca cuando se recibe un mensaje desde un cliente.
//    protected override void OnMessage(MessageEventArgs e)
//    {
// Envía de vuelta el mismo mensaje recibido.
//        Send(e.Data);
//    }
//}
