using System;
using System.Text;
using System.Text.Json;
using GameServer.Schemas;
using QuicNet;
using QuicNet.Streams;
using QuicNet.Connections;

namespace GameServer.Router;

public class GameRouter
{
    private Controllers.Player _playerController;

    public GameRouter()
    {
        _playerController = new Controllers.Player();
    }
    public void ClientConnected(QuicConnection connection)
    {
        Console.WriteLine("Client connected.");
        try
        {
            connection.OnStreamOpened += StreamOpened;
        }
        catch (Exception e)
        {
            Console.WriteLine(e + " \nserver busy");

        }
    }
    
        
    // Fired when a new stream has been opened (It does not carry data with it)
    void StreamOpened(QuicStream stream)
    {
        stream.OnStreamDataReceived += StreamDataReceived;
    }
        
    // Fired when a stream received full batch of data
    void StreamDataReceived(QuicStream stream, byte[] data)
    {
        var (function,Payload) = Schema.DeserializeQueryData<PlayerData>(data);

        switch (function)
        {
            case 0x01://join room
                var playerData = Payload;
                _playerController.Join(stream,playerData);
                
                break;
            case 0x02://get players in range(10 units)
                _playerController.GetPlayers(stream,Payload.x, Payload.y);
                break;
            case 0x03://update player position
               _playerController.UpdatePosition(stream,Payload);
                break;
                
        }
    }

    
}