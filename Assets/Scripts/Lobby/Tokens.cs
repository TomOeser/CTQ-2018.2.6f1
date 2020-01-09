﻿using System;
using UdpKit;

public class RoomProtocolToken : Bolt.IProtocolToken
{
    public String ArbitraryData;
    public String matchName;
    public String mapName;
    public Byte maxPlayers;
    public String password;

    public void Read(UdpPacket packet)
    {
        ArbitraryData = packet.ReadString();
        matchName = packet.ReadString();
        mapName = packet.ReadString();
        maxPlayers = packet.ReadByte();
        password = packet.ReadString();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(ArbitraryData);
        packet.WriteString(matchName);
        packet.WriteString(mapName);
        packet.WriteByte(maxPlayers);
        packet.WriteString(password);
    }
}

public class ServerAcceptToken : Bolt.IProtocolToken
{
    public String data;

    public void Read(UdpPacket packet)
    {
        data = packet.ReadString();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(data);
    }
}

public class ServerConnectToken : Bolt.IProtocolToken
{
    public String data;

    public void Read(UdpPacket packet)
    {
        data = packet.ReadString();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(data);
    }
}